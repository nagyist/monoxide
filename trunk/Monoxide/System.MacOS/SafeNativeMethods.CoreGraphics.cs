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

		#region Path Construction

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextAddPath(IntPtr context, IntPtr path);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGContextCopyPath(IntPtr context);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextMoveToPoint")]
		private static extern void CGContextMoveToPoint_32(IntPtr c, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextMoveToPoint")]
		private static extern void CGContextMoveToPoint_64(IntPtr c, double x, double y);

		public static void CGContextMoveToPoint(IntPtr c, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGContextMoveToPoint_64(c, x, y);
			else
				CGContextMoveToPoint_32(c, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddLineToPoint")]
		private static extern void CGContextAddLineToPoint_32(IntPtr c, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddLineToPoint")]
		private static extern void CGContextAddLineToPoint_64(IntPtr c, double x, double y);

		public static void CGContextAddLineToPoint(IntPtr c, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGContextAddLineToPoint_64(c, x, y);
			else
				CGContextAddLineToPoint_32(c, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddQuadCurveToPoint")]
		private static extern void CGContextAddQuadCurveToPoint_32(IntPtr c, float cpx, float cpy, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddQuadCurveToPoint")]
		private static extern void CGContextAddQuadCurveToPoint_64(IntPtr c, double cpx, double cpy, double x, double y);

		public static void CGContextAddQuadCurveToPoint(IntPtr c, double cpx, double cpy, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGContextAddQuadCurveToPoint_64(c, cpx, cpy, x, y);
			else
				CGContextAddQuadCurveToPoint_32(c, (float)cpx, (float)cpy, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddCurveToPoint")]
		private static extern void CGContextAddCurveToPoint_32(IntPtr c, float cpx1, float cpy1, float cpx2, float cpy2, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddCurveToPoint")]
		private static extern void CGContextAddCurveToPoint_64(IntPtr c, double cpx1, double cpy1, double cpx2, double cpy2, double x, double y);

		public static void CGContextAddCurveToPoint(IntPtr c, double cpx1, double cpy1, double cpx2, double cpy2, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGContextAddCurveToPoint_64(c, cpx1, cpy1, cpx2, cpy2, x, y);
			else
				CGContextAddCurveToPoint_32(c, (float)cpx1, (float)cpy1, (float)cpx2, (float)cpy2, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddArc")]
		private static extern void CGContextAddArc_32(IntPtr c, float x, float y, float radius, float startAngle, float endAngle, [MarshalAs(UnmanagedType.I4)] bool clockwise);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddArc")]
		private static extern void CGContextAddArc_64(IntPtr c, double x, double y, double radius, double startAngle, double endAngle, [MarshalAs(UnmanagedType.I4)] bool clockwise);

		public static void CGContextAddArc(IntPtr c, double x, double y, double radius, double startAngle, double endAngle, bool clockwise)
		{
			if (ObjectiveC.LP64)
				CGContextAddArc_64(c, x, y, radius, startAngle, endAngle, clockwise);
			else
				CGContextAddArc_32(c, (float)x, (float)y, (float)radius, (float)startAngle, (float)endAngle, clockwise);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddArcToPoint")]
		private static extern void CGContextAddArcToPoint_32(IntPtr c, float x1, float y1, float x2, float y2, float radius);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddArcToPoint")]
		private static extern void CGContextAddArcToPoint_64(IntPtr c, double x1, double y1, double x2, double y2, double radius);

		public static void CGContextAddArcToPoint(IntPtr c, double x1, double y1, double x2, double y2, double radius)
		{
			if (ObjectiveC.LP64)
				CGContextAddArcToPoint_64(c, x1, y1, x2, y2, radius);
			else
				CGContextAddArcToPoint_32(c, (float)x1, (float)y1, (float)x2, (float)y2, (float)radius);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddRect")]
		private static extern void CGContextAddRect_32(IntPtr c, RectangleF rect);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddRect")]
		private static extern void CGContextAddRect_64(IntPtr c, Rectangle rect);

		public static void CGContextAddRect(IntPtr c, Rectangle rect)
		{
			if (ObjectiveC.LP64)
				CGContextAddRect_64(c, rect);
			else
				CGContextAddRect_32(c, rect);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddEllipseInRect")]
		private static extern void CGContextAddEllipseInRect_32(IntPtr c, RectangleF rect);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGContextAddEllipseInRect")]
		private static extern void CGContextAddEllipseInRect_64(IntPtr c, Rectangle rect);

		public static void CGContextAddEllipseInRect(IntPtr c, Rectangle rect)
		{
			if (ObjectiveC.LP64)
				CGContextAddEllipseInRect_64(c, rect);
			else
				CGContextAddEllipseInRect_32(c, rect);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextBeginPath(IntPtr c);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGContextClosePath(IntPtr c);

		#endregion

		#region Path Painting

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

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextDrawPath(IntPtr c, PathDrawingMode mode);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextEOFillPath(IntPtr c);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextFillPath(IntPtr c);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern void CGContextStrokePath(IntPtr c);

		#endregion

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
		private static extern void CGPathAddLineToPoint_32(IntPtr path, IntPtr m, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathLineToPoint")]
		private static extern void CGPathAddLineToPoint_64(IntPtr path, IntPtr m, double x, double y);

		public static void CGPathAddLineToPoint(IntPtr path, IntPtr m, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGPathAddLineToPoint_64(path, m, x, y);
			else
				CGPathAddLineToPoint_32(path, m, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathAddQuadCurveToPoint")]
		private static extern void CGPathAddQuadCurveToPoint_32(IntPtr path, IntPtr m, float cpx, float cpy, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathAddQuadCurveToPoint")]
		private static extern void CGPathAddQuadCurveToPoint_64(IntPtr path, IntPtr m, double cpx, double cpy, double x, double y);

		public static void CGPathAddQuadCurveToPoint(IntPtr path, IntPtr m, double cpx, double cpy, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGPathAddQuadCurveToPoint_64(path, m, cpx, cpy, x, y);
			else
				CGPathAddQuadCurveToPoint_32(path, m, (float)cpx, (float)cpy, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathAddCurveToPoint")]
		private static extern void CGPathAddCurveToPoint_32(IntPtr path, IntPtr m, float cpx1, float cpy1, float cpx2, float cpy2, float x, float y);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics, EntryPoint = "CGPathAddCurveToPoint")]
		private static extern void CGPathAddCurveToPoint_64(IntPtr path, IntPtr m, double cpx1, double cpy1, double cpx2, double cpy2, double x, double y);

		public static void CGPathAddCurveToPoint(IntPtr path, IntPtr m, double cpx1, double cpy1, double cpx2, double cpy2, double x, double y)
		{
			if (ObjectiveC.LP64)
				CGPathAddCurveToPoint_64(path, m, cpx1, cpy1, cpx2, cpy2, x, y);
			else
				CGPathAddCurveToPoint_32(path, m, (float)cpx1, (float)cpy1, (float)cpx2, (float)cpy2, (float)x, (float)y);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGPathCloseSubpath(IntPtr path);

		public struct CGPathElement
		{
			public PathCommand type;
			public IntPtr points;
		}

		public delegate void CGPathApplierFunction(IntPtr info, [In, MarshalAs(UnmanagedType.LPStruct)] ref CGPathElement element);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreGraphics)]
		public static extern IntPtr CGPathApply(IntPtr path, IntPtr info, [MarshalAs(UnmanagedType.FunctionPtr)] CGPathApplierFunction function);

		#endregion
	}
}
