using System;
using System.Diagnostics;

namespace System.MacOS.AppKit
{
	public sealed class AutoReleasePool : IDisposable
	{
		IntPtr nativePointer;
		
		public AutoReleasePool()
		{
			nativePointer = ObjectiveC.AllocAndInitObject(ObjectiveC.Classes.NSAutoreleasePool);
#if DEBUG && VERBOSE
			Debug.WriteLine("NSAutoReleasePool created: " + nativePointer.ToString("X16"));
#endif
		}
		
		~AutoReleasePool() { Dispose(false); }
		
		private void Dispose(bool disosing)
		{
			if (nativePointer != IntPtr.Zero)
			{
				//ObjectiveC.ReleaseObject(nativePointer);
				SafeNativeMethods.objc_msgSend(nativePointer, ObjectiveC.Selectors.Drain);
#if DEBUG && VERBOSE
				Debug.WriteLine("NSAutoReleasePool drained: " + nativePointer.ToString("X16"));
#endif
				nativePointer = IntPtr.Zero;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		public bool Disposed { get { return nativePointer == IntPtr.Zero; } }
	}
}
