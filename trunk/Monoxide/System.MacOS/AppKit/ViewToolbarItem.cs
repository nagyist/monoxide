using System;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	public class ViewToolbarItem : ToolbarItem
	{
		private View view;
		
		public ViewToolbarItem() { }
		
		public ViewToolbarItem(string name)
			: base(name) { }
		
		internal override void OnCreated()
		{
			base.OnCreated();
			
			if (view != null)
				SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetView, view.NativePointer);
		}
		
		public View View
		{
			get { return view; }
			set
			{
				if (value != view)
				{
					if (value != null) value.Owner = this;
					if (view != null) view.Owner = null;
					
					view = value;

					if (view != null)
					{
						view.Measure(Size.Infinite);
						view.Arrange(new Rectangle(Point.Zero, Size.Infinite));
					}
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetView, view != null ? view.NativePointer : IntPtr.Zero);
				}
			}
		}
		
		public override object Clone()
		{
			var clone = base.Clone() as ViewToolbarItem;

			clone.view = null;

			if (view != null) clone.View = view.Clone() as View;
			
			return clone;
		}
	}
}
