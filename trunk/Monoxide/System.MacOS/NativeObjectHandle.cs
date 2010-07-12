using System;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	public class NativeObjectHandle : SafeHandle
	{
		public NativeObjectHandle()
			: base(IntPtr.Zero, true) { }
		
		public override bool IsInvalid { get { return handle == IntPtr.Zero; } }
		
		protected override bool ReleaseHandle()
		{
			if (handle == IntPtr.Zero)
				return true;
			
			ObjectiveC.ReleaseObject(handle);
			handle = IntPtr.Zero;
			
			return true;
		}
	}
}
