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
				item.Owner = view;

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
		
		[SelectorStub("resizeSubviewsWithOldSize:")]
		static void ArrangeChildrenInternal(IntPtr self, IntPtr sel, Size oldSize)
		{
			var view = GetInstance(self);

			if (view != null) view.ArrangeChildren();
		}

		[SelectorStub("resizeWithOldSuperviewSize:")]
		static void AdjustSize(IntPtr self, IntPtr sel, Size size)
		{
			var view = GetInstance(self);

			if (view != null) view.Arrange();
		}
		
		[SelectorStub("setFrame:")]
		static void SetFrameInternal(IntPtr self, IntPtr sel, Rectangle frameRect)
		{
			var view = GetInstance(self);

			// While resizing, NSWindow will call this method on its content view.
			// Thus, when the owner is a window, this method is the best way to detect a frame resize.
			// Detecting such a situation is needed for making the managed layout engine coherent.
			if (view != null && !(view.owner is View))
			{
				view.margin = new Thickness(frameRect.Left, 0, 0, frameRect.Top);
				view.width = frameRect.Width;
				view.height = frameRect.Height;

				view.Arrange();
			}
		}
		
		#region Method Selector Ids

		#warning Don't forget to remove "View.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class initWithFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithFrame:"); }
			static class frame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("frame"); }
			static class setFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setFrame:"); }
			static class autoresizingMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("autoresizingMask"); }
			static class setAutoresizingMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAutoresizingMask:"); }
			static class addSubview { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("addSubview:"); }
			static class resizeSubviewsWithOldSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeSubviewsWithOldSize:"); }
			static class resizeWithOldSuperviewSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeWithOldSuperviewSize:"); }
			
			public static IntPtr InitWithFrame { get { return initWithFrame.SelectorHandle; } }
			public static IntPtr Frame { get { return View.Selectors.frame.SelectorHandle; } }
			public static IntPtr SetFrame { get { return setFrame.SelectorHandle; } }
			public static IntPtr AutoresizingMask { get { return autoresizingMask.SelectorHandle; } }
			public static IntPtr SetAutoresizingMask { get { return setAutoresizingMask.SelectorHandle; } }
			public static IntPtr AddSubview { get { return addSubview.SelectorHandle; } }
			public static IntPtr ResizeSubviewsWithOldSize { get { return resizeSubviewsWithOldSize.SelectorHandle; } }
			public static IntPtr ResizeWithOldSuperviewSize { get { return resizeWithOldSuperviewSize.SelectorHandle; } }
		}
		
		#endregion
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<View> viewCache = new NativeObjectCache<View>(v => v.NativePointer, null);
		
		internal static View GetInstance(IntPtr nativePointer) { return viewCache.GetObject(nativePointer); }
		
		#endregion

		private object owner;
		private List<View> viewList;
		private ViewCollection viewCollection;
		private Rectangle frame;
		private Thickness margin;
		private Thickness padding;
		private double width, minWidth, maxWidth;
		private double height, minHeight, maxHeight;
		private bool measureValid, arrangeValid;
		
		public View()
		{
			margin = new Thickness(0, 0, 0, 0);
			width = double.NaN;
			height = double.NaN;
			minWidth = 0;
			maxWidth = double.PositiveInfinity;
			minHeight = 0;
			maxHeight = double.PositiveInfinity;
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
			// Compute the frame rectangle if possible
			Arrange();

			using (var pool = LocalAutoReleasePool.Create())
			{
				var nativePointer = ObjectiveC.AllocObject(NativeClass);
				InitializeNative(ref nativePointer);
				super.Receiver = nativePointer;
				viewCache.RegisterObject(this);
				//SafeNativeMethods.objc_msgSend(nativePointer, Selectors.SetAutoresizingMask, (IntPtr)autoResizingMask);
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
			InitWithFrame(ref nativePointer, frame);
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
		
//		public LayoutOptions LayoutOptions
//		{
//			get
//			{
//				if (Created)
//					autoResizingMask = (LayoutOptions)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.AutoresizingMask);
//				
//				return autoResizingMask;
//			}
//			set
//			{
//				if (value != autoResizingMask)
//				{
//					autoResizingMask = value;
//					
//					if (Created)
//						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetAutoresizingMask, (IntPtr)autoResizingMask);
//				}
//			}
//		}
		
		#region Position & Dimensions
		
		internal Rectangle Frame
		{
			// No need to query the native object for the frame since we always set it via managed code.
			get { return frame; }
			set
			{
				if (value != frame)
				{
					frame = value;
					
					if (Created)
						if (ObjectiveC.LP64)
							SafeNativeMethods.objc_msgSendSuper_set_Rectangle_64(ref super, Selectors.SetFrame, frame);
						else
							SafeNativeMethods.objc_msgSendSuper_set_Rectangle_32(ref super, Selectors.SetFrame, frame);
				}
			}
		}
		
		public Size DesiredSize { get; private set; }
		
		public Thickness Margin
		{
			get { return margin; }
			set
			{
				margin = value;
			}
		}

		public Thickness Padding
		{
			get { return padding; }
			set
			{
				padding = value;
			}
		}
		
		public double Width
		{
			get { return width; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");
				width = value;
			}
		}
		
		public double MinWidth
		{
			get { return minWidth; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");
				minWidth = value;
			}
		}
		
		public double MaxWidth
		{
			get { return maxWidth; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");
				maxWidth = value;
			}
		}
		
		public double Height
		{
			get { return height; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");
				height = value;
			}
		}
		
		public double MinHeight
		{
			get { return minHeight; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");
				minHeight = value;
			}
		}
		
		public double MaxHeight
		{
			get { return maxHeight; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");
				maxHeight = value;
			}
		}
		
		#endregion

		// A layout engine similar to WPF is needed, but I need to find the best way to do that.
		// For now the layout is basically the layout of a WPF grid with only one cell.
		// Also, this seems to break the ScrollView and SplitView badly…

//		public bool IsMeasureValid { get { return measureValid; } }
//
//		public bool IsArrangeValid { get { return arrangeValid; } }
//
//		public void Measure(Size availableSize)
//		{
//			DesiredSize = availableSize;
//		}
//		
//		public virtual Size MeasureCore(Size availableSize)
//		{
//			return new Size(width, height);
//		}
//		
//		public virtual Size MeasureOverride(Size availableSize)
//		{
//			return new Size(width, height);
//		}
//		
//		public void Arrange(Rectangle finalRect)
//		{
//			Frame = finalRect;
//		}
//		
//		public Size ArrangeCore(Rectangle finalRect)
//		{
//		}
//		
//		public Size ArrangeOverride(Size finalSize)
//		{
//		}

		private void Arrange()
		{
			if (Parent != null)
			{
				var parentSize = Parent.Frame.Size;
				double x, y, w, h;

				// Process dimensions

				// Width
				if (double.IsNaN(width))
					if (double.IsNaN(margin.Left) || double.IsNaN(margin.Right))
						if (double.IsNaN(minWidth)) w = 0;
						else w = minWidth;
					else w = parentSize.Width - margin.Left - margin.Right;
				else w = width;
				if (w > maxWidth) w = maxWidth;
				if (w < minWidth) w = minWidth;
				if (w < 0) w = 0;

				// Height
				if (double.IsNaN(height))
					if (double.IsNaN(margin.Top) || double.IsNaN(margin.Bottom))
						if (double.IsNaN(minWidth)) h = 0;
						else h = minHeight;
					else h = parentSize.Height - margin.Top - margin.Bottom;
				else h = height;
				if (h > maxHeight) w = maxHeight;
				if (h < minHeight) w = minHeight;
				if (h < 0) w = 0;


				// Process position

				// Horizontal
				if (double.IsNaN(margin.Left))
					if (double.IsNaN(margin.Right)) x = (parentSize.Width - w) / 2;
					else x = parentSize.Width - margin.Right - w;
				else
					if (double.IsNaN(margin.Right)) x = margin.Left;
					else x = margin.Left + (parentSize.Width - (w + margin.Left + margin.Right)) / 2;

				// Vertical (Remember that y is going up in Cocoa…)
				if (double.IsNaN(margin.Bottom))
					if (double.IsNaN(margin.Top)) y = (parentSize.Height - h) / 2;
					else y = parentSize.Height - margin.Top - h;
				else
					if (double.IsNaN(margin.Top)) y = margin.Bottom;
					else y = margin.Bottom + (parentSize.Height - (h + margin.Top + margin.Bottom)) / 2;
	
				Frame = new Rectangle(x, y, w, h);
			}
			else
				Frame = new Rectangle(margin.Left, margin.Bottom, Width, Height);
		}

		private void ArrangeChildren()
		{
			foreach (var child in viewList)
				child.Arrange();
		}

		public object Owner
		{
			get { return owner; }
			internal set
			{
				if (value != null && owner != null)
					throw new InvalidOperationException(Localization.GetExceptionText(""));
				if (value == null && owner == null)
					throw new InvalidOperationException();
				owner = value;
			}
		}

		public View Parent { get { return owner as View; } }
		
		public ViewCollection Children { get { return viewCollection; } }
		
		public virtual object Clone()
		{
			var clone = MemberwiseClone() as View;

			clone.super.Receiver = IntPtr.Zero;
			clone.owner = null;
			clone.viewList = new List<View>(viewList.Count);
			clone.viewCollection = new ViewCollection(clone);
			
			foreach (var child in viewList)
			{
				var childClone = child.Clone() as View;

				childClone.Owner = clone;

				clone.viewCollection.Add(childClone);
			}
			
			return clone;
		}
	}
}
