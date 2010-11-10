using System;

namespace System.MacOS.CoreGraphics
{
	public sealed class Path : IDisposable
	{
		private PathData data = new PathData();

		public Path()
		{
		}

		~Path() { Dispose(false); }

		private void Dispose(bool disposing)
		{
		}

		public void Dispose() { Dispose(true); }

		public bool Sealed { get { return data.Sealed; } }

		public void Seal() { data.Seal(); }

		public void MoveTo(Point p) { data.MoveTo(p); }

		public void LineTo(Point p) { data.LineTo(p); }

		public void QuadraticCurveTo(Point c, Point p) { data.QuadraticCurveTo(c, p); }

		public void CubicCurveTo(Point c1, Point c2, Point p) { data.CubicCurveTo(c1, c2, p); }

		public void ClosePath() { data.ClosePath(); }
	}
}

