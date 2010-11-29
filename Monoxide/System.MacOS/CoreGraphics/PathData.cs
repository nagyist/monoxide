using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.MacOS.CoreGraphics
{
	public sealed class PathData
	{
		List<PathCommand> commandList;
		List<Point> pointList;
		bool @sealed;

		internal PathData()
		{
			commandList = new List<PathCommand>();
			pointList = new List<Point>();
		}

		internal PathData(PathData @base)
		{
			commandList = new List<PathCommand>(@base.commandList);
			pointList = new List<Point>(@base.pointList);
		}

		internal PathData(IntPtr nativePath, bool @sealed)
			: this()
		{
			this.@sealed = @sealed;
			var handle = GCHandle.Alloc(this);
			SafeNativeMethods.CGPathApply(nativePath, (IntPtr)handle, ImportPathElement);
			handle.Free();
		}

		internal void MoveTo(Point p)
		{
			if (@sealed) throw new InvalidOperationException();

			commandList.Add(PathCommand.MoveTo);
			pointList.Add(p);
		}

		internal void LineTo(Point p)
		{
			if (@sealed) throw new InvalidOperationException();

			commandList.Add(PathCommand.LineTo);
			pointList.Add(p);
		}

		internal void QuadraticCurveTo(Point c, Point p)
		{
			if (@sealed) throw new InvalidOperationException();

			commandList.Add(PathCommand.QuaddraticCurveTo);
			pointList.Add(c);
			pointList.Add(p);
		}

		internal void CubicCurveTo(Point c1, Point c2, Point p)
		{
			if (@sealed) throw new InvalidOperationException();

			commandList.Add(PathCommand.QuaddraticCurveTo);
			pointList.Add(c1);
			pointList.Add(c2);
			pointList.Add(p);
		}

		internal void ClosePath()
		{
			if (@sealed) throw new InvalidOperationException();

			commandList.Add(PathCommand.ClosePath);
		}

		internal void Append(PathData data)
		{
			commandList.AddRange(data.commandList);
			pointList.AddRange(data.pointList);
		}

		internal void Seal() { @sealed = true; }

		internal bool Sealed { get { return @sealed; } }

		private IntPtr SealPath(IntPtr nativePath)
		{
			var sealedNativePath = SafeNativeMethods.CGPathCreateCopy(nativePath);
			SafeNativeMethods.CFRelease(nativePath);
			return sealedNativePath;
		}

		internal IntPtr CreateNativePath()
		{
			var nativePath = SafeNativeMethods.CGPathCreateMutable();
			int pi = 0;

			for (int ci = 0; ci < commandList.Count; ci++)
			{
				var command = commandList[ci];
				Point p, cp1, cp2;

				switch ((int)command)
				{
					case (int)PathCommand.MoveTo:
						p = pointList[pi++];
						SafeNativeMethods.CGPathMoveToPoint(nativePath, IntPtr.Zero, p.X, p.Y);
						break;
					case (int)PathCommand.LineTo:
						p = pointList[pi++];
						SafeNativeMethods.CGPathAddLineToPoint(nativePath, IntPtr.Zero, p.X, p.Y);
						break;
					case (int)PathCommand.QuaddraticCurveTo:
						cp1 = pointList[pi++];
						p = pointList[pi++];
						SafeNativeMethods.CGPathAddQuadCurveToPoint(nativePath, IntPtr.Zero, cp1.X, cp1.Y, p.X, p.Y);
						break;
					case (int)PathCommand.CubicCurveTo:
						cp1 = pointList[pi++];
						cp2 = pointList[pi++];
						p = pointList[pi++];
						SafeNativeMethods.CGPathAddCurveToPoint(nativePath, IntPtr.Zero, cp1.X, cp1.Y, cp2.X, cp2.Y, p.X, p.Y);
						break;
					case (int)PathCommand.ClosePath:
						SafeNativeMethods.CGPathCloseSubpath(nativePath);
						break;
				}
			}

			return @sealed ? SealPath(nativePath) : nativePath;
		}

		internal void AddToContext(IntPtr context)
		{
			int pi = 0;

			for (int ci = 0; ci < commandList.Count; ci++)
			{
				var command = commandList[ci];
				Point p, cp1, cp2;

				switch ((int)command)
				{
					case (int)PathCommand.MoveTo:
						p = pointList[pi++];
						SafeNativeMethods.CGContextMoveToPoint(context, p.X, p.Y);
						break;
					case (int)PathCommand.LineTo:
						p = pointList[pi++];
						SafeNativeMethods.CGContextAddLineToPoint(context, p.X, p.Y);
						break;
					case (int)PathCommand.QuaddraticCurveTo:
						cp1 = pointList[pi++];
						p = pointList[pi++];
						SafeNativeMethods.CGContextAddQuadCurveToPoint(context, cp1.X, cp1.Y, p.X, p.Y);
						break;
					case (int)PathCommand.CubicCurveTo:
						cp1 = pointList[pi++];
						cp2 = pointList[pi++];
						p = pointList[pi++];
						SafeNativeMethods.CGContextAddCurveToPoint(context, cp1.X, cp1.Y, cp2.X, cp2.Y, p.X, p.Y);
						break;
					case (int)PathCommand.ClosePath:
						SafeNativeMethods.CGContextClosePath(context);
						break;
				}
			}
		}

		private static void ImportPathElement(IntPtr info, ref SafeNativeMethods.CGPathElement element)
		{
			var handle = GCHandle.FromIntPtr(info);
			var @this = handle.Target as PathData;
			int pointCount;

			@this.commandList.Add(element.type);

			switch ((int)element.type)
			{
				case (int)PathCommand.MoveTo:
				case (int)PathCommand.LineTo:
					pointCount = 1;
					break;
				case (int)PathCommand.QuaddraticCurveTo: pointCount = 2; break;
				case (int)PathCommand.CubicCurveTo: pointCount = 3; break;
				default: return;
			}

			if (ObjectiveC.LP64) @this.AppendPointList(element.points, pointCount);
			else @this.ApendPointFList(element.points, pointCount);
		}

		private unsafe void AppendPointList(IntPtr address, int count)
		{
			Point* points = (Point*)address;

			for (int i = 0; i < count; i++)
				pointList.Add(*points++);
		}

		private unsafe void ApendPointFList(IntPtr address, int count)
		{
			PointF* points = (PointF*)address;

			for (int i = 0; i < count; i++)
				pointList.Add(*points++);
		}
	}
}

