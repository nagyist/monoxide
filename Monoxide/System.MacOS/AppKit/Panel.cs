using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSPanel", "AppKit")]
	public class Panel : Window
	{
		public Panel()
		{
			DisposeWhenClosed = false;
			Style |= WindowStyle.Utility;
		}
	}
}
