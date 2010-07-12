using System;

namespace System.MacOS.AppKit
{
	[Flags]
	public enum PresentationOptions
	{
		Default = 0,
		AutoHideDock = 1,
		HideDock = 2,
		AutoHideMenuBar = 4,
		HideMenuBar = 8,
		NSApplicationPresentationDisableAppleMenu = 16,
		NSApplicationPresentationDisableProcessSwitching = 32,
		NSApplicationPresentationDisableForceQuit = 64,
		NSApplicationPresentationDisableSessionTermination = 128,
		NSApplicationPresentationDisableHideApplication = 256,
		NSApplicationPresentationDisableMenuBarTransparency = 512
	}
}
