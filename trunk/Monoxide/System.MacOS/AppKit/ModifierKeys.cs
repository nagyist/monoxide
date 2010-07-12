using System;

namespace System.MacOS.AppKit
{
	[Flags]
	public enum ModifierKeys
	{
		None = 0,
		AlphaShift = 0x010000,
		Shift = 0x020000,
		Control = 0x040000,
		Alternate = 0x080000,
		Command = 0x100000,
		NumericPad = 0x200000,
		Help = 0x400000,
		Function = 0x800000
	}
}
