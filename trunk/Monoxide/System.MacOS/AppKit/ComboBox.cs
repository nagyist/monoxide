using System;

namespace System.MacOS.AppKit
{
	public sealed class ComboBox : ComboBox<ComboBoxCell> { }
	
	[NativeClass("NSComboBox", "AppKit")]
	[DefaultCellType(typeof(ComboBoxCell))]
	public class ComboBox<TCell> : TextField<TCell>
		where TCell : ComboBoxCell, new()
	{
	}
}
