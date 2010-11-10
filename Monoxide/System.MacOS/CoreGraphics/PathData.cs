using System;
using System.Collections.Generic;

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
	}
}

