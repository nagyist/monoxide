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
			set { SafeNativeMethods.CGContextSetFillColorWithColor(GraphicsPort, value.NativePointer); }
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
	}
}

