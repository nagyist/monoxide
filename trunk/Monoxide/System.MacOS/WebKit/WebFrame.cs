using System;
using System.Security;
using System.Runtime.InteropServices;

namespace System.MacOS.WebKit
{
	public class WebFrame : IDisposable
	{
		#region WebFrame Interop
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_loadHTMLString_baseURL(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string @string, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeUrlMarshaler), MarshalCookie = "k")] Uri url);
		
		#region Method Selector Ids
		
		static class Selectors
		{
			static class loadHTMLStringBaseUrl { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("loadHTMLString:baseURL:"); }
						
			public static IntPtr LoadHTMLStringBaseUrl { get { return loadHTMLStringBaseUrl.SelectorHandle; } }
		}
		
		#endregion
		
		#endregion

		#region Cache
		
		static readonly NativeObjectCache<WebFrame> webFrameCache = new NativeObjectCache<WebFrame>(wf => wf.NativePointer, p => new WebFrame(p));
		
		internal static WebFrame GetInstance(IntPtr nativePointer) { return webFrameCache.GetObject(nativePointer); }
		
		#endregion

		IntPtr nativePointer;
		bool disposed;
		
		private WebFrame(IntPtr nativePointer)
		{
			this.nativePointer = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~WebFrame() { Dispose(false); }
		
		protected virtual void Dispose(bool disposing)
		{
			if (nativePointer != IntPtr.Zero)
			{
				ObjectiveC.ReleaseObject(nativePointer);
				nativePointer = IntPtr.Zero;
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
				
				return nativePointer;
			}
		}
		
		public void SetHtmlContent(string content, Uri baseUrl)
		{
			objc_msgSend_loadHTMLString_baseURL(nativePointer, Selectors.LoadHTMLStringBaseUrl, content, baseUrl);
		}
	}
}
