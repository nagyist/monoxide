using System;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	public class DrawEventArgs : EventArgs
	{
		public DrawEventArgs(GraphicsContext context, Rectangle bounds)
		{
			Context = context;
			Bounds = bounds;
		}

		public GraphicsContext Context { get; private set; }

		public Rectangle Bounds { get; private set; }
	}
}

