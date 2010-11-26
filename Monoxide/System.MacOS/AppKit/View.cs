using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.MacOS.CoreGraphics;

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

			public void AddRange(params View[] items)
			{
				foreach (var item in items)
					Add(item);
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
		
		#region Method Selector Ids

		#warning Don't forget to remove "View.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class initWithFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithFrame:"); }
			//static class frame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("frame"); }
			static class setFrame { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setFrame:"); }
			static class setFrameOrigin { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setFrameOrigin:"); }
			static class setFrameSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setFrameSize:"); }
			static class autoresizesSubviews { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("autoresizesSubviews"); }
			static class setAutoresizesSubviews { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAutoresizesSubviews:"); }
			//static class autoresizingMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("autoresizingMask"); }
			static class setAutoresizingMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAutoresizingMask:"); }
			static class addSubview { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("addSubview:"); }
			static class resizeSubviewsWithOldSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeSubviewsWithOldSize:"); }
			static class resizeWithOldSuperviewSize { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("resizeWithOldSuperviewSize:"); }
			static class isFlipped { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isFlipped"); }
			static class isOpaque { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isOpaque"); }
			static class drawRect { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("drawRect:"); }
			
			public static IntPtr InitWithFrame { get { return initWithFrame.SelectorHandle; } }
			//public static IntPtr Frame { get { return View.Selectors.frame.SelectorHandle; } }
			public static IntPtr SetFrame { get { return setFrame.SelectorHandle; } }
			public static IntPtr SetFrameOrigin { get { return setFrameOrigin.SelectorHandle; } }
			public static IntPtr SetFrameSize { get { return setFrameSize.SelectorHandle; } }
			public static IntPtr AutoresizesSubviews { get { return autoresizesSubviews.SelectorHandle; } }
			public static IntPtr SetAutoresizesSubviews { get { return setAutoresizesSubviews.SelectorHandle; } }
			//public static IntPtr AutoresizingMask { get { return autoresizingMask.SelectorHandle; } }
			public static IntPtr SetAutoresizingMask { get { return setAutoresizingMask.SelectorHandle; } }
			public static IntPtr AddSubview { get { return addSubview.SelectorHandle; } }
			public static IntPtr ResizeSubviewsWithOldSize { get { return resizeSubviewsWithOldSize.SelectorHandle; } }
			public static IntPtr ResizeWithOldSuperviewSize { get { return resizeWithOldSuperviewSize.SelectorHandle; } }
			public static IntPtr IsFlipped { get { return isFlipped.SelectorHandle; } }
			public static IntPtr IsOpaque { get { return isOpaque.SelectorHandle; } }
			public static IntPtr DrawRect { get { return drawRect.SelectorHandle; } }
		}
		
		#endregion
		
		#region NSView Interop
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithFrame_32(IntPtr self, IntPtr sel, RectangleF frame);
		[SuppressUnmanagedCodeSecurity]
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		static extern IntPtr objc_msgSend_initWithFrame_64(IntPtr self, IntPtr sel, Rectangle frame);
		
//		[SelectorStub("resizeSubviewsWithOldSize:")]
//		static void ResizeSubviewsWithOldSize(IntPtr self, IntPtr sel, Size oldSize)
//		{
//			var view = GetInstance(self);
//
//			if (ObjectiveC.LP64)
//				SafeNativeMethods.objc_msgSendSuper_set_Size_64(ref view.super, Selectors.ResizeSubviewsWithOldSize, oldSize);
//			else
//				SafeNativeMethods.objc_msgSendSuper_set_Size_32(ref view.super, Selectors.ResizeSubviewsWithOldSize, oldSize);
//		}

		[SelectorStub("resizeWithOldSuperviewSize:")]
		static void ResizeWithOldSuperviewSize(IntPtr self, IntPtr sel, Size size)
		{
			var view = GetInstance(self);

			view.ApplyFrame();
		}
		
		[SelectorStub("setFrame:")]
		static void SetFrameInternal(IntPtr self, IntPtr sel, Rectangle frameRect)
		{
			var view = GetInstance(self);

			// Windows, Toolbars and such all have internal views for placing our more regular views.
			// These views can be accessed, but since it's part of the private interface, it is better not to.
			// Here, we can track changes to the frame when the view is used outside of the public view hierarchy.
			// Anyway, the goal is to manage [almost] all of the layout in managed code, so this implementation
			// effectively helps in ignoring external (non-managed) changes to the frame.
			if (!(view.owner is View))
			{
				view.margin = new Thickness(frameRect.Left, frameRect.Top, frameRect.Left, frameRect.Top);
				view.Measure(frameRect.Size);
				view.Arrange(frameRect);
				view.ApplyFrame();
			}
			else Debug.WriteLine("Ignored NSView -setFrame: for View of type {0}", view.GetType());
		}

		[SelectorStub("setFrameSize:")]
		static void SetFrameSizeInternal(IntPtr self, IntPtr sel, Size frameSize)
		{
			var view = GetInstance(self);

			// While resizing, NSWindow will call this method on its content view.
			// Thus, when the owner is a window, this method is the best way to detect a frame resize.
			// Detecting such a situation is needed for making the managed layout engine coherent.
			// (Note that just in case, the code for setFrame will also work correctly if it was ever to be called in place of setFrameSize)
			if (view.Owner is Window)
			{
				view.Measure(frameSize);
				view.Arrange(new Rectangle(view.frame.Location, frameSize));
				//view.ApplyFrame();
			}
			// setFrameSize: will be called internally by setFrame: (as it seems), so we need to call the base implementation
			if (ObjectiveC.LP64)
				SafeNativeMethods.objc_msgSendSuper_set_Size_64(ref view.super, sel, frameSize);
			else
				SafeNativeMethods.objc_msgSendSuper_set_Size_32(ref view.super, sel, frameSize);
		}

		[SelectorStub("isFlipped")]
		static bool IsFlippedInternal(IntPtr self, IntPtr sel) { return true; } // All System.MacOS views will have flipped coordinate systems

		[SelectorStub("isOpaque", Kind = StubKind.InstanceLazy)]
		static bool IsOpaqueInternal(IntPtr self, IntPtr sel)
		{
			var view = GetInstance(self);

			return view.IsOpaque;
		}

		[SelectorStub("drawRect:", Kind = StubKind.InstanceLazy)]
		static void DrawRectangleInternal(IntPtr self, IntPtr sel, Rectangle dirtyRect)
		{
			var view = GetInstance(self);

			view.DrawRectangle(GraphicsContext.Current, dirtyRect);
		}
		
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
		private HorizontalAlignment horizontalAlignment;
		private VerticalAlignment verticalAlignment;
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
			horizontalAlignment = HorizontalAlignment.Stretch;
			verticalAlignment = VerticalAlignment.Stretch;
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
				InitializeNative(ref nativePointer, frame);
				super.Receiver = nativePointer;
				viewCache.RegisterObject(this);
				SafeNativeMethods.objc_msgSend(nativePointer, Selectors.SetAutoresizingMask, (IntPtr)63);
				SafeNativeMethods.objc_msgSend_set_Boolean(nativePointer, Selectors.SetAutoresizesSubviews, true);
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
		
		internal virtual void InitializeNative(ref IntPtr nativePointer, Rectangle frame)
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
		
		#region Position & Dimensions
		
		private Rectangle Frame
		{
			// No need to query the native object for the frame since we always set it via managed code.
			get { return frame; }
			set { frame = value; }
		}

		private void ApplyFrame()
		{
			if (Created)
				if (ObjectiveC.LP64)
					SafeNativeMethods.objc_msgSendSuper_set_Rectangle_64(ref super, Selectors.SetFrame, frame);
				else
					SafeNativeMethods.objc_msgSendSuper_set_Rectangle_32(ref super, Selectors.SetFrame, frame);
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

		public HorizontalAlignment HorizontalAlignment
		{
			get { return horizontalAlignment; }
			set
			{
				horizontalAlignment = value;
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return verticalAlignment; }
			set
			{
				verticalAlignment = value;
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
				if (value < 0 || value > maxWidth || double.IsPositiveInfinity(value))
					throw new ArgumentOutOfRangeException("value");
				minWidth = value;
			}
		}
		
		public double MaxWidth
		{
			get { return maxWidth; }
			set
			{
				if (value < 0 || value < minWidth)
					throw new ArgumentOutOfRangeException("value");
				maxWidth = value;
			}
		}

		public double ActualWidth { get { return frame.Width; } }
		
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

		public double ActualHeight { get { return frame.Height; } }
		
		#endregion

		// A layout engine similar to WPF is needed, but I need to find the best way to do that.
		// For now the layout is basically the layout of a WPF grid with only one cell.
		// Also, this seems to break the ScrollView and SplitView badly…

		public bool IsMeasureValid { get { return measureValid; } }

		public bool IsArrangeValid { get { return arrangeValid; } }

		public void Measure(Size availableSize)
		{
			var constraintSize = availableSize - margin; // Subtract the margin from available size

			DesiredSize = MeasureOverride(constraintSize) + margin; // Measure the view and add the margin back
		}

		public virtual Size MeasureOverride(Size availableSize)
		{
			double w, h;

			// Width
			if (double.IsNaN(width))
				if (double.IsPositiveInfinity(availableSize.Width))
					if (double.IsNaN(minWidth)) w = 0;
					else w = minWidth;
				else w = availableSize.Width;
			else w = width;
			if (w > maxWidth) w = maxWidth;
			if (w < minWidth) w = minWidth;
			if (w < 0) w = 0;

			// Height
			if (double.IsNaN(height))
				if (double.IsPositiveInfinity(availableSize.Height))
					if (double.IsNaN(minWidth)) h = 0;
					else h = minHeight;
				else h = availableSize.Height;
			else h = height;
			if (h > maxHeight) w = maxHeight;
			if (h < minHeight) w = minHeight;
			if (h < 0) w = 0;

			// Children size adjustment
			foreach (var child in Children)
			{
				child.Measure(availableSize);

				var desiredSize = child.DesiredSize;

				if (desiredSize.Width > w) w = desiredSize.Width;
				if (desiredSize.Height > h) h = desiredSize.Height;
			}

			return new Size(w, h);
		}

		public void Arrange(Rectangle finalRect)
		{
			var constraintRect = finalRect - margin;

			var arrangeSize = ArrangeOverride(constraintRect.Size);

			// Horizontal
			if (constraintRect.Size.Width >= arrangeSize.Width)
			{
				switch ((int)horizontalAlignment)
				{
					case (int)HorizontalAlignment.Left:
						finalRect.Left = constraintRect.Left;
						finalRect.Width = arrangeSize.Width;
						break;
					case (int)HorizontalAlignment.Center:
						finalRect.Left += (finalRect.Width - (margin.Left + arrangeSize.Width + margin.Right)) / 2;
						finalRect.Width = arrangeSize.Width;
						break;
					case (int)HorizontalAlignment.Right:
						finalRect.Left += finalRect.Width - arrangeSize.Width - margin.Right;
						finalRect.Width = arrangeSize.Width;
						break;
					case (int)HorizontalAlignment.Stretch:
						if (!double.IsNaN(width)) // Width has precedence over stretch
							goto case (int)HorizontalAlignment.Center;

						finalRect.Left = constraintRect.Left;
						finalRect.Width = constraintRect.Width;
						break;
				}
			}
			if (double.IsPositiveInfinity(finalRect.Left)) finalRect.Left = 0;

			// Vertical (for flipped coordinates)
			if (constraintRect.Size.Height >= arrangeSize.Height)
			{
				switch ((int)verticalAlignment)
				{
					case (int)VerticalAlignment.Top:
						finalRect.Top = constraintRect.Top;
						finalRect.Height = arrangeSize.Height;
						break;
					case (int)VerticalAlignment.Center:
						finalRect.Top += (finalRect.Height - (margin.Top + arrangeSize.Height + margin.Bottom)) / 2;
						finalRect.Height = arrangeSize.Height;
						break;
					case (int)VerticalAlignment.Bottom:
						finalRect.Top += finalRect.Height - arrangeSize.Height - margin.Bottom;
						finalRect.Height = arrangeSize.Height;
						break;
					case (int)VerticalAlignment.Stretch:
						if (!double.IsNaN(height)) // Width has precedence over stretch
							goto case (int)VerticalAlignment.Center;

						finalRect.Top = constraintRect.Top;
						finalRect.Height = constraintRect.Height;
						break;
				}
			}
			if (double.IsPositiveInfinity(finalRect.Top)) finalRect.Top = 0;

			Frame = finalRect;
		}
		
		public virtual Size ArrangeOverride(Size finalSize)
		{
			finalSize = DesiredSize - margin;

			foreach (var child in Children)
				child.Arrange(new Rectangle(Point.Zero, finalSize));

			return finalSize;
		}

//		private void Arrange()
//		{
//			if (Parent != null)
//			{
//				var parentSize = Parent.Frame.Size;
//				double x, y, w, h;
//
//				// Process dimensions
//
//				// Width
//				if (double.IsNaN(width))
//					if (double.IsNaN(margin.Left) || double.IsNaN(margin.Right))
//						if (double.IsNaN(minWidth)) w = 0;
//						else w = minWidth;
//					else w = parentSize.Width - margin.Left - margin.Right;
//				else w = width;
//				if (w > maxWidth) w = maxWidth;
//				if (w < minWidth) w = minWidth;
//				if (w < 0) w = 0;
//
//				// Height
//				if (double.IsNaN(height))
//					if (double.IsNaN(margin.Top) || double.IsNaN(margin.Bottom))
//						if (double.IsNaN(minWidth)) h = 0;
//						else h = minHeight;
//					else h = parentSize.Height - margin.Top - margin.Bottom;
//				else h = height;
//				if (h > maxHeight) w = maxHeight;
//				if (h < minHeight) w = minHeight;
//				if (h < 0) w = 0;
//
//
//				// Process position
//
//				// Horizontal
//				if (double.IsNaN(margin.Left))
//					if (double.IsNaN(margin.Right)) x = (parentSize.Width - w) / 2;
//					else x = parentSize.Width - margin.Right - w;
//				else
//					if (double.IsNaN(margin.Right)) x = margin.Left;
//					else x = margin.Left + (parentSize.Width - (w + margin.Left + margin.Right)) / 2;
//
////				// Vertical (Remember that y is going up in Cocoa…)
////				if (double.IsNaN(margin.Bottom))
////					if (double.IsNaN(margin.Top)) y = (parentSize.Height - h) / 2;
////					else y = parentSize.Height - margin.Top - h;
////				else
////					if (double.IsNaN(margin.Top)) y = margin.Bottom;
////					else y = margin.Bottom + (parentSize.Height - (h + margin.Top + margin.Bottom)) / 2;
//				// Vertical (for flipped coordinates)
//				if (double.IsNaN(margin.Top))
//					if (double.IsNaN(margin.Bottom)) y = (parentSize.Height - h) / 2;
//					else y = parentSize.Height - margin.Bottom - h;
//				else
//					if (double.IsNaN(margin.Bottom)) y = margin.Top;
//					else y = margin.Top + (parentSize.Height - (h + margin.Top + margin.Bottom)) / 2;
//	
//				Frame = new Rectangle(x, y, w, h);
//			}
//			else
//				Frame = new Rectangle(margin.Left, margin.Bottom, Width, Height);
//		}
//
//		private void ArrangeChildren()
//		{
//			foreach (var child in viewList)
//				child.Arrange();
//		}

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

		public virtual bool IsOpaque
		{
			[SelectorImplementation("isOpaque")]
			get { return Created ? SafeNativeMethods.objc_msgSendSuper_get_Boolean(ref super, Selectors.IsOpaque) : false; }
		}

		[SelectorImplementation("drawRect:")]
		protected virtual void DrawRectangle(GraphicsContext context, Rectangle bounds)
		{
			if (ObjectiveC.LP64)
				SafeNativeMethods.objc_msgSendSuper_set_Rectangle_64(ref super, Selectors.DrawRect, bounds);
			else
				SafeNativeMethods.objc_msgSendSuper_set_Rectangle_32(ref super, Selectors.DrawRect, bounds);
		}
		
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
