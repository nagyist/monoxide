using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSFontPanel", "AppKit")]
	public sealed class FontPanel : Panel, ICommandItem
	{
		public FontPanel()
		{
		}
		
		public Command Command { get; set; }
		public CommandTarget CommandTarget { get; set; }
		public object Tag { get; set; }
	}
}
