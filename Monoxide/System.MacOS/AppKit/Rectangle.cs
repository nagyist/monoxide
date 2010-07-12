using System;

namespace System.MacOS.AppKit
{
	public struct Rectangle
	{
		public static readonly Rectangle Zero = new Rectangle();
		
		public Point Location;
		public Size Size;
		
		public Rectangle(Point location, Size size)
		{
			Location = location;
			Size = size;
		}
		
		public Rectangle(double left, double top, double width, double height)
		{
			Location = new Point(left, top);
			Size = new Size(width, height);
		}
		
		public double Left { get { return Location.X; } }
		public double Top { get { return Location.Y; } }
		public double Width { get { return Size.Width; } }
		public double Height { get { return Size.Height; } }
	}
	
	internal struct RectangleF
	{
		public float Left;
		public float Top;
		public float Width;
		public float Height;
		
		public RectangleF(Rectangle rectangle)
		{
			Left = (float)rectangle.Left;
			Top = (float)rectangle.Top;
			Width = (float)rectangle.Width;
			Height = (float)rectangle.Height;
		}
		
		public static implicit operator Rectangle(RectangleF rectangle)
		{
			return new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		}
		
		public static implicit operator RectangleF(Rectangle rectangle) { return new RectangleF(rectangle); }
	}
}
