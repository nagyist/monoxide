using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSTableColumn", "AppKit")]
	public class TableColumn<TCell> : ICloneable
		where TCell : Cell, new()
	{
		private Cell headerCell;
		private TCell dataCell;
		int width;
		int minWidth;
		int maxWidth;
		
		public TableColumn()
		{
		}
		
		public virtual TCell GetDataCell(int row)
		{
			return dataCell;
		}
		
		public void SizeToFit()
		{
		}
		
		public virtual object Clone()
		{
			var clone = MemberwiseClone() as TableColumn<TCell>;
			
			clone.headerCell = headerCell.Clone() as Cell;
			clone.dataCell = dataCell.Clone() as TCell;
			
			return clone;
		}
	}
}
