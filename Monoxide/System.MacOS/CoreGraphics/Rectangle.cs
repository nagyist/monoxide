using System;

namespace System.MacOS.CoreGraphics
{
	public struct Rectangle : IEquatable<Rectangle>
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
		
		public double Left
		{
			get { return Location.X; }
			set { Location.X = value; }
		}
		public double Top
		{
			get { return Location.Y; }
			set { Location.Y = value; }
		}
		public double Width
		{
			get { return Size.Width; }
			set { Size.Width = value; }
		}
		public double Height
		{
			get { return Size.Height; }
			set { Size.Height = value; }
		}

		public override string ToString()
		{
			return string.Format("[Rectangle: Left={0}, Top={1}, Width={2}, Height={3}]", Left, Top, Width, Height);
		}

		public bool Equals(Rectangle other)
		{
			return other.Location == Location && other.Size == Size;
		}

		public override bool Equals(object obj)
		{
			return obj is Rectangle ? Equals((Rectangle)obj) : base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Location.GetHashCode() ^ Size.GetHashCode();
		}

		public static bool operator ==(Rectangle a, Rectangle b)
		{
			return a.Location == b.Location && a.Size == b.Size;
		}

		public static bool operator !=(Rectangle a, Rectangle b)
		{
			return a.Size != b.Size || a.Location != b.Location;
		}
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
		
		public static implicit operator Rectangle(RectangleF rectangle) { return new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height); }
		
		public static implicit operator RectangleF(Rectangle rectangle) { return new RectangleF(rectangle); }
	}
}
