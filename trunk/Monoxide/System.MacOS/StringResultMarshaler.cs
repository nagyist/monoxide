using System;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	public class StringResultMarshaler : ICustomMarshaler
	{
		public static readonly StringResultMarshaler Default = new StringResultMarshaler();
		
		public static ICustomMarshaler GetInstance(string cookie)
		{
			return Default;
		}
		
		#region ICustomMarshaler Implementation
		
		public void CleanUpManagedData(object ManagedObj) { }
		
		public void CleanUpNativeData(IntPtr pNativeData) { }
		
		public int GetNativeDataSize()
		{
			return IntPtr.Size;
		}
		
		public IntPtr MarshalManagedToNative(object ManagedObj) { return IntPtr.Zero; }
		
		public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return new string((sbyte *)pNativeData);
		}
		
		#endregion
	}
}
