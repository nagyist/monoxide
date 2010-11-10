using System;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	public class DrawableView : View
	{
		public event EventHandler<DrawEventArgs> Draw;

		public override bool IsOpaque { get { return true; } }

		protected override void DrawRectangle(GraphicsContext context, Rectangle bounds)
		{
			OnDraw(new DrawEventArgs(context, bounds));
		}

		protected virtual void OnDraw(DrawEventArgs e)
		{
			if (Draw != null)
				Draw(this, e);
		}
	}
}

