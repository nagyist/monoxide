using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	[NativeClass("NSView", "AppKit")]
	public class View : EventTarget, ICloneable
	{
		#region ViewCollection Class
		
		public sealed class ViewCollection : ICollection<View>
		{
			private readonly View view;
			
			internal ViewCollection(View view) { this.view = view; }
			
			#region ICollection<View> Implementation
			
			public void Add(View item)
			{
				view.viewList.Add(item);
				if (view.Created)
					SafeNativeMethods.objc_msgSend(view.NativePointer, Selectors.AddSubview, item.NativePointer);
			}
			
			public void Clear()
			{
				throw new System.NotImplementedException();
			}
			
			public bool Contains(View item)
			{
				throw new System.NotImplementedException();
			}
			
			public void CopyTo(View[] array, int arrayIndex)
			{
				throw new System.NotImplementedException();
			}
			
			public bool Remove(View item)
			{
				throw new System.NotImplementedException();
			}
			
			public int Count { get { return view.viewList.Count; } }
			
			public bool IsReadOnly { get { return false; } }
			
			#endregion
			
			#region IEnumerable Implementations
			
			public List<View>.Enumerator GetEnumerator() { return view.viewList.GetEnumerator(); }
			
			IEnumerator<View> IEnumerable<View>.GetEnumerator() { return view.viewList.GetEnumerator(); }
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return view.viewList.GetEnumerator(); }
			
			#endregion
		}
		
		#endregion
		
		#region NSView Interop
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithFrame_32(IntPtr self, IntPtr sel, RectangleF frame);
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithFrame_64(IntPtr self, IntPtr sel, Rectangle frame);
		
		#region Method Selector Ids
		
		static class Selectors
		{
			static class initWithFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithFrame:"); }
			static class frame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("frame"); }
			static class setFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setFrame:"); }
			static class autoresizingMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("autoresizingMask"); }
			static class setAutoresizingMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAutoresizingMask:"); }
			static class addSubview { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("addSubview:"); }
			
			public static IntPtr InitWithFrame { get { return initWithFrame.SelectorHandle; } }
			public static IntPtr Frame { get { return frame.SelectorHandle; } }
			public static IntPtr SetFrame { get { return setFrame.SelectorHandle; } }
			public static IntPtr AutoresizingMask { get { return autoresizingMask.SelectorHandle; } }
			public static IntPtr SetAutoresizingMask { get { return setAutoresizingMask.SelectorHandle; } }
			public static IntPtr AddSubview { get { return addSubview.SelectorHandle; } }
		}
		
		#endregion
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<View> viewCache = new NativeObjectCache<View>(v => v.NativePointer, null);
		
		internal static View GetInstance(IntPtr nativePointer) { return viewCache.GetObject(nativePointer); }
		
		#endregion
		
		private List<View> viewList;
		private ViewCollection viewCollection;
		private LayoutOptions autoResizingMask;
		private Rectangle bounds;
		
		public View()
		{
			bounds = new Rectangle(0, 0, 100, 100);
			viewList = new List<View>();
			viewCollection = new ViewCollection(this);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				viewCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
		}
		
		private void CreateNative()
		{
			using (var pool = LocalAutoReleasePool.Create())
			{
				var nativePointer = ObjectiveC.AllocObject(NativeClass);
				InitializeNative(ref nativePointer);
				super.Receiver = nativePointer;
				viewCache.RegisterObject(this);
				SafeNativeMethods.objc_msgSend(nativePointer, Selectors.SetAutoresizingMask, (IntPtr)autoResizingMask);
				try { OnCreated(); }
				catch // A fault handler would have been better…
				{
					viewCache.UnregisterObject(nativePointer);
					ObjectiveC.ReleaseObject(nativePointer);
					super.Receiver = IntPtr.Zero;
					throw;
				}
				foreach (var view in viewList)
					SafeNativeMethods.objc_msgSend(nativePointer, Selectors.AddSubview, view.NativePointer);
			}
		}
		
		internal virtual void InitializeNative(ref IntPtr nativePointer)
		{
			InitWithFrame(ref nativePointer, bounds);
		}
		
		internal virtual void OnCreated() { }
		
		private static void InitWithFrame(ref IntPtr nativePointer, Rectangle bounds)
		{
			nativePointer = ObjectiveC.LP64 ?
				objc_msgSend_initWithFrame_64(nativePointer, Selectors.InitWithFrame, bounds) :
				objc_msgSend_initWithFrame_32(nativePointer, Selectors.InitWithFrame, bounds);
		}
		
		internal override IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(GetType().Name);
				
				if (!Created)
					CreateNative();
				
				return super.Receiver;
			}
		}
		
		public LayoutOptions LayoutOptions
		{
			get
			{
				if (Created)
					autoResizingMask = (LayoutOptions)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.AutoresizingMask);
				
				return autoResizingMask;
			}
			set
			{
				if (value != autoResizingMask)
				{
					autoResizingMask = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetAutoresizingMask, (IntPtr)autoResizingMask);
				}
			}
		}
		
		public virtual Rectangle Bounds
		{
			get
			{
				if (Created)
					if (ObjectiveC.LP64)
						bounds = SafeNativeMethods.objc_msgSend_get_Rectangle_64(NativePointer, Selectors.Frame);
					else
						bounds = SafeNativeMethods.objc_msgSend_get_Rectangle_32(NativePointer, Selectors.Frame);
				
				return bounds;
			}
			set
			{
				bounds = value;
				
				if (Created)
					if (ObjectiveC.LP64)
						SafeNativeMethods.objc_msgSend_set_Rectangle_64(NativePointer, Selectors.Frame, bounds);
					else
						SafeNativeMethods.objc_msgSend_set_Rectangle_32(NativePointer, Selectors.Frame, bounds);
			}
		}
		
		public ViewCollection Children { get { return viewCollection; } }
		
		public virtual object Clone()
		{
			var clone = MemberwiseClone() as View;
			
			clone.super.Receiver = IntPtr.Zero;
			clone.viewList = new List<View>(viewList.Count);
			clone.viewCollection = new ViewCollection(clone);
			
			foreach (var view in viewList)
				clone.viewList.Add(view.Clone() as View);
			
			return clone;
		}
	}
}
