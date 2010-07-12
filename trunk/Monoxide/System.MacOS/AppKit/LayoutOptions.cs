using System;

namespace System.MacOS.AppKit
{
	[Flags]
	public enum LayoutOptions
	{
		Fixed = 0,
		Left = 1,
		Width = 2,
		Right = 4,
		Bottom = 8,
		Height = 16,
		Top = 32
	}
}
