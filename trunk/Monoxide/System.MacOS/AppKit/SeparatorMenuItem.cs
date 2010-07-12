using System;

namespace System.MacOS.AppKit
{
	public class SeparatorMenuItem : MenuItem
	{
		public SeparatorMenuItem()
			: base(MenuItemKind.Separator) { }
		
		protected sealed override bool CanHaveMenuItems { get { return false; } }
	}
}
