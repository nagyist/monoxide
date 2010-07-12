using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security;

namespace System.MacOS
{
	partial class ObjectiveC
	{
		private const string GlueAssemblyName = "System.MacOS.Objective-C~Glue";
		
		private static Dictionary<IntPtr, IntPtr> nativeBaseClassDictionary = new Dictionary<IntPtr, IntPtr>();
		private static Dictionary<Type, IntPtr> baseClassDictionary = new Dictionary<Type, IntPtr>();
		private static Dictionary<Type, IntPtr> bridgeClassDictionary = new Dictionary<Type, IntPtr>();
		private static HashSet<string> bridgeClassNames = new HashSet<string>();
		private static AssemblyBuilder glueAssembly = CreateGlueAssembly();
		private static ModuleBuilder glueModule = glueAssembly.DefineDynamicModule(GlueAssemblyName);
		private static HashSet<Type> createdDelegateTypes = new HashSet<Type>();
		/// <summary>Keeps a reference on marshaled delegates.</summary>
		/// <remarks>
		/// When delegates are marshaled to native code, it is necessary to keep a reference somewhere to prevent garbage collection.
		/// Here, the marshaled delegates should never be released, so we keep permanent references.
		/// </remarks>
		private static List<Delegate> boundDelegates = new List<Delegate>();
		
