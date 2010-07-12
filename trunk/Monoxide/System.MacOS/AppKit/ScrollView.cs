using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSScrollView", "AppKit")]
	public class ScrollView : View
	{
		#region Method Selector Ids
		
		static class Selectors
		{
			static class documentView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("documentView"); }
			static class setDocumentView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDocumentView:"); }
			
			static class horizontalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("horizontalScroller"); }
			static class setHorizontalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHorizontalScroller:"); }
			static class hasHorizontalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("hasHorizontalScroller"); }
			static class setHasHorizontalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHasHorizontalScroller:"); }
			static class verticalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("verticalScroller"); }
			static class setVerticalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setVerticalScroller:"); }
			static class hasVerticalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("hasVerticalScroller"); }
			static class setHasVerticalScroller { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHasVerticalScroller:"); }
			static class autohidesScrollers { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("autohidesScrollers"); }
			static class setAutohidesScrollers { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAutohidesScrollers:"); }
			
			static class horizontalRulerView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("horizontalRulerView"); }
			static class setHorizontalRulerView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHorizontalRulerView:"); }
			static class hasHorizontalRuler { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("hasHorizontalRuler"); }
			static class setHasHorizontalRuler { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHasHorizontalRuler:"); }
			static class verticalRulerView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("verticalRuler"); }
			static class setVerticalRulerView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setVerticalRuler:"); }
			static class hasVerticalRuler { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("hasVerticalRuler"); }
			static class setHasVerticalRuler { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHasVerticalRuler:"); }
			static class rulersVisible { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("rulersVisible"); }
			static class setRulersVisible { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setRulersVisible:"); }
			
			public static IntPtr DocumentView { get { return documentView.SelectorHandle; } }
			public static IntPtr SetDocumentView { get { return setDocumentView.SelectorHandle; } }
			
			public static IntPtr HorizontalScroller { get { return horizontalScroller.SelectorHandle; } }
			public static IntPtr SetHorizontalScroller { get { return setHorizontalScroller.SelectorHandle; } }
			public static IntPtr HasHorizontalScroller { get { return hasHorizontalScroller.SelectorHandle; } }
			public static IntPtr SetHasHorizontalScroller { get { return setHasHorizontalScroller.SelectorHandle; } }
			public static IntPtr VerticalScroller { get { return verticalScroller.SelectorHandle; } }
			public static IntPtr SetVerticalScroller { get { return setVerticalScroller.SelectorHandle; } }
			public static IntPtr HasVerticalScroller { get { return hasVerticalScroller.SelectorHandle; } }
			public static IntPtr SetHasVerticalScroller { get { return setHasVerticalScroller.SelectorHandle; } }
			public static IntPtr AutohidesScrollers { get { return autohidesScrollers.SelectorHandle; } }
			public static IntPtr SetAutohidesScrollers { get { return setAutohidesScrollers.SelectorHandle; } }
			
			public static IntPtr HorizontalRulerView { get { return horizontalRulerView.SelectorHandle; } }
			public static IntPtr SetHorizontalRulerView { get { return setHorizontalRulerView.SelectorHandle; } }
			public static IntPtr HasHorizontalRuler { get { return hasHorizontalRuler.SelectorHandle; } }
			public static IntPtr SetHasHorizontalRuler { get { return setHasHorizontalRuler.SelectorHandle; } }
			public static IntPtr VerticalRulerView { get { return verticalRulerView.SelectorHandle; } }
			public static IntPtr SetVerticalRulerView { get { return setVerticalRulerView.SelectorHandle; } }
			public static IntPtr HasVerticalRuler { get { return hasVerticalRuler.SelectorHandle; } }
			public static IntPtr SetHasVerticalRuler { get { return setHasVerticalRuler.SelectorHandle; } }
			public static IntPtr RulersVisible { get { return rulersVisible.SelectorHandle; } }
			public static IntPtr SetRulersVisible { get { return setRulersVisible.SelectorHandle; } }
		}
		
		#endregion
		
		View documentView;
		Axis scrollers;
		Axis rulers;
		bool autoHidesScrollers;
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetDocumentView, documentView != null ? documentView.NativePointer : IntPtr.Zero);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetHasHorizontalScroller, (scrollers | Axis.Horizontal) != Axis.None);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetHasVerticalScroller, (scrollers | Axis.Vertical) != Axis.None);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetAutohidesScrollers, true);
		}
		
		public View DocumentView
		{
			get { return documentView; }
			set
			{
				if (value != documentView)
				{
					documentView = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetDocumentView, documentView != null ? documentView.NativePointer : IntPtr.Zero);
				}
			}
		}
		
		public Axis Scrollers
		{
			get
			{
				if (Created)
				{
					scrollers = Axis.None;
					if (SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, Selectors.HasHorizontalScroller))
						scrollers |= Axis.Horizontal;
					if (SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, Selectors.HasVerticalScroller))
						scrollers |= Axis.Vertical;
				}
				
				return scrollers;
			}
			set
			{
				if (value != Axis.None && value != Axis.Horizontal && value != Axis.Vertical && value != Axis.Both)
					throw new ArgumentOutOfRangeException("value");
				
				if (value != scrollers)
				{
					scrollers = value;
					
					if (Created)
					{
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetHasHorizontalScroller, (scrollers | Axis.Horizontal) != Axis.None);
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetHasVerticalScroller, (scrollers | Axis.Vertical) != Axis.None);
					}
				}
			}
		}
	}
}
