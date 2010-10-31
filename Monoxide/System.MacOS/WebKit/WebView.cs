using System;
using System.MacOS.AppKit;
using System.Security;
using System.Runtime.InteropServices;
namespace System.MacOS.WebKit
{
	[NativeClass("WebView", "WebKit")]
	public class WebView : View
	{
		#region NSView Interop
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithFrame_frameName_groupName_32(IntPtr self, IntPtr sel, RectangleF frame, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string frameName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string groupName);
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithFrame_frameName_groupName_64(IntPtr self, IntPtr sel, Rectangle frame, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string frameName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string groupName);
		
		#region Method Selector Ids

		#warning Don't forget to remove "WebView.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class initWithFrameFrameNameGroupName { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithFrame:frameName:groupName:"); }
			static class setMainFrameURL { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setMainFrameURL:"); }
			static class mainFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("mainFrame"); }
			
			public static IntPtr InitWithFrameFrameNameGroupName { get { return initWithFrameFrameNameGroupName.SelectorHandle; } }
			public static IntPtr SetMainFrameURL { get { return setMainFrameURL.SelectorHandle; } }
			public static IntPtr MainFrame { get { return WebView.Selectors.mainFrame.SelectorHandle; } }
		}
		
		#endregion
		
		#endregion
		
		WebFrame mainFrame;
		string frameName;
		//string url;
		
		public WebView()
			: this(null) { }
		
		public WebView(string frameName)
		{
			this.frameName = frameName;
			ObjectiveC.EnsureWebKitFrameworkIsLoaded();
		}
		
		internal override void InitializeNative(ref IntPtr nativePointer)
		{
			InitWithFrameFrameNameGroupName(ref nativePointer, Frame, frameName, null);
		}
		
		private static void InitWithFrameFrameNameGroupName(ref IntPtr nativePointer, Rectangle bounds, string frameName, string groupName)
		{
			nativePointer = ObjectiveC.LP64 ?
				objc_msgSend_initWithFrame_frameName_groupName_64(nativePointer, Selectors.InitWithFrameFrameNameGroupName, bounds, frameName, groupName) :
				objc_msgSend_initWithFrame_frameName_groupName_32(nativePointer, Selectors.InitWithFrameFrameNameGroupName, bounds, frameName, groupName);
		}
		
		public void SetMainFrameUrl(Uri url)
		{
			SetMainFrameUrl(url.ToString());
		}
		
		public void SetMainFrameUrl(string url)
		{
			if (MainFrame == null) throw new NullReferenceException();
			SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetMainFrameURL, url);
		}
		
		public WebFrame MainFrame
		{
			get
			{
				return Created && mainFrame == null ?
					mainFrame = WebFrame.GetInstance(SafeNativeMethods.objc_msgSend(NativePointer, Selectors.MainFrame)) :
					mainFrame;
			}
		}
	}
}
