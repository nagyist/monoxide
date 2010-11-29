using System;

namespace System.MacOS
{
	/// <summary>Code template for a native object wrapper.</summary>
	[NativeClass("NSResponder", "AppKit")] // Specify the wrapped/base native class as well a the framework.
	public class NativeObject : IDisposable
	{
		SafeNativeMethods.objc_super super;
		bool disposed;
		
		public NativeObject()
		{
			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
			super.Receiver = ObjectiveC.AllocAndInitObject(nativeClass);
		}
		
		private NativeObject(IntPtr nativePointer)
		{
			super.Class = ObjectiveC.GetNativeBaseClass(ObjectiveC.GetNativeClass(this.GetType(), false));
			super.Receiver = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~NativeObject() { Dispose(false); }
		
		protected void Dispose(bool disposing)
		{
			if (super.Receiver != (IntPtr)0)
			{
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
			disposed = true;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);
				
				return super.Receiver;
			}
		}
	}
}
