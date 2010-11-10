using System;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSImageRep", "AppKit")]
	public class ImageRepresentation
	{
		public ImageRepresentation()
		{
		}

		public bool HasAlpha { get; set; }
		public int? Height { get; set; }
		public int? Width { get; set; }
		public int? BitsPerPixel { get; set; }
		public Size RenderSize { get; set; }
	}
}
