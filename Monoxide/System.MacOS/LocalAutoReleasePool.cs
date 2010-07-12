using System;
using System.Diagnostics;

namespace System.MacOS
{
	struct LocalAutoReleasePool : IDisposable
	{
		IntPtr nativePointer;
		
		public static LocalAutoReleasePool Create()
		{
			LocalAutoReleasePool @this;
			
			@this.nativePointer = ObjectiveC.AllocAndInitObject(ObjectiveC.Classes.NSAutoreleasePool);
#if DEBUG && VERBOSE
			Debug.WriteLine("Local NSAutoReleasePool created: " + pool.nativePointer.ToString("X16"));
#endif
			
			return @this;
		}
		
		public void Dispose()
		{
			if (nativePointer != IntPtr.Zero)
			{
				//ObjectiveC.ReleaseObject(nativePointer);
				SafeNativeMethods.objc_msgSend(nativePointer, ObjectiveC.Selectors.Drain);
#if DEBUG && VERBOSE
				Debug.WriteLine("Local NSAutoReleasePool drained: " + nativePointer.ToString("X16"));
#endif
				nativePointer = IntPtr.Zero;
			}
		}
	}
}
