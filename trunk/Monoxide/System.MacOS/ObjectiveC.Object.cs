using System;
namespace System.MacOS
{
	partial class ObjectiveC
	{
		/// <summary>Encapsulate a random CLR object in an Objective-C NSObject object.</summary>
		/// <remarks>
		/// This will map to the Objective-C CLRObject class.
		/// This class is static because System.Object is the real wrapped class.
		/// Also, this will serve as a base native bridge implementation for all C# overlay classes.
		/// </remarks>
		[NativeClass("NSObject")]
		internal static class Object
		{
			[SelectorStub("description")]
			private static string GetDescription(IntPtr self, IntPtr _cmd)
			{
				object @object;

				lock (managedToNativeDictionary)
					return ObjectiveC.nativeToManagedDictionary.TryGetValue(self, out @object) ? @object.ToString() : null;
			}

			[SelectorStub("hash")]
			private static IntPtr GetHashCode(IntPtr self, IntPtr _cmd)
			{
				object @object;

				lock (managedToNativeDictionary)
					return checked((IntPtr)(nativeToManagedDictionary.TryGetValue(self, out @object) ? @object.GetHashCode() : 0));
			}

			[SelectorStub("isEqual:")]
			private static bool Equals(IntPtr self, IntPtr _cmd, IntPtr anObject)
			{
				object object1, object2;

				lock (managedToNativeDictionary)
					return self == anObject ||
						nativeToManagedDictionary.TryGetValue(self, out object1) &&
						nativeToManagedDictionary.TryGetValue(anObject, out object2) &&
						object1.Equals(object2);
			}

			[SelectorStub("copyWithZone:")]
			private static IntPtr CopyWithZone(IntPtr self, IntPtr _cmd, IntPtr zone)
			{
				object @object;

				lock (managedToNativeDictionary)
					nativeToManagedDictionary.TryGetValue(self, out @object);

				var cloneable = @object as ICloneable;

				// TODO: implement native cloning protocol
				// Maybe a method like NativeCloned(IntPtr nativePointer)
				if (cloneable != null) return CloneFactory.NativeClone(cloneable);
				else return RetainObject(self); // The object should not be cloned, as it would add no value, and we would lose track of it
			}

			[SelectorStub("dealloc")]
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
		}
	}
}
