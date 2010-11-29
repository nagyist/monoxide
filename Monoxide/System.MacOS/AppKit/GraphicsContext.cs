using System;
using System.MacOS.CoreGraphics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSGraphicsContext", "AppKit")]
	public sealed class GraphicsContext : IDisposable
	{
		#region Method Selector Ids
		
		#warning Don't forget to remove "GraphicsContext.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class currentContext { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("currentContext"); }
			static class graphicsPort { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("graphicsPort"); }

			public static IntPtr CurrentContext { get { return currentContext.SelectorHandle; } }
			public static IntPtr GraphicsPort { get { return GraphicsContext.Selectors.graphicsPort.SelectorHandle; } }
		}
		
		#endregion

		#region Cache

		private static readonly NativeObjectCache<GraphicsContext> contextCache = new NativeObjectCache<GraphicsContext>(c => c.NativePointer, p => new GraphicsContext(p, Kind.NextStep));
		
		internal static GraphicsContext GetInstance(IntPtr nativePointer) { return contextCache.GetObject(nativePointer); }
		
		#endregion

		internal enum Kind { NextStep, CoreGraphics, CoreImage }

		// Since NSGraphicsContext is essentially a wrapper around CGContext, this class represents both NSGraphcisContext and CGContext.
		private IntPtr nativePointer;
		private IntPtr graphicsPort;
		private bool disposed;

		public static GraphicsContext Current { get { return GetInstance(SafeNativeMethods.objc_msgSend(CommonClasses.NSGraphicsContext, Selectors.CurrentContext)); } }

		private GraphicsContext(IntPtr nativePointer, Kind kind)
		{
			switch ((int)kind)
			{
				case (int)Kind.NextStep:
					this.nativePointer = ObjectiveC.RetainObject(nativePointer);
					break;
//				case (int)Kind.CoreGraphics:
//					graphicsPort = nativePointer;
//					break;
//				case (int)Kind.CoreImage:
//					ciNativePointer = nativePointer;
//					break;
			}
		}

		~GraphicsContext() { Dispose(false); }

		private void Dispose(bool disposing)
		{
			if (nativePointer != (IntPtr)0)
			{
				contextCache.UnregisterObject(nativePointer);
				ObjectiveC.ReleaseObject(nativePointer);
				nativePointer = IntPtr.Zero;
			}
			disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool Disposed { get { return disposed; } }

		internal IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);

				return nativePointer;
			}
		}

		internal IntPtr GraphicsPort
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);

				if (graphicsPort == IntPtr.Zero)
					graphicsPort = SafeNativeMethods.objc_msgSend(NativePointer, Selectors.GraphicsPort);
				return graphicsPort;
			}
		}

		public Color FillColor
		{
			get { return null; }
			set { SafeNativeMethods.CGContextSetFillColorWithColor(GraphicsPort, value.NativePointer); }
		}

		public Color StrokeColor
		{
			get { return null; }
			set { SafeNativeMethods.CGContextSetStrokeColorWithColor(GraphicsPort, value.NativePointer); }
		}

		#region Path Construction

		public void AddPath(Path path)
		{
			// TODO: Allow usage of native CGPath objects
			path.Data.AddToContext(GraphicsPort);
		}

		public void AddRectangle(Rectangle rectangle)
		{
			SafeNativeMethods.CGContextAddRect(GraphicsPort, rectangle);
		}

		public void AddRectangle(Point location, Size size)
		{
			SafeNativeMethods.CGContextAddRect(GraphicsPort, new Rectangle(location, size));
		}

		public void AddRectangle(double left, double top, double width, double height)
		{
			SafeNativeMethods.CGContextAddRect(GraphicsPort, new Rectangle(left, top, width, height));
		}

		public void AddEllipse(Rectangle rectangle)
		{
			SafeNativeMethods.CGContextAddEllipseInRect(GraphicsPort, rectangle);
		}

		public void AddEllipse(Point location, Size size)
		{
			SafeNativeMethods.CGContextAddEllipseInRect(GraphicsPort, new Rectangle(location, size));
		}

		public void AddEllipse(double left, double top, double width, double height)
		{
			SafeNativeMethods.CGContextAddEllipseInRect(GraphicsPort, new Rectangle(left, top, width, height));
		}

		public void AddRoundedRectangle(Rectangle rectangle, double cornerRadius)
		{
			if (cornerRadius <= 0
				|| double.IsNaN(cornerRadius)
				|| double.IsPositiveInfinity(cornerRadius)
				|| 2 * cornerRadius >= rectangle.Width
				|| 2 * cornerRadius >= rectangle.Height)
				throw new ArgumentOutOfRangeException("cornerRadius");

			var c = GraphicsPort;
			double right = rectangle.Left + rectangle.Width;
			double bottom = rectangle.Top + rectangle.Height;

			SafeNativeMethods.CGContextMoveToPoint(c, rectangle.Left, rectangle.Top + cornerRadius);
			SafeNativeMethods.CGContextAddArcToPoint(c, rectangle.Left, rectangle.Top, rectangle.Left + cornerRadius, rectangle.Top, cornerRadius);
			SafeNativeMethods.CGContextAddLineToPoint(c, right - cornerRadius, rectangle.Top);
			SafeNativeMethods.CGContextAddArcToPoint(c, right, rectangle.Top, right, rectangle.Top + cornerRadius, cornerRadius);
			SafeNativeMethods.CGContextAddLineToPoint(c, right, bottom - cornerRadius);
			SafeNativeMethods.CGContextAddArcToPoint(c, right, bottom, right - cornerRadius, bottom, cornerRadius);
			SafeNativeMethods.CGContextAddLineToPoint(c, rectangle.Left + cornerRadius, bottom);
			SafeNativeMethods.CGContextAddArcToPoint(c, rectangle.Left, bottom, rectangle.Left, bottom - cornerRadius, cornerRadius);
			SafeNativeMethods.CGContextClosePath(c);
		}

		public void MoveTo(Point p)
		{
			SafeNativeMethods.CGContextMoveToPoint(GraphicsPort, p.X, p.Y);
		}

		public void MoveTo(double x, double y)
		{
			SafeNativeMethods.CGContextMoveToPoint(GraphicsPort, x, y);
		}

		public void AddLineTo(Point p)
		{
			SafeNativeMethods.CGContextAddLineToPoint(GraphicsPort, p.X, p.Y);
		}

		public void AddLineTo(double x, double y)
		{
			SafeNativeMethods.CGContextAddLineToPoint(GraphicsPort, x, y);
		}

		public void AddQuadCurveTo(Point controlPoint, Point p)
		{
			SafeNativeMethods.CGContextAddQuadCurveToPoint(GraphicsPort, controlPoint.X, controlPoint.Y, p.X, p.Y);
		}

		public void AddQuadCurveTo(double cpx, double cpy, double x, double y)
		{
			SafeNativeMethods.CGContextAddQuadCurveToPoint(GraphicsPort, cpx, cpy, x, y);
		}

		public void AddQuadCurveTo(Point controlPoint1, Point controlPoint2, Point p)
		{
			SafeNativeMethods.CGContextAddCurveToPoint(GraphicsPort, controlPoint1.X, controlPoint1.Y, controlPoint2.X, controlPoint2.Y, p.X, p.Y);
		}

		public void AddQuadCurveTo(double cpx1, double cpy1, double cpx2, double cpy2, double x, double y)
		{
			SafeNativeMethods.CGContextAddCurveToPoint(GraphicsPort, cpx1, cpy1, cpx2, cpy2, x, y);
		}

		public void AddArc(Point center, double radius, double startAngle, double endAngle, bool clockwise)
		{
			SafeNativeMethods.CGContextAddArc(GraphicsPort, center.X, center.Y, radius, startAngle, endAngle, clockwise);
		}

		public void AddArc(double centerX, double centerY, double radius, double startAngle, double endAngle, bool clockwise)
		{
			SafeNativeMethods.CGContextAddArc(GraphicsPort, centerX, centerY, radius, startAngle, endAngle, clockwise);
		}

		public void AddArcTo(Point p1, Point p2, double radius)
		{
			SafeNativeMethods.CGContextAddArcToPoint(GraphicsPort, p1.X, p1.Y, p2.X, p2.Y, radius);
		}

		public void AddArcTo(double x1, double y1, double x2, double y2, double radius)
		{
			SafeNativeMethods.CGContextAddArcToPoint(GraphicsPort, x1, y1, x2, y2, radius);
		}

		public void BeginPath()
		{
			SafeNativeMethods.CGContextBeginPath(GraphicsPort);
		}

		public void ClosePath()
		{
			SafeNativeMethods.CGContextClosePath(GraphicsPort);
		}

		public Path CopyPath()
		{
			return new Path(SafeNativeMethods.CGContextCopyPath(GraphicsPort), false);
		}

		#endregion

		#region Path Painting

		public void DrawPath(PathDrawingMode mode)
		{
			SafeNativeMethods.CGContextDrawPath(GraphicsPort, mode);
		}

		public void DrawPath(Path path, PathDrawingMode mode)
		{
			BeginPath(); // Clear the path before drawing
			AddPath(path); // Adds the path to the context's now empty current path
			DrawPath(mode); // Render the context's current path
			BeginPath(); // Clear the path after drawing (same as with other path painting methods)
		}

		public void FillRectangle(Rectangle rectangle)
		{
			SafeNativeMethods.CGContextFillRect(GraphicsPort, rectangle);
		}

		public void FillRectangle(Point location, Size size)
		{
			SafeNativeMethods.CGContextFillRect(GraphicsPort, new Rectangle(location, size));
		}

		public void FillRectangle(double left, double top, double width, double height)
		{
			SafeNativeMethods.CGContextFillRect(GraphicsPort, new Rectangle(left, top, width, height));
		}

		public void StrokeRectangle(Rectangle rectangle)
		{
			SafeNativeMethods.CGContextFillRect(GraphicsPort, rectangle);
		}

		public void StrokeRectangle(Point location, Size size)
		{
			SafeNativeMethods.CGContextFillRect(GraphicsPort, new Rectangle(location, size));
		}

		public void StrokeRectangle(double left, double top, double width, double height)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, new Rectangle(left, top, width, height));
		}

		public void FillEllipse(Rectangle rectangle)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, rectangle);
		}

		public void FillEllipse(Point location, Size size)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, new Rectangle(location, size));
		}

		public void FillEllipse(double left, double top, double width, double height)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, new Rectangle(left, top, width, height));
		}

		public void StrokeEllipse(Rectangle rectangle)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, rectangle);
		}

		public void StrokeEllipse(Point location, Size size)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, new Rectangle(location, size));
		}

		public void StrokeEllipse(double left, double top, double width, double height)
		{
			SafeNativeMethods.CGContextFillEllipseInRect(GraphicsPort, new Rectangle(left, top, width, height));
		}

		public void FillRoundedRectangle(Rectangle rectangle, double cornerRadius)
		{
			BeginPath();
			AddRoundedRectangle(rectangle, cornerRadius);
			SafeNativeMethods.CGContextFillPath(GraphicsPort);
			BeginPath();
		}

		public void StrokeRoundedRectangle(Rectangle rectangle, double cornerRadius)
		{
			BeginPath();
			AddRoundedRectangle(rectangle, cornerRadius);
			SafeNativeMethods.CGContextStrokePath(GraphicsPort);
			BeginPath();
		}

		#endregion
	}
}

