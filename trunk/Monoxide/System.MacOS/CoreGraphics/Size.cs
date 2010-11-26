using System;

namespace System.MacOS.CoreGraphics
{
	public struct Size : IEquatable<Size>
	{
		public static readonly Size Zero = new Size();
		public static readonly Size Infinite = new Size(double.PositiveInfinity, double.PositiveInfinity);
		
		public double Width;
		public double Height;
		
		public Size(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return string.Format("[Width={0}, Height={1}]", Width, Height);
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

		public static Size operator +(Size s1, Size s2)
		{
			return new Size(s1.Width + s2.Width, s1.Height + s2.Height);
		}

		public static Size operator -(Size s1, Size s2)
		{
			return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
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
