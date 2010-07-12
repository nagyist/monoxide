using System;

namespace System.MacOS.AppKit
{
	[Flags]
	public enum WindowStyle
	{
		Borderless = 0x0000,
		Titled = 0x0001,
		Closable = 0x0002,
		Miniaturizable = 0x0004,
		Resizable = 0x0008,
		TexturedBackground = 0x0100,
		Unscaled = 0x1000,
		UnifiedTitleAndToolbar = 0x4000,
		
		// Panel styles
		Utility = 0x0010,
		DocModal = 0x0040,
		NonActivating = 0x0080,
		Hud = 0x2000,
	}
}
