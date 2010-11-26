using System;
using System.Security;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Markup;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSWindow", "AppKit")]
	[ContentProperty("Content")]
	public class Window : EventTarget
	{
		#region Method Selector Ids

		#warning Don't forget to remove "Window.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class initWithContentRect_styleMask_backing_defer { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithContentRect:styleMask:backing:defer:"); }
			static class setReleasedWhenClosed { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setReleasedWhenClosed:"); }
			static class isReleasedWhenClosed { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isReleasedWhenClosed"); }
			static class makeKeyAndOrderFront { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("makeKeyAndOrderFront:"); }
			static class orderFront { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("orderFront:"); }
			static class orderOut { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("orderOut:"); }
			static class close { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("close"); }
			static class contentView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("contentView"); }
			static class setContentView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setContentView:"); }
			static class contentSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("contentSize"); }
			static class setContentSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setContentSize:"); }
			static class styleMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("styleMask"); }
			static class setStyleMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setStyleMask:"); }
			static class toolbar { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("toolbar"); }
			static class setToolbar { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setToolbar:"); }
			static class contentBorderThicknessForEdge { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("contentBorderThicknessForEdge:"); }
			static class setContentBorderThicknessForEdge { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setContentBorderThickness:forEdge:"); }
			
			public static IntPtr InitWithContentRectStyleMaskBackingDefer { get { return initWithContentRect_styleMask_backing_defer.SelectorHandle; } }
			
			public static IntPtr SetReleasedWhenClosed { get { return setReleasedWhenClosed.SelectorHandle; } }
			public static IntPtr IsReleasedWhenClosed { get { return isReleasedWhenClosed.SelectorHandle; } }
			
			public static IntPtr MakeKeyAndOrderFront { get { return makeKeyAndOrderFront.SelectorHandle; } }
			public static IntPtr OrderFront { get { return orderFront.SelectorHandle; } }
			public static IntPtr OrderOut { get { return orderOut.SelectorHandle; } }
			
			public static IntPtr Close { get { return close.SelectorHandle; } }
			
			public static IntPtr ContentView { get { return Window.Selectors.contentView.SelectorHandle; } }
			public static IntPtr SetContentView { get { return setContentView.SelectorHandle; } }

			public static IntPtr ContentSize { get { return contentSize.SelectorHandle; } }
			public static IntPtr SetContentSize { get { return setContentSize.SelectorHandle; } }
			
			public static IntPtr StyleMask { get { return styleMask.SelectorHandle; } }
			public static IntPtr SetStyleMask { get { return setStyleMask.SelectorHandle; } }
			
			public static IntPtr Toolbar { get { return Window.Selectors.toolbar.SelectorHandle; } }
			public static IntPtr SetToolbar { get { return setToolbar.SelectorHandle; } }
			
			public static IntPtr ContentBorderThicknessForEdge { get { return contentBorderThicknessForEdge.SelectorHandle; } }
			public static IntPtr SetContentBorderThicknessForEdge { get { return setContentBorderThicknessForEdge.SelectorHandle; } }
		}
		
		#endregion
		
		#region NSWindow Interop
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithContentRect_styleMask_backing_defer_32(IntPtr self, IntPtr sel, RectangleF contentRect, int windowStyle, int bufferingType, [MarshalAs(UnmanagedType.I1)] bool deferCreation);
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithContentRect_styleMask_backing_defer_64(IntPtr self, IntPtr sel, Rectangle contentRect, long windowStyle, long bufferingType, [MarshalAs(UnmanagedType.I1)] bool deferCreation);
		
		static void InitWithContentRectStyleMaskBackingDefer(ref IntPtr nativePointer, Rectangle contentRect, WindowStyle windowStyle, SafeNativeMethods.BackingStoreType bufferingType, bool deferCreation)
		{
			nativePointer = ObjectiveC.LP64 ?
				objc_msgSend_initWithContentRect_styleMask_backing_defer_64(
					nativePointer, Selectors.InitWithContentRectStyleMaskBackingDefer,
					contentRect, (long)windowStyle, (long)bufferingType, deferCreation) :
				objc_msgSend_initWithContentRect_styleMask_backing_defer_32(
					nativePointer, Selectors.InitWithContentRectStyleMaskBackingDefer,
					contentRect, (int)windowStyle, (int)bufferingType, deferCreation);
		}
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern void objc_msgSend_setContentBorderThickness_forEdge_32(IntPtr self, IntPtr sel, float thickness, IntPtr edge);
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern void objc_msgSend_setContentBorderThickness_forEdge_64(IntPtr self, IntPtr sel, double thickness, IntPtr edge);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend_fpret")]
		static extern float objc_msgSend_contentBorderThicknessForEdge_32(IntPtr self, IntPtr sel, IntPtr edge);
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern double objc_msgSend_contentBorderThicknessForEdge_64(IntPtr self, IntPtr sel, IntPtr edge);
		
		static double GetContentBorderThicknessForEdge(IntPtr nativePointer, Edge edge)
		{
			return ObjectiveC.LP64 ?
				objc_msgSend_contentBorderThicknessForEdge_64(
					nativePointer,
					Selectors.ContentBorderThicknessForEdge,
					(IntPtr)edge) :
				objc_msgSend_contentBorderThicknessForEdge_32(
					nativePointer,
					Selectors.ContentBorderThicknessForEdge,
					(IntPtr)edge);
		}
		
		static void SetContentBorderThicknessForEdge(IntPtr nativePointer, double thickness, Edge edge)
		{
			if (ObjectiveC.LP64)
				objc_msgSend_setContentBorderThickness_forEdge_64(
					nativePointer,
					Selectors.SetContentBorderThicknessForEdge,
					thickness,
					(IntPtr)edge);
			else
				objc_msgSend_setContentBorderThickness_forEdge_32(
					nativePointer,
					Selectors.SetContentBorderThicknessForEdge,
					(float)thickness,
					(IntPtr)edge);
		}
		
		#endregion
		
		#region Event Handling & Dispatching
		
		static class Delegate
		{
			public static readonly IntPtr NativePointer = CreateWindowDelegate();
				
			private static IntPtr CreateWindowDelegate()
			{
				// Standard notification handler, that will handle most notification and dispatch them apropriately
				var notificationHandler = (SafeNativeMethods.StandardEventHandler)HandleNotification;
				// Documentation here: http://developer.apple.com/mac/library/documentation/Cocoa/Reference/ObjCRuntimeRef/Reference/reference.html#//apple_ref/c/func/objc_allocateClassPair
				var delegateClass = SafeNativeMethods.objc_allocateClassPair(ObjectiveC.Classes.NSObject, "CLRWindowDelegate", IntPtr.Zero);
				// Add a method to the runtime-created class for each of the delegated event we want to intercept
				SafeNativeMethods.class_addMethod(delegateClass, ObjectiveC.GetSelector("windowWillClose:"), notificationHandler, "v@:@");
				SafeNativeMethods.class_addMethod(delegateClass, ObjectiveC.GetSelector("windowWillMiniaturize:"), notificationHandler, "v@:@");
				SafeNativeMethods.class_addMethod(delegateClass, ObjectiveC.GetSelector("windowDidMiniaturize:"), notificationHandler, "v@:@");
				SafeNativeMethods.class_addMethod(delegateClass, ObjectiveC.GetSelector("windowShouldClose:"), (SafeNativeMethods.EventHandlerWithBooleanResult)windowShouldClose, "c@:@");
				// Register the newly created class
				SafeNativeMethods.objc_registerClassPair(delegateClass);
				// Return an allocated and initialized a instance of our newly created delegate class
				return SafeNativeMethods.objc_msgSend(ObjectiveC.AllocObject(delegateClass), ObjectiveC.Selectors.Init);
			}
			
			private static void HandleNotification(IntPtr self, IntPtr _cmd, IntPtr aNotification)
			{
				var form = GetInstance(ObjectiveC.GetNotificationObject(aNotification));
				
				if (form == null) return;
				
				switch (ObjectiveC.GetNotificationName(aNotification))
				{
					case "NSWindowWillCloseNotification": form.HandleClosed(); break;
					case "NSWindowWillMiniaturizeNotification": form.HandleMinimizing(); break;
					case "NSWindowDidMiniaturizeNotification": form.HandleMinimized(); break;
				}
			}
			
			private static bool windowShouldClose(IntPtr self, IntPtr _cmd, IntPtr sender)
			{
				var form = GetInstance(sender);
				
				if (form == null) return true; // Maybe false would be a better choice, anyway, this case should never happen.
				
				return form.HandleClosing();
			}
		}
		
		#endregion
		
		#region Cache
		
		private static readonly NativeObjectCache<Window> windowCache = new NativeObjectCache<Window>(w => w.NativePointer);
		
		internal static Window GetInstance(IntPtr nativePointer) { return windowCache.GetObject(nativePointer); }
		
		#endregion
		
		public const int RegularBottomBarHeight = 32;
		public const int SmallBottomBarHeight = 22;
		
		private bool disposeWhenClosed;
		private WindowStyle style;
		private Rectangle clientRectangle;
		private double bottomBarHeight;
		private string title;
		private View contentView;
		private Toolbar toolbar;
		
		public event EventHandler Load;
		public event EventHandler<CancelEventArgs> Closing;
		public event EventHandler Closed;
		public event EventHandler Minimizing;
		public event EventHandler Minimized;
		
		public Window()
		{
			#warning Lazy content view creation might be a good idea
			contentView = new View();
			contentView.Owner = this;
			style = WindowStyle.Titled | WindowStyle.Resizable | WindowStyle.Closable | WindowStyle.Miniaturizable;
			clientRectangle = new Rectangle(335, 390, 480, 360);
			disposeWhenClosed = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && contentView != null)
				contentView.Dispose();
			contentView = null;
			if (super.Receiver != IntPtr.Zero)
			{
				windowCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
		}
		
		internal void CreateNative()
		{	
			using (var pool = LocalAutoReleasePool.Create())
			{
				contentView.Measure(clientRectangle.Size);
				contentView.Arrange(new Rectangle(Point.Zero, clientRectangle.Size));
				super.Receiver = ObjectiveC.AllocObject(NativeClass);
				InitWithContentRectStyleMaskBackingDefer(ref super.Receiver, clientRectangle, style, SafeNativeMethods.BackingStoreType.BackingStoreBuffered, true);
				windowCache.RegisterObject(this);
				SafeNativeMethods.objc_msgSend_set_Boolean(super.Receiver, Selectors.SetReleasedWhenClosed, disposeWhenClosed);
				SafeNativeMethods.objc_msgSend(super.Receiver, CommonSelectors.SetDelegate, Delegate.NativePointer);
				SafeNativeMethods.objc_msgSend_set_String(super.Receiver, CommonSelectors.SetTitle, title ?? string.Empty);
				if (toolbar != null)
					SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetToolbar, toolbar.NativePointer);
				SetContentBorderThicknessForEdge(super.Receiver, bottomBarHeight, Edge.Bottom);
				SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetContentView, contentView.NativePointer);
				OnLoad(EventArgs.Empty);
			}
		}
		
		internal override IntPtr NativePointer
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
		
		public bool Visible
		{
			get
			{
				return Created ? SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, CommonSelectors.IsVisible) : false;
			}
			set
			{
				if (value)
					Show();
				else
					Hide();
			}
		}
		
		public string Title
		{
			get
			{
				if (Created)
					title = SafeNativeMethods.objc_msgSend_get_String(NativePointer, CommonSelectors.Title);
				
				return title;
			}
			set
			{
				if (value != title)
				{
					title = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetTitle, title);
				}
			}
		}
		
		public WindowStyle Style
		{
			get
			{
				if (Created)
					style = (WindowStyle)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.StyleMask);
				
				return style;
			}
			set
			{
				if (Created)
					throw new InvalidOperationException();
				
				style = value;
			}
		}
		
		public Toolbar Toolbar
		{
			get { return toolbar; }
			set
			{
				if (value != toolbar)
				{
					toolbar = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetToolbar, toolbar != null ? toolbar.NativePointer : IntPtr.Zero);
				}
			}
		}
		
		public double BottomBarHeight
		{
			get
			{
				if (Created)
					bottomBarHeight = GetContentBorderThicknessForEdge(NativePointer, Edge.Bottom);
				
				return bottomBarHeight;
			}
			set
			{
				if (value < 0 || double.IsNaN(value))
					throw new ArgumentOutOfRangeException();
				
				if (value != bottomBarHeight)
				{
					bottomBarHeight = value;
					
					if (Created)
						SetContentBorderThicknessForEdge(NativePointer, bottomBarHeight, Edge.Bottom);
				}
			}
		}

		public View Content
		{
			get { return contentView; }
			set
			{
				if (value != contentView)
				{
					var newContentView = value ?? new View();

					// Take ownership of the new content view, and free the old one
					newContentView.Owner = this; // This may fail if the view is already owned somewhere else
					contentView.Owner = false;

					// Assign the view now that everything is ok
					contentView = newContentView;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetContentView, contentView.NativePointer);
				}
			}
		}
		
		public bool DisposeWhenClosed
		{
			get
			{
				if (Created)
					disposeWhenClosed = SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, Selectors.IsReleasedWhenClosed);
				
				return disposeWhenClosed;
			}
			set
			{
				if (value != disposeWhenClosed)
				{
					disposeWhenClosed = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetReleasedWhenClosed, value);
				}
			}
		}
		
		public void ShowAndMakeKey()
		{
			if (!Application.Current.IsRunning)
				throw new ApplicationNotRunningException();
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.MakeKeyAndOrderFront, IntPtr.Zero);
		}
		
		public void Show()
		{
			if (!Application.Current.IsRunning)
				throw new ApplicationNotRunningException();
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.OrderFront, IntPtr.Zero);
		}
		
		public void Hide()
		{
			if (Created)
				SafeNativeMethods.objc_msgSend(NativePointer, Selectors.OrderOut, IntPtr.Zero);
		}
		
		public void Close()
		{
			if (!Created)
				throw new InvalidOperationException();
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.Close);
		}
		
		private bool HandleClosing()
		{
			var e = new CancelEventArgs(false);
			
			OnClosing(e);
			
			return !e.Cancel;
		}
		
		private void HandleClosed()
		{
			// The NSWindow object might be about to be released, so we dispose the instance if needed.
			if (DisposeWhenClosed)
			{
				windowCache.UnregisterObject(super.Receiver);
				super.Receiver = IntPtr.Zero; // Pointer will be automatically released by NSWindow or its subclass.
				Dispose();
			}
			
			OnClosed(EventArgs.Empty);
		}
		
		private void HandleMinimizing()
		{
			OnMinimizing(EventArgs.Empty);
		}
		
		private void HandleMinimized()
		{
			OnMinimized(EventArgs.Empty);
		}
		
		protected virtual void OnLoad(EventArgs e)
		{
			if (Load != null)
				Load(this, e);
		}
		
		protected virtual void OnClosing(CancelEventArgs e)
		{
			if (Closing != null)
				Closing(this, e);
		}
		
		protected virtual void OnClosed(EventArgs e)
		{
			if (Closed != null)
				Closed(this, e);
		}
		
		protected virtual void OnMinimizing(EventArgs e)
		{
			if (Minimizing != null)
				Minimizing(this, e);
		}
		
		protected virtual void OnMinimized(EventArgs e)
		{
			if (Minimized != null)
				Minimized(this, e);
		}
	}
}
