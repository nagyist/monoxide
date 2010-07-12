using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSTableHeaderCell", "AppKit")]
	public class TableHeaderCell : TextFieldCell
	{
		public TableHeaderCell()
		{
		}
		
		protected virtual Rectangle GetSortIndicatorBounds(Rectangle bounds)
		{
			return Rectangle.Zero;
		}
		
		protected virtual void DrawSortIndicator(Rectangle bounds, View view, bool ascending, int priority)
		{
		}
	}
}
