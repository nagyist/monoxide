using System;

namespace System.MacOS.CoreGraphics
{
	public sealed class RGBColor : Color
	{
		public RGBColor(double red, double green, double blue, double alpha)
			: base(CreateFromRGBA(red, green, blue, alpha), false)
		{
		}
	}
}

