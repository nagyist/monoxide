using System;

namespace System.MacOS.CoreGraphics
{
	public struct Size : IEquatable<Size>
	{
		public static readonly Size Zero = new Size();
		
		public double Width;
		public double Height;
		
		public Size(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public bool Equals(Size other)
		{
			return other.Width == Width && other.Height == Height;
		}

		public override bool Equals(object obj)
		{
			return obj is Size ? Equals((Size)obj) : base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Width.GetHashCode() ^ Height.GetHashCode();
		}

		public static bool operator ==(Size a, Size b)
		{
			return a.Width == b.Width && a.Height == b.Height;
		}

		public static bool operator !=(Size a, Size b)
		{
			return a.Width != b.Width || a.Height != b.Height;
		}

		public static explicit operator Point(Size s)
		{
			return new Point(s.Width, s.Height);
		}
	}
	
	internal struct SizeF
	{
		public float Width;
		public float Height;
		
		public SizeF(Size size)
		{
			Width = (float)size.Width;
			Height = (float)size.Height;
		}
		
		public static implicit operator Size(SizeF size) { return new Size(size.Width, size.Height); }
		
		public static implicit operator SizeF(Size size) { return new SizeF(size); }
	}
}
