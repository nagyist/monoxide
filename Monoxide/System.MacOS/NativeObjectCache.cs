using System;
using System.Collections.Generic;

namespace System.MacOS
{
	public class NativeObjectCache<T>
		where T : class
	{
		readonly Dictionary<IntPtr, T> dictionary;
		readonly Func<T, IntPtr> getPointer;
		readonly Func<IntPtr, T> createObject;
		
		public NativeObjectCache(Func<T, IntPtr> getPointer)
			: this(getPointer, null) { }
		
		public NativeObjectCache(Func<T, IntPtr> getPointer, Func<IntPtr, T> createObject)
		{
			if (getPointer == null)
				throw new ArgumentNullException("getPointer");
			
			this.dictionary = new Dictionary<IntPtr, T>();
			this.getPointer = getPointer;
			this.createObject = createObject;
		}
		
		public void RegisterObject(T @object)
		{
			lock (dictionary)
			{
				var nativePointer = getPointer(@object);
				
				dictionary.Add(nativePointer, @object);
				ObjectiveC.RegisterObjectPair(@object, nativePointer);
			}
		}
		
		public void UnregisterObject(IntPtr nativePointer)
		{
			lock (dictionary)
			{
				ObjectiveC.UnregisterObject(dictionary[nativePointer]);
				dictionary.Remove(nativePointer);
			}
		}
		
		public T GetObject(IntPtr nativePointer)
		{
			lock (dictionary)
			{
				T @object;
				
				if (!dictionary.TryGetValue(nativePointer, out @object) &&
					nativePointer != IntPtr.Zero &&
					createObject != null &&
					(@object = createObject(nativePointer)) != null)
				{
					dictionary.Add(nativePointer, @object);
					ObjectiveC.RegisterObjectPair(@object, nativePointer);
				}
				
				return @object;
			}
		}
	}
}
