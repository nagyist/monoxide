using System;

namespace System.MacOS.AppKit
{
	public struct Thickness : IEquatable<Thickness>
	{
		public Thickness(double uniformLength)
		{
			this.Left = uniformLength;
			this.Top = uniformLength;
			this.Right = uniformLength;
			this.Bottom = uniformLength;
		}
		
		public Thickness(double left, double top, double right, double bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
		
		public double Left { get; set; }
		public double Top { get; set; }
		public double Right { get; set; }
		public double Bottom { get; set; }
		
		public bool Equals(Thickness other)
		{
			return other.Left == Left
				&& other.Top == Top
				&& other.Right == Right
				&& other.Bottom == Bottom;
		}
		
		public override bool Equals(object obj)
		{
			return obj is Thickness ?
				Equals((Thickness)obj) :
				base.Equals(obj);
		}
		
		public override int GetHashCode()
		{
			return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
		}
		
		public static bool operator ==(Thickness a, Thickness b)
		{
			return a.Left == b.Left
				&& a.Top == b.Top
				&& a.Right == b.Right
				&& a.Bottom == b.Bottom;
		}
		
		public static bool operator !=(Thickness a, Thickness b)
		{
			return a.Left != b.Left
				|| a.Top != b.Top
				|| a.Right != b.Right
				|| a.Bottom != b.Bottom;
		}
	}
}

