using System;

namespace System.MacOS
{
	/// <summary>Code template for a native object wrapper.</summary>
	[NativeClass("NSResponder", "AppKit")] // Specify the wrapped/base native class as well a the framework.
	public class DelayCreatedNativeObject : IDisposable
	{
		SafeNativeMethods.objc_super super;
		bool disposed;
		
		public DelayCreatedNativeObject()
		{
		}
		
		private DelayCreatedNativeObject(IntPtr nativePointer)
		{
			super.Class = ObjectiveC.GetNativeBaseClass(ObjectiveC.GetNativeClass(this.GetType(), false));
			super.Receiver = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~DelayCreatedNativeObject() { Dispose(false); }
		
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
		
		private void CreateNative()
		{
			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
			super.Receiver = ObjectiveC.AllocAndInitObject(nativeClass);
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal bool Created { get { return super.Receiver != (IntPtr)0; } }
		
		internal IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);
				
				if (!Created)
					CreateNative();
				
				return super.Receiver;
			}
		}
	}
}
