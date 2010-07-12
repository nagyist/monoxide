using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSBrowser", "AppKit")]
	[DefaultCellType(typeof(BrowserCell))]
	public class Browser<TCell> : Control<TCell>
		where TCell : BrowserCell, new()
	{
	}
}
