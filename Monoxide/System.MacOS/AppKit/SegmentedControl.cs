using System;

namespace System.MacOS.AppKit
{
	public sealed class SegmentedControl : SegmentedControl<SegmentedCell> { }
	
	// TODO: Implement Actions
	[NativeClass("NSSegmentedControl", "AppKit")]
	[DefaultCellType(typeof(SegmentedCell))]
	public class SegmentedControl<TCell> : Control<TCell>
		where TCell : SegmentedCell, new()
	{
		public SegmentedControl()
		{
			Bounds = new Rectangle(0, 0, 60, 24);
		}
		
		public SegmentedCell.SegmentCollection Segments { get { return Cell.Segments; } }
		
		public SegmentStyle Style
		{
			get { return Cell.Style; }
			set { Cell.Style = value; }
		}
		
		public ItemSelectionMode SelectionMode
		{
			get { return Cell.SelectionMode; }
			set { Cell.SelectionMode = value; }
		}
	}
}
