using System;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	internal sealed class NativeStringMarshaler : ICustomMarshaler
	{
		enum ReleaseType
		{
			None,
			Forced,
			Automatic
		}
		
		public static readonly NativeStringMarshaler NoRelease = new NativeStringMarshaler(ReleaseType.None);
		public static readonly NativeStringMarshaler ForcedRelease = new NativeStringMarshaler(ReleaseType.Forced);
		public static readonly NativeStringMarshaler AutoRelease = new NativeStringMarshaler(ReleaseType.Automatic);
		
		public static ICustomMarshaler GetInstance(string cookie)
		{
			switch (cookie)
			{
				case "n": return NoRelease;
				case "a": return AutoRelease;
				default: return ForcedRelease;
			}
		}
		
		ReleaseType releaseType;
		
		private NativeStringMarshaler(ReleaseType releaseType) { this.releaseType = releaseType; }
		
		#region ICustomMarshaler Implementation
		
		public void CleanUpManagedData(object ManagedObj) { }
		
		public void CleanUpNativeData(IntPtr pNativeData)
		{
			if (releaseType == ReleaseType.Forced && pNativeData != IntPtr.Zero)
				ObjectiveC.ReleaseObject(pNativeData);
		}
		
		public int GetNativeDataSize()
		{
			return IntPtr.Size;
		}
		
		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			var nativePointer = ObjectiveC.StringToNativeString((string)ManagedObj); // Cast will throw exception on failure
			
			if (releaseType == ReleaseType.Automatic)
				ObjectiveC.AutoReleaseObject(nativePointer);
			
			return nativePointer;
		}
		
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return ObjectiveC.NativeStringToString(pNativeData);
		}
		
		#endregion
	}
}
