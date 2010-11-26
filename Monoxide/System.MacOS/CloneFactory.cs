using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace System.MacOS
{
	internal static class CloneFactory
	{
		public delegate IntPtr CloneHandler(ICloneable @object);

		private static Dictionary<Type, CloneHandler> cloneHandlers = new Dictionary<Type, CloneHandler>();

		public static IntPtr NativeClone(ICloneable @object)
		{
			CloneHandler clone;
			var type = @object.GetType();

			lock (cloneHandlers)
				if (!cloneHandlers.TryGetValue(type, out clone))
					cloneHandlers.Add(type, clone = GetHandler(type));

			return clone(@object); // This code may have side effects and should not be run from the lock
		}

		private static CloneHandler GetHandler(Type type)
		{
			var attributes = type.GetCustomAttributes(typeof(NativeClassAttribute), true);

			if (attributes.Length > 0) return CreateCloneWrapper(type);
			else return CloneRegular;
		}

		private static CloneHandler CreateCloneWrapper(Type type)
		{
			var cloneMethod = new DynamicMethod
			(
				"NativeClone",
				MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot,
				CallingConventions.Standard,
				typeof(IntPtr), new Type[] { type },
				type, false
			);
			var ilGenerator = cloneMethod.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0); // Step 1: Push the object to clone on the stack
			// Just to be clean, don't suppose ICloneable only has one member…
			var cloneableInterfaceMap = type.GetInterfaceMap(typeof(ICloneable));
			for (int i = 0; i < cloneableInterfaceMap.InterfaceMethods.Length; i++)
				if (cloneableInterfaceMap.InterfaceMethods[i].Name == "Clone")
				{
					ilGenerator.Emit(OpCodes.Call, cloneableInterfaceMap.TargetMethods[i]); // Step 2: clone it
					goto CloneMethodFound; // Finish the job once we found the Clone method (which should always be found)
				}
			throw new InvalidOperationException(); // This line should never be reached
		CloneMethodFound:
			ilGenerator.Emit(OpCodes.Isinst, type); // Step 3: Cast it to the correct type
			var nativePointerProperty = type.GetProperty
			(
				"NativePointer",
				BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
				typeof(IntPtr), Type.EmptyTypes, null
			);
			ilGenerator.Emit(OpCodes.Call, nativePointerProperty.GetGetMethod(true)); // Step 4: Get the native pointer
			ilGenerator.Emit(OpCodes.Ret); // Step 5: Return the value

			return cloneMethod.CreateDelegate(typeof(CloneHandler)) as CloneHandler;
		}

		private static IntPtr CloneRegular(ICloneable @object)
		{
			return ObjectiveC.GetNativeObject(@object.Clone());
		}

		private static IntPtr CloneWrapperImpl(ICloneable @object)
		{
			var clone = @object.Clone();

			return (IntPtr)clone.GetType().InvokeMember("NativePointer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, Type.DefaultBinder, clone, null);
		}
	}
}
