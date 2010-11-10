using System;

namespace System.MacOS.AppKit
{
	public sealed class SearchField : SearchField<SearchFieldCell> { }
	
	[NativeClass("NSSearchField", "AppKit")]
	[DefaultCellType(typeof(SearchFieldCell))]
	public class SearchField<TCell> : TextField<TCell>
		where TCell : SearchFieldCell, new()
	{
		public SearchField()
		{
			Width = 200;
			Height = 22;
		}
	}
}
