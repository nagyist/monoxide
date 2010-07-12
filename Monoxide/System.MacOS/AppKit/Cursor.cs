using System;

namespace System.MacOS.AppKit
{
	public sealed class Cursor
	{
		#region Method Selector Ids
		
		static class Selectors
		{
			static class currentCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("currentCursor"); }
			static class currentSystemCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("currentSystemCursor"); }
			static class arrowCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("arrowCursor"); }
			static class contextualMenuCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("contextualMenuCursor"); }
			static class closedHandCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("closedHandCursor"); }
			static class crosshairCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("crosshairCursor"); }
			static class disappearingItemCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("disappearingItemCursor"); }
			static class dragCopyCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dragCopyCursor"); }
			static class dragLinkCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dragLinkCursor"); }
			static class _IBeamCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("IBeamCursor"); }
			static class openHandCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("openHandCursor"); }
			static class operationNotAllowedCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("operationNotAllowedCursor"); }
			static class pointingHandCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("pointingHandCursor"); }
			static class resizeDownCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeDownCursor"); }
			static class resizeLeftCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeLeftCursor"); }
			static class resizeLeftRightCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeLeftRightCursor"); }
			static class resizeRightCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeRightCursor"); }
			static class resizeUpCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeUpCursor"); }
			static class resizeUpDownCursor { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeUpDownCursor"); }
			
			public static IntPtr CurrentCursor { get { return currentCursor.SelectorHandle; } }
			public static IntPtr CurrentSystemCursor { get { return currentSystemCursor.SelectorHandle; } }
			public static IntPtr ArrowCursor { get { return arrowCursor.SelectorHandle; } }
			public static IntPtr ContextualMenuCursor { get { return contextualMenuCursor.SelectorHandle; } }
			public static IntPtr ClosedHandCursor { get { return closedHandCursor.SelectorHandle; } }
			public static IntPtr CrosshairCursor { get { return crosshairCursor.SelectorHandle; } }
			public static IntPtr DisappearingItemCursor { get { return disappearingItemCursor.SelectorHandle; } }
			public static IntPtr DragCopyCursor { get { return dragCopyCursor.SelectorHandle; } }
			public static IntPtr DragLinkCursor { get { return dragLinkCursor.SelectorHandle; } }
			public static IntPtr IBeamCursor { get { return _IBeamCursor.SelectorHandle; } }
			public static IntPtr OpenHandCursor { get { return openHandCursor.SelectorHandle; } }
			public static IntPtr OperationNotAllowedCursor { get { return operationNotAllowedCursor.SelectorHandle; } }
			public static IntPtr PointingHandCursor { get { return pointingHandCursor.SelectorHandle; } }
			public static IntPtr ResizeDownCursor { get { return resizeDownCursor.SelectorHandle; } }
			public static IntPtr ResizeLeftCursor { get { return resizeLeftCursor.SelectorHandle; } }
			public static IntPtr ResizeLeftRightCursor { get { return resizeLeftRightCursor.SelectorHandle; } }
			public static IntPtr ResizeRightCursor { get { return resizeRightCursor.SelectorHandle; } }
			public static IntPtr ResizeUpCursor { get { return resizeUpCursor.SelectorHandle; } }
			public static IntPtr ResizeUpDownCursor { get { return resizeUpDownCursor.SelectorHandle; } }
		}
		
		#endregion

		#region Cache
		
		static readonly NativeObjectCache<Cursor> cursorCache = new NativeObjectCache<Cursor>(c => c.NativePointer, p => new Cursor(p));
		
		internal static Cursor GetInstance(IntPtr nativePointer) { return cursorCache.GetObject(nativePointer); }
		
		#endregion
		
		private static Cursor GetCursor(IntPtr selector)
		{
			return GetInstance(SafeNativeMethods.objc_msgSend(CommonClasses.NSCursor, selector));
		}
		
		public static Cursor Current { get { return GetCursor(Selectors.CurrentCursor); } }
		//public static Cursor SystemCurrent { get { return GetCursor(Selectors.CurrentSystemCursor); } }
		public static Cursor Arrow { get { return GetCursor(Selectors.ArrowCursor); } }
		//public static Cursor ContextualMenu { get { return GetCursor(Selectors.ContextualMenuCursor); } }
		public static Cursor ClosedHand { get { return GetCursor(Selectors.ClosedHandCursor); } }
		public static Cursor Crosshair { get { return GetCursor(Selectors.CrosshairCursor); } }
		public static Cursor DisappearingItem { get { return GetCursor(Selectors.DisappearingItemCursor); } }
		//public static Cursor DragCopy { get { return GetCursor(Selectors.DragCopyCursor); } }
		//public static Cursor DragLink { get { return GetCursor(Selectors.DragLinkCursor); } }
		public static Cursor IBeam { get { return GetCursor(Selectors.IBeamCursor); } }
		public static Cursor OpenHand { get { return GetCursor(Selectors.OpenHandCursor); } }
		//public static Cursor OperationNotAllowed { get { return GetCursor(Selectors.OperationNotAllowedCursor); } }
		public static Cursor PointingHand { get { return GetCursor(Selectors.PointingHandCursor); } }
		public static Cursor ResizeDown { get { return GetCursor(Selectors.ResizeDownCursor); } }
		public static Cursor ResizeLeft { get { return GetCursor(Selectors.ResizeLeftCursor); } }
		public static Cursor ResizeLeftRight { get { return GetCursor(Selectors.ResizeLeftRightCursor); } }
		public static Cursor ResizeRight { get { return GetCursor(Selectors.ResizeRightCursor); } }
		public static Cursor ResizeUp { get { return GetCursor(Selectors.ResizeUpCursor); } }
		public static Cursor ResizeUpDown { get { return GetCursor(Selectors.ResizeUpDownCursor); } }
		
		IntPtr nativePointer;
		bool disposed;
		
		private Cursor(IntPtr nativePointer)
		{
			this.nativePointer = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~Cursor()
		{
			Dispose(false);
		}
		
		private void Dispose(bool disposing)
		{
			if (nativePointer != IntPtr.Zero)
			{
				cursorCache.UnregisterObject(nativePointer);
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
	}
}
