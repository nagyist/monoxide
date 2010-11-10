using System;

namespace System.MacOS.CoreGraphics
{
	public struct Point : IEquatable<Point>
	{
		public static readonly Point Zero = new Point();
		
		public double X;
		public double Y;
		
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

		public bool Equals(Point other)
		{
			return other.X == X && other.Y == Y;
		}

		public override bool Equals(object obj)
		{
			return obj is Point ? Equals((Point)obj) : base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public static bool operator ==(Point a, Point b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Point a, Point b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public static explicit operator Size(Point p)
		{
			return new Size(p.X, p.Y);
		}
	}
	
	internal struct PointF
	{
		public float X;
		public float Y;
		
		public PointF(Point point)
		{
			X = (float)point.X;
			Y = (float)point.Y;
		}
		
		public static implicit operator Point(PointF point) { return new Point(point.X, point.Y); }
		
		public static implicit operator PointF(Point point) { return new PointF(point); }
	}
}
