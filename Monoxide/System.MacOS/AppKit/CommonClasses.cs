using System;

namespace System.MacOS.AppKit
{
	internal static class CommonClasses
	{
		private static IntPtr GetClass(string name)
		{
			ObjectiveC.EnsureAppKitFrameworkIsLoaded();
			
			return ObjectiveC.GetClass(name);
		}
		
		private static class _NSBundle { public static readonly IntPtr ClassHandle = GetClass("NSBundle"); }
		private static class _NSImage { public static readonly IntPtr ClassHandle = GetClass("NSImage"); }
		private static class _NSCursor { public static readonly IntPtr ClassHandle = GetClass("NSCursor"); }
		private static class _NSAlert { public static readonly IntPtr ClassHandle = GetClass("NSAlert"); }
		private static class _NSMenu { public static readonly IntPtr ClassHandle = GetClass("NSMenu"); }
		private static class _NSMenuItem { public static readonly IntPtr ClassHandle = GetClass("NSMenuItem"); }
		private static class _NSWindow { public static readonly IntPtr ClassHandle = GetClass("NSWindow"); }
		private static class _NSToolbar { public static readonly IntPtr ClassHandle = GetClass("NSToolbar"); }
		private static class _NSToolbarItem { public static readonly IntPtr ClassHandle = GetClass("NSToolbarItem"); }
		private static class _NSCell { public static readonly IntPtr ClassHandle = GetClass("NSCell"); }
		private static class _NSActionCell { public static readonly IntPtr ClassHandle = GetClass("NSActionCell"); }
		private static class _NSView { public static readonly IntPtr ClassHandle = GetClass("NSView"); }
		private static class _NSOpenGLView { public static readonly IntPtr ClassHandle = GetClass("NSOpenGLView"); }
		private static class _NSControl { public static readonly IntPtr ClassHandle = GetClass("NSControl"); }
		private static class _NSButton { public static readonly IntPtr ClassHandle = GetClass("NSButton"); }
		private static class _NSTableView { public static readonly IntPtr ClassHandle = GetClass("NSTableView"); }
		private static class _NSOutlineView { public static readonly IntPtr ClassHandle = GetClass("NSOutlineView"); }
		
		public static IntPtr NSBundle { get { return _NSBundle.ClassHandle; } }
		public static IntPtr NSImage { get { return _NSImage.ClassHandle; } }
		public static IntPtr NSCursor { get { return _NSCursor.ClassHandle; } }
		public static IntPtr NSAlert { get { return _NSAlert.ClassHandle; } }
		public static IntPtr NSMenu { get { return _NSMenu.ClassHandle; } }
		public static IntPtr NSMenuItem { get { return _NSMenuItem.ClassHandle; } }
		public static IntPtr NSCell { get { return _NSCell.ClassHandle; } }
		public static IntPtr NSActionCell { get { return _NSActionCell.ClassHandle; } }
		public static IntPtr NSWindow { get { return _NSWindow.ClassHandle; } }
		public static IntPtr NSToolbar { get { return _NSToolbar.ClassHandle; } }
		public static IntPtr NSToolbarItem { get { return _NSToolbarItem.ClassHandle; } }
		public static IntPtr NSView { get { return _NSView.ClassHandle; } }
		public static IntPtr NSOpenGLView { get { return _NSOpenGLView.ClassHandle; } }
		public static IntPtr NSControl { get { return _NSControl.ClassHandle; } }
		public static IntPtr NSButton { get { return _NSButton.ClassHandle; } }
		public static IntPtr NSTableView { get { return _NSTableView.ClassHandle; } }
		public static IntPtr NSOutlineView { get { return _NSOutlineView.ClassHandle; } }
	}
}
