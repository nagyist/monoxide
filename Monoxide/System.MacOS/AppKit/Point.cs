using System;

namespace System.MacOS.AppKit
{
	public struct Point
	{
		public static readonly Point Zero = new Point();
		
		public double X;
		public double Y;
		
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
	}
}
