using System;

namespace System.MacOS.AppKit
{
	public class OutlineView<TCell> : OutlineViewBase<TCell>
		where TCell : Cell, new()
	{
		public OutlineView()
		{
		}
	}
}
