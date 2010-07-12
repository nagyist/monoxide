using System;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	public class NativeUrlMarshaler : ICustomMarshaler
	{
		public static readonly NativeUrlMarshaler Default = new NativeUrlMarshaler(true);
		public static readonly NativeUrlMarshaler Keep = new NativeUrlMarshaler(false);
		
		public static ICustomMarshaler GetInstance(string cookie)
		{
			switch (cookie)
			{
				case "k": return Keep;
				default: return Default;
			}
		}
		
		bool freeMemory;
		
		public NativeUrlMarshaler(bool freeMemory) { this.freeMemory = freeMemory; }
		
		#region ICustomMarshaler Implementation
		
		public void CleanUpManagedData(object ManagedObj) { }
		
		public void CleanUpNativeData(IntPtr pNativeData)
		{
			// From what can be found, some objects will take ownership of the NSURL without retaining it…
			// Thus it shouldn't be freed. But maybe this is not correct.
			if (freeMemory && pNativeData != IntPtr.Zero)
				ObjectiveC.ReleaseObject(pNativeData);
		}
		
		public int GetNativeDataSize()
		{
			return IntPtr.Size;
		}
		
		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			return ObjectiveC.UriToNativeUrl((Uri)ManagedObj); // Cast will throw exception on failure
		}
		
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return ObjectiveC.NativeUrlToUri(pNativeData);
		}
		
		#endregion
	}
}
