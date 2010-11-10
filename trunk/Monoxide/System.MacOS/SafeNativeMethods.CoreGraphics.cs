using System;
using System.Security;
using System.Runtime.InteropServices;
using System.MacOS.CoreGraphics;

namespace System.MacOS
{
	partial class SafeNativeMethods
	{
		#region CGContext

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextSetFillColorWithColor(IntPtr c, IntPtr color);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextSetStrokeColorWithColor(IntPtr c, IntPtr color);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextBeginPath(IntPtr context);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextAddPath(IntPtr context, IntPtr path);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextFillRect")]
		private static extern void CGContextFillRect_32(IntPtr c, RectangleF rect);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextFillRect")]
		private static extern void CGContextFillRect_64(IntPtr c, Rectangle rect);

		public static void CGContextFillRect(IntPtr c, Rectangle rect)
		{
			if (ObjectiveC.LP64)
				CGContextFillRect_64(c, rect);
			else
				CGContextFillRect_32(c, rect);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextStrokeRect")]
		private static extern void CGContextStrokeRect_32(IntPtr c, RectangleF rect);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextStrokeRect")]
		private static extern void CGContextStrokeRect_64(IntPtr c, Rectangle rect);

		public static void CGContextStrokeRect(IntPtr c, Rectangle rect)
		{
			if (ObjectiveC.LP64)
				CGContextStrokeRect_64(c, rect);
			else
				CGContextStrokeRect_32(c, rect);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextFillEllipseInRect")]
		private static extern void CGContextFillEllipseInRect_32(IntPtr c, RectangleF rect);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextFillEllipseInRect")]
		private static extern void CGContextFillEllipseInRect_64(IntPtr c, Rectangle rect);

		public static void CGContextFillEllipseInRect(IntPtr c, Rectangle rect)
		{
			if (ObjectiveC.LP64)
				CGContextFillEllipseInRect_64(c, rect);
			else
				CGContextFillEllipseInRect_32(c, rect);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextStrokeEllipseInRect")]
		private static extern void CGContextStrokeEllipseInRect_32(IntPtr c, RectangleF rect);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextStrokeEllipseInRect")]
		private static extern void CGContextStrokeEllipseInRect_64(IntPtr c, Rectangle rect);

		public static void CGContextStrokeEllipseInRect(IntPtr c, Rectangle rect)
		{
			if (ObjectiveC.LP64)
				CGContextStrokeEllipseInRect_64(c, rect);
			else
				CGContextStrokeEllipseInRect_32(c, rect);
		}

		#endregion

		#region CGColor

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorCreate")]
		private static extern IntPtr CGColorCreate_32(IntPtr colorSpace, float[] components);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorCreate")]
		private static extern IntPtr CGColorCreate_64(IntPtr colorSpace, double[] components);

		public static IntPtr CGColorCreate(IntPtr colorSpace, double[] components)
		{
			if (ObjectiveC.LP64)
				return CGColorCreate_64(colorSpace, components);

			float[] fComponents = new float[components.Length];

			for (int i = 0; i < fComponents.Length; i++)
				fComponents[i] = (float)components[i];

			return CGColorCreate_32(colorSpace, fComponents);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorCreateGenericCMYK")]
		private static extern IntPtr CGColorCreateGenericCMYK_32(float cyan, float magenta, float yellow, float black, float alpha);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorCreateGenericCMYK")]
		private static extern IntPtr CGColorCreateGenericCMYK_64(double cyan, double magenta, double yellow, double black, double alpha);

		public static IntPtr CGColorCreateGenericCMYK(double cyan, double magenta, double yellow, double black, double alpha)
		{
			return ObjectiveC.LP64 ? CGColorCreateGenericCMYK_64(cyan, magenta, yellow, black, alpha) : CGColorCreateGenericCMYK_32((float)cyan, (float)magenta, (float)yellow, (float)black, (float)alpha);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorCreateGenericRGB")]
		private static extern IntPtr CGColorCreateGenericRGB_32(float red, float green, float blue, float alpha);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorCreateGenericRGB")]
		private static extern IntPtr CGColorCreateGenericRGB_64(double red, double green, double blue, double alpha);

		public static IntPtr CGColorCreateGenericRGB(double red, double green, double blue, double alpha)
		{
			return ObjectiveC.LP64 ? CGColorCreateGenericRGB_64(red, green, blue, alpha) : CGColorCreateGenericRGB_32((float)red, (float)green, (float)blue, (float)alpha);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorGetAlpha")]
		private static extern float CGColorGetAlpha_32(IntPtr color);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGColorGetAlpha")]
		private static extern double CGColorGetAlpha_64(IntPtr color);

		public static double CGColorGetAlpha(IntPtr color)
		{
			return ObjectiveC.LP64 ? CGColorGetAlpha_64(color) : CGColorGetAlpha_32(color);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGColorGetColorSpace(IntPtr color);

		#endregion

		#region CGPath

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGPathCreateMutable();

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGPathCreateCopy(IntPtr path);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathMoveToPoint")]
		private static extern void CGPathMoveToPoint_32(IntPtr path, IntPtr m, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathMoveToPoint")]
		private static extern void CGPathMoveToPoint_64(IntPtr path, IntPtr m, double x, double y);

		public static void CGPathMoveToPoint(IntPtr path, IntPtr m, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGPathMoveToPoint_64(path, m, x, y);
			else
				CGPathMoveToPoint_32(path, m, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathLineToPoint")]
		private static extern void CGPathLineToPoint_32(IntPtr path, IntPtr m, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathLineToPoint")]
		private static extern void CGPathLineToPoint_64(IntPtr path, IntPtr m, double x, double y);

		public static void CGPathLineToPoint(IntPtr path, IntPtr m, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGPathLineToPoint_64(path, m, x, y);
			else
				CGPathLineToPoint_32(path, m, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		private static extern IntPtr CGPathCloseSubpath(IntPtr path);

		#endregion
	}
}
