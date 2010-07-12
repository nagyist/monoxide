using System;

namespace System.MacOS.AppKit
{
	public struct Size
	{
		public static readonly Size Zero = new Size();
		
		public double Width;
		public double Height;
		
		public Size(double width, double height)
		{
			Width = width;
			Height = height;
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
		
		public static implicit operator Size(SizeF size)
		{
			return new Size(size.Width, size.Height);
		}
		
		public static implicit operator SizeF(Size size) { return new SizeF(size); }
	}
}
