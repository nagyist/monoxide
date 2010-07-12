using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSColorPanel", "AppKit")]
	public sealed class ColorPanel : Panel, ICommandItem
	{
		public ColorPanel()
		{
		}
		
		public Command Command { get; set; }
		public CommandTarget CommandTarget { get; set; }
		public object Tag { get; set; }
	}
}
