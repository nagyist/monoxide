using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSSplitView", "AppKit")]
	public class SplitView : View
	{
		#region Method Selector Ids

		#warning Don't forget to remove "SplitView.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class isVertical { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isVertical"); }
			static class setVertical { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setVertical:"); }
			static class dividerStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dividerStyle"); }
			static class setDividerStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDividerStyle:"); }
			static class dividerThickness { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dividerThickness"); }
			static class setDividerThickness { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDividerThickness:"); }
			
			public static IntPtr IsVertical { get { return isVertical.SelectorHandle; } }
			public static IntPtr SetVertical { get { return setVertical.SelectorHandle; } }
			public static IntPtr DividerStyle { get { return SplitView.Selectors.dividerStyle.SelectorHandle; } }
			public static IntPtr SetDividerStyle { get { return setDividerStyle.SelectorHandle; } }
			public static IntPtr DividerThickness { get { return dividerThickness.SelectorHandle; } }
			public static IntPtr SetDividerThickness { get { return setDividerThickness.SelectorHandle; } }
		}
		
		#endregion

		DividerStyle dividerStyle;
		bool verticalSplitBars;
		
		public SplitView()
		{
			dividerStyle = DividerStyle.Thick;
		}
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetDividerStyle, (IntPtr)dividerStyle);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetVertical, verticalSplitBars);
		}
		
		public DividerStyle DividerStyle
		{
			get
			{
				if (Created)
					dividerStyle = (DividerStyle)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.DividerStyle);
				
				return dividerStyle;
			}
			set
			{
				if (!Enum.IsDefined(typeof(DividerStyle), value))
					throw new ArgumentOutOfRangeException("value");
				
				if (value != dividerStyle)
				{
					dividerStyle = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetDividerStyle, (IntPtr)dividerStyle);
				}
			}
		}
		
		public Orientation Orientation
		{
			get
			{
				if (Created)
					verticalSplitBars = SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, Selectors.IsVertical);
				
				return verticalSplitBars ? Orientation.Horizontal : Orientation.Vertical;
			}
			set
			{
				if (value != Orientation.Horizontal && value != Orientation.Vertical)
					throw new ArgumentOutOfRangeException("value");
				
				if ((value == Orientation.Horizontal) != verticalSplitBars)
				{
					verticalSplitBars = value == Orientation.Horizontal;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetVertical, verticalSplitBars);
				}
			}
		}
	}
}