		[return: MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler), MarshalCookie = "a")]
		private delegate string DescriptionDelegate(IntPtr self, IntPtr _cmd);
		private delegate IntPtr HashDelegate(IntPtr self, IntPtr _cmd);
		private delegate bool IsEqualDelegate(IntPtr self, IntPtr _cmd, IntPtr obj);
		private delegate IntPtr CopyWithZoneDelegate(IntPtr self, IntPtr _cmd, IntPtr zone);
		private delegate void DeallocDelegate(IntPtr self, IntPtr _cmd);
		
		private static string GetDescription(IntPtr self, IntPtr _cmd)
		{
			object @object;
			
			lock (managedToNativeDictionary)
				return nativeToManagedDictionary.TryGetValue(self, out @object) ? @object.ToString() : null;
		}
		
		private static IntPtr GetHash(IntPtr self, IntPtr _cmd)
		{
			object @object;
			
			lock (managedToNativeDictionary)
				return checked((IntPtr)(nativeToManagedDictionary.TryGetValue(self, out @object) ? @object.GetHashCode() : 0));
		}
		
		private static bool IsEqual(IntPtr self, IntPtr _cmd, IntPtr anObject)
		{
			object object1, object2;
			
			lock (managedToNativeDictionary)
				return self == anObject ||
					nativeToManagedDictionary.TryGetValue(self, out object1) &&
					nativeToManagedDictionary.TryGetValue(anObject, out object2) &&
					object1.Equals(object2);
		}
		
		private static IntPtr CopyWithZone(IntPtr self, IntPtr _cmd, IntPtr zone)
		{
			return self; // The object should not be cloned, as it would add no value, and we would lose track of it
		}
		
		private static void Dealloc(IntPtr self, IntPtr _cmd)
		{
			object @object;
			
			lock (managedToNativeDictionary)
			{
				if (nativeToManagedDictionary.TryGetValue(self, out @object))
				{
					nativeToManagedDictionary.Remove(self);
					managedToNativeDictionary.Remove(@object);
				}
				
				var super = new SafeNativeMethods.objc_super(self, Classes.NSObject);
				SafeNativeMethods.objc_msgSendSuper(ref super, _cmd);
			}
		}
		
		/// <summary>Gets a native Objective-C class for the specified CLR type.</summary>
		/// <param name="type">The <see cref="Type"/> for which to provide an Objective-C class.</param>
		/// <param name="subclass">A flag indicating whether the base Objective-C class should be subclassed.</param>
		/// <returns>An <see cref="IntPtr"/> representing the associated Objective-C class.</returns>
		/// <remarks>
		/// The base class for the new Objective-C class will be determined by using the <see cref="NativeClassAttribute"/>
		/// applied on the type or its ancestors. In order to provide the wrapper class for one given type, the native classes
		/// for all the inheritance chain up to the last type decorated with <see cref="NativeClassAttribute"/>, must be
		/// determined.
		/// The rules for determining the class are designed for maximum interop performance, thus, only overriden methods on
		/// the managed side will be overriden on the native side.
		/// Decorating a class with the <see cref="NativeClassAttribute"/> will imply the creation of a new Objective-C class,
		/// while otherwise, the native class of one of the ancestors for the specified type might be used.
		/// </remarks>
		internal static IntPtr GetNativeClass(Type type, bool subclass)
		{
			//Debug.Assert(type.IsClass);
			
			lock (bridgeClassDictionary)
				return GetNativeClassInternal(type, subclass);
		}
		
		// TODO: Look for ways to improve handling of generic types… (For Control<> and subclasses)
		// Currently we have to create one native class for every constructed generic managed type.
		// This is because selector stubs are implemented as static methods in the type.
		// They need to know the fully constructed type in order to call methods on it.
		// But very few of them really need the generic type parameter.
		private static IntPtr GetNativeClassInternal(Type type, bool subclass)
		{
			// The rules for bridging are as follow:
			// - There must be a private static method with the SelectorImplementationAttribute applied.
			// - There must be a virtual instance method with the BridgeSelectorAttribute applied.
			// - The static method must route calls to the instance method (cannot be verified).
			//   If the method is overriden from a base class, the attribute is inherited, and the method was not
			//   bridged in the previous subclass, or if the bridging is forced by BridgeSelectorAttribute.Force,
			//   then the method should be bridged.
			// Bridging implies binding the static method to the native class definition.
			
			if (type == null)
			{
				nativeBaseClassDictionary.Add(Classes.NSObject, Classes.NSObject);
				return Classes.NSObject;
			}
			//else if (type.IsGenericType && !type.IsGenericTypeDefinition)
			//	return GetNativeClassInternal(type.GetGenericTypeDefinition(), subclass);
			
			IntPtr bridgeClass;
			IntPtr baseClass; // This may be a native<->managed bridge class
			IntPtr nativeBaseClass; // This shall always be a pure native class (e.g. NSObject)
			Type referenceAncestor = null;
			
			if (subclass && bridgeClassDictionary.TryGetValue(type, out bridgeClass)) return bridgeClass;
			else if (!subclass && baseClassDictionary.TryGetValue(type, out baseClass)) return baseClass;
			
			// Find the base class from attributes or ancestors
			{
				var classAttributes = type.GetCustomAttributes(typeof(NativeClassAttribute), false) as NativeClassAttribute[];
				
				Debug.Assert(classAttributes.Length <= 1);
				
				// The recursive call can be optimized later if needed.
				nativeBaseClass = classAttributes.Length > 0 ?
					baseClass = classAttributes[0].Class :
					nativeBaseClassDictionary[baseClass = GetNativeClassInternal(type.BaseType, true)];
			}
			
			// The key may already be present, so only add it if needed
			if (!subclass || !baseClassDictionary.ContainsKey(type)) // Evaluating subclass will be faster than key lookup.
				baseClassDictionary.Add(type, baseClass);
			
			if (!nativeBaseClassDictionary.ContainsKey(baseClass))
				nativeBaseClassDictionary.Add(baseClass, nativeBaseClass);
			
			if (!subclass) return baseClass;
			
			// Declare containers for method analysis
			var selectorStubDictionary = new Dictionary<IntPtr, MethodInfo>();
			var staticSelectors = new HashSet<IntPtr>();
			var requiredSelectors = new HashSet<IntPtr>();
			var forbiddenSelectors = new HashSet<IntPtr>();
			
			// Parse the hierarchy for selector stub methods.
			// These methods must be static and private.
			// Selector stub methods are here as helper methods for message dispatching
			{
				var currentType = type;
				
				while (currentType != null)
				{
					if (referenceAncestor == null && type.GetCustomAttributes(typeof(NativeClassAttribute), false).Length > 0)
						referenceAncestor = currentType;
					
					var methods = currentType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
					
					foreach (var method in methods)
					{
						var selectorAttributes = method.GetCustomAttributes(typeof(SelectorStubAttribute), true) as SelectorStubAttribute[];
						
						// A method can implement multiple selectors
						foreach (var attribute in selectorAttributes)
						{
							selectorStubDictionary.Add(attribute.Selector, method);
							
							if (attribute.Kind == StubKind.ClassMandatory)
								staticSelectors.Add(attribute.Selector);
							else if (attribute.Kind == StubKind.InstanceMandatory && (currentType == type || referenceAncestor == type))
								requiredSelectors.Add(attribute.Selector);
						}
					}
					
					currentType = currentType.BaseType;
				}
			}
			
			if (referenceAncestor == type)
				referenceAncestor = null;
			
			// Parse the hierarchy for selector implementation methods.
			// These methods must not be static, and should preferably be either public or protected.
			// Selector implementation methods depends on selector stub methods for proper functionning, that's why the stub
			// detection is done in a separate step.
			{
				var currentType = type;
				
				while (currentType != null && (referenceAncestor == null || currentType.IsSubclassOf(referenceAncestor)))
				{
					var methods = currentType.GetMethods(BindingFlags.Instance |
						BindingFlags.Public | BindingFlags.NonPublic |
						BindingFlags.DeclaredOnly);
					
					foreach (var method in methods)
					{
						var inheritedSelectorAttributes = method.GetCustomAttributes(typeof(SelectorImplementationAttribute), true) as SelectorImplementationAttribute[];
						var declaredSelectorAttributes = method.GetCustomAttributes(typeof(SelectorImplementationAttribute), false) as SelectorImplementationAttribute[];
						
						if (inheritedSelectorAttributes.Length == 0) continue;
						
						Debug.Assert(inheritedSelectorAttributes.Length == 1);
						
						var attribute = inheritedSelectorAttributes[0];
						
						Debug.Assert(selectorStubDictionary.ContainsKey(attribute.Selector));
						
						if (declaredSelectorAttributes.Length > 0)
						{
							attribute = declaredSelectorAttributes[0]; // This line is probably not needed…
							
							if (attribute.BridgeMode != BridgeMode.Force)
							{
								if (!requiredSelectors.Contains(attribute.Selector))
									forbiddenSelectors.Add(attribute.Selector);
							}
							else if (!forbiddenSelectors.Contains(attribute.Selector))
								requiredSelectors.Add(attribute.Selector);
						}
						else if (attribute.BridgeMode != BridgeMode.Automatic && attribute.BridgeMode != BridgeMode.Force)
						{
							if (!requiredSelectors.Contains(attribute.Selector))
								forbiddenSelectors.Add(attribute.Selector);
						}
						else if (!forbiddenSelectors.Contains(attribute.Selector))
							requiredSelectors.Add(attribute.Selector);
					}
					
					currentType = currentType.BaseType;
				}
			}
			
			if (requiredSelectors.Count > 0 || staticSelectors.Count > 0)
			{
				var className = "CLR" + type.Name;
				var i = 1;
				
				while (bridgeClassNames.Contains(className)) className = "CLR" + type.Name + (++i).ToString();
				
				bridgeClassNames.Add(className);
			
				bridgeClass = CreateBridgeClassBase(baseClass, className);
				
				{
					var metaClass = SafeNativeMethods.object_getClass(bridgeClass);
					
					foreach (var staticSelector in staticSelectors)
					{
						var method = selectorStubDictionary[staticSelector];
						
						AddMethod(metaClass, staticSelector, method, null);
					}
				}
				
				foreach (var requiredSelector in requiredSelectors)
				{
					var method = selectorStubDictionary[requiredSelector];
					
					AddMethod(bridgeClass, requiredSelector, method, null);
				}
				
				nativeBaseClassDictionary.Add(bridgeClass, nativeBaseClass);
			}
			else
				bridgeClass = baseClass;
			
			bridgeClassDictionary.Add(type, bridgeClass);
			
			return bridgeClass;
		}
		
		internal static IntPtr GetNativeBaseClass(IntPtr nativeClass)
		{
			lock (bridgeClassDictionary)
				return nativeBaseClassDictionary[nativeClass];
		}
		
		internal static IntPtr CreateBridgeClassBase(IntPtr nativeClass, string className)
		{
			// Documentation here: http://developer.apple.com/mac/library/documentation/Cocoa/Reference/ObjCRuntimeRef/Reference/reference.html#//apple_ref/c/func/objc_allocateClassPair
			var wrapperClass = SafeNativeMethods.objc_allocateClassPair(nativeClass, className, IntPtr.Zero);
			// Add a method to the runtime-created class for each message we need to override
			SafeNativeMethods.class_addMethod(wrapperClass, Selectors.Description, (DescriptionDelegate)GetDescription, "@@:");
			SafeNativeMethods.class_addMethod(wrapperClass, Selectors.Hash, (HashDelegate)GetHash, LP64 ? "L@:" : "I@:");
			SafeNativeMethods.class_addMethod(wrapperClass, Selectors.IsEqual, (IsEqualDelegate)IsEqual, "c@:@");
			// Add methods for wrapper object management
			if (nativeClass == ObjectiveC.Classes.NSObject)
			{
				SafeNativeMethods.class_addMethod(wrapperClass, Selectors.CopyWithZone, (CopyWithZoneDelegate)CopyWithZone, "@@:^{_NSZone=}");
				SafeNativeMethods.class_addMethod(wrapperClass, Selectors.Dealloc, (DeallocDelegate)Dealloc, "v@:");
			}
			// Register the newly created class
			SafeNativeMethods.objc_registerClassPair(wrapperClass);
					
			return wrapperClass;
		}
		
		private static AssemblyBuilder CreateGlueAssembly()
		{
			var customAttributes = new CustomAttributeBuilder[]
			{
				new CustomAttributeBuilder(typeof(InternalsVisibleToAttribute).GetConstructor(new [] { typeof(string) }),
					new [] { typeof(ObjectiveC).Assembly.FullName})
			};
			
			var assemblyName = new AssemblyName(GlueAssemblyName);
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run, customAttributes);
			
			return assembly;
		}
		
		private static void AddMethod(IntPtr nativeClass, IntPtr selector, MethodInfo method, string signature)
		{
			Type delegateType = null;
			var parameters = method.GetParameters();
			
			foreach (var type in createdDelegateTypes)
			{
				var invokeMethod = type.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
				
				if (invokeMethod.ReturnType != method.ReturnType) continue;
				
				var invokeParameters = invokeMethod.GetParameters();
				
				if (invokeParameters.Length != parameters.Length) continue;
				
				for (int i = 0; i < invokeParameters.Length; i++)
					if (invokeParameters[i].ParameterType != parameters[i].ParameterType)
						continue;
				
				delegateType = type;
				
				break;
			}
			
			if (delegateType == null)
			{
				var marshalAsConstructor = typeof(MarshalAsAttribute).GetConstructor(new Type[] { typeof(UnmanagedType) });
				var marshalAsTypeRef = typeof(MarshalAsAttribute).GetField("MarshalTypeRef");
				var marshalAsCookie = typeof(MarshalAsAttribute).GetField("MarshalCookie");
				var stringBuilder = new StringBuilder("GlueDelegate_");
				var parameterTypes = new Type[parameters.Length];
				
				stringBuilder.Append(method.ReturnType.Name);
				stringBuilder.Append('_');
				
				for (int i = 0; i < parameters.Length; i++)
					stringBuilder.Append((parameterTypes[i] = parameters[i].ParameterType).Name);
				
				var newType = glueModule.DefineType
				(
					stringBuilder.ToString(),
					TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AutoClass,
					typeof(MulticastDelegate)
				);
				
				newType.DefineConstructor
				(
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName,
					CallingConventions.Standard,
					new [] { typeof(object), typeof(IntPtr) }
				).SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
				
				var invokeMethod = newType.DefineMethod
				(
					"Invoke",
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
					method.ReturnType, parameterTypes
				);
				
				invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
				
				invokeMethod.SetCustomAttribute
				(
					new CustomAttributeBuilder
					(
						typeof(SuppressUnmanagedCodeSecurityAttribute).GetConstructor(Type.EmptyTypes),
						new object[0]
					)
				);
				
				if (!method.ReturnType.IsSubclassOf(typeof(ValueType)) || method.ReturnType == typeof(bool))
				{
					var returnParameter = invokeMethod.DefineParameter
					(
						0,
						ParameterAttributes.Retval | ParameterAttributes.HasFieldMarshal,
						null
					);
					
					if (method.ReturnType == typeof(bool))
						returnParameter.SetCustomAttribute
						(
							new CustomAttributeBuilder(marshalAsConstructor,
							new object[] { UnmanagedType.I1 })
						);
					else if (method.ReturnType == typeof(string))
						returnParameter.SetCustomAttribute
						(
							new CustomAttributeBuilder(marshalAsConstructor,
							new object[] { UnmanagedType.CustomMarshaler },
							new [] { marshalAsTypeRef, marshalAsCookie },
							new object[] { typeof(NativeStringMarshaler), "a" })
						);
					else if (method.ReturnType == typeof(Uri))
						returnParameter.SetCustomAttribute
						(
							new CustomAttributeBuilder(marshalAsConstructor,
							new object[] { UnmanagedType.CustomMarshaler },
							new [] { marshalAsTypeRef },
							new object[] { typeof(NativeUrlMarshaler) })
						);
					else
						throw new InvalidCastException(Localization.GetExceptionText("BridgingUnknownType", method.ReturnType));
				}
				
				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i];
					
					if (parameter.ParameterType.IsSubclassOf(typeof(ValueType)) && parameter.ParameterType != typeof(bool)) continue;
					
					var parameterDefinition = invokeMethod.DefineParameter
					(
						i + 1,
						parameters[i].Attributes | ParameterAttributes.HasFieldMarshal,
						null
					);
					
					if (parameter.ParameterType == typeof(bool))
						parameterDefinition.SetCustomAttribute
						(
							new CustomAttributeBuilder(marshalAsConstructor,
							new object[] { UnmanagedType.I1 })
						);
					else if (parameter.ParameterType == typeof(string))
						parameterDefinition.SetCustomAttribute
						(
							new CustomAttributeBuilder(marshalAsConstructor,
							new object[] { UnmanagedType.CustomMarshaler },
							new [] { marshalAsTypeRef, marshalAsCookie },
							new object[] { typeof(NativeStringMarshaler), "n" })
						);
						//parameterDefinition.SetMarshal(UnmanagedMarshal.DefineCustom(typeof(NativeStringMarshaler), "n", typeof(NativeStringMarshaler).FullName, Guid.Empty));
					else if (parameter.ParameterType == typeof(Uri))
						parameterDefinition.SetCustomAttribute
						(
							new CustomAttributeBuilder(marshalAsConstructor,
							new object[] { UnmanagedType.CustomMarshaler },
							new [] { marshalAsTypeRef },
							new object[] { typeof(NativeUrlMarshaler) })
						);
					else
						throw new InvalidCastException(Localization.GetExceptionText("BridgingUnknownType", parameter.ParameterType));
				}
				
				delegateType = newType.CreateType();
				createdDelegateTypes.Add(delegateType);
			}
			
			var createdDelegate = Delegate.CreateDelegate(delegateType, method);
			boundDelegates.Add(createdDelegate); // Prevents the delegate from being garbage collected
			SafeNativeMethods.class_addMethod(nativeClass, selector, createdDelegate, signature);
		}
	}
}
