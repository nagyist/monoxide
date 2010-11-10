using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSBitmapImageRep", "AppKit")]
	[SupportedFileType("tiff"), SupportedFileType("gif"), SupportedFileType("jpg"), SupportedFileType("bmp")]
	public class BitmapImageRepresentation : ImageRepresentation
	{
		public BitmapImageRepresentation()
		{
		}
	}
}

