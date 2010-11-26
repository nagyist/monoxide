using System;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	public struct Thickness : IEquatable<Thickness>
	{
		public static Thickness Zero = new Thickness(0);

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

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
		}
		
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

		public static Thickness operator ++(Thickness t)
		{
			return new Thickness(t.Left + 1, t.Top + 1, t.Right + 1, t.Bottom + 1);
		}

		public static Thickness operator --(Thickness t)
		{
			return new Thickness(t.Left - 1, t.Top - 1, t.Right - 1, t.Bottom - 1);
		}

		public static implicit operator Thickness(int i)
		{
			return new Thickness(i);
		}

		public static implicit operator Thickness(double d)
		{
			return new Thickness(d);
		}

		public static Thickness operator *(int coef, Thickness t)
		{
			return new Thickness(coef * t.Left, coef * t.Top, coef * t.Right, coef * t.Bottom);
		}

		public static Thickness operator *(Thickness t, int coef)
		{
			return new Thickness(t.Left * coef, t.Top * coef, t.Right * coef, t.Bottom * coef);
		}

		public static Thickness operator *(double coef, Thickness t)
		{
			return new Thickness(coef * t.Left, coef * t.Top, coef * t.Right, coef * t.Bottom);
		}

		public static Thickness operator *(Thickness t, double coef)
		{
			return new Thickness(t.Left * coef, t.Top * coef, t.Right * coef, t.Bottom * coef);
		}

		public static Thickness operator +(Thickness t1, Thickness t2)
		{
			return new Thickness(t1.Left + t2.Left, t1.Top + t2.Top, t1.Right + t2.Right, t1.Bottom + t2.Bottom);
		}

		public static Thickness operator +(Thickness t, int i)
		{
			return new Thickness(t.Left + i, t.Top + i, t.Right + i, t.Bottom + i);
		}

		public static Thickness operator +(int i, Thickness t)
		{
			return new Thickness(i + t.Left, i + t.Top, i + t.Right, i + t.Bottom);
		}

		public static Thickness operator +(Thickness t, double d)
		{
			return new Thickness(t.Left + d, t.Top + d, t.Right + d, t.Bottom + d);
		}

		public static Thickness operator +(double d, Thickness t)
		{
			return new Thickness(d + t.Left, d + t.Top, d + t.Right, d + t.Bottom);
		}

		public static Thickness operator -(Thickness t1, Thickness t2)
		{
			return new Thickness(t1.Left - t2.Left, t1.Top - t2.Top, t1.Right - t2.Right, t1.Bottom - t2.Bottom);
		}

		public static Thickness operator -(Thickness t, int i)
		{
			return new Thickness(t.Left - i, t.Top - i, t.Right - i, t.Bottom - i);
		}

		public static Thickness operator -(int i, Thickness t)
		{
			return new Thickness(i - t.Left, i - t.Top, i - t.Right, i - t.Bottom);
		}

		public static Thickness operator -(Thickness t, double d)
		{
			return new Thickness(t.Left - d, t.Top - d, t.Right - d, t.Bottom - d);
		}

		public static Thickness operator -(double d, Thickness t)
		{
			return new Thickness(d - t.Left, d - t.Top, d - t.Right, d - t.Bottom);
		}

		public static Size operator +(Size s, Thickness t)
		{
			s.Width += t.Left;
			s.Height += t.Top;
			s.Width += t.Right;
			s.Height += t.Bottom;

			return s;
		}

		public static Size operator -(Size s, Thickness t)
		{
			s.Width -= t.Left;
			s.Height -= t.Top;
			s.Width -= t.Right;
			s.Height -= t.Bottom;

			return s;
		}

		public static Rectangle operator +(Rectangle r, Thickness t)
		{
			r.Left -= t.Left;
			r.Width += t.Left;
			r.Top -= t.Top;
			r.Height += t.Top;
			r.Width += t.Right;
			r.Height += t.Bottom;

			return r;
		}

		public static Rectangle operator -(Rectangle r, Thickness t)
		{
			r.Left += t.Left;
			r.Width -= t.Left;
			r.Top += t.Top;
			r.Height -= t.Top;
			r.Width -= t.Right;
			r.Height -= t.Bottom;

			return r;
		}
	}
}

