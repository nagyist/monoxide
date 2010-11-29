using System;
using System.Threading;

namespace System.MacOS.CoreGraphics
{
	public sealed class Path : ICloneable, IDisposable
	{
		private PathData data;
		private IntPtr nativePath;

		public Path()
		{
			data = new PathData();
		}

		private Path(Path @base)
		{
			data = new PathData(@base.data);
		}

		internal Path(IntPtr nativePath, bool retain)
		{
			this.nativePath = nativePath;
			data = new PathData(nativePath, true);
			if (retain) SafeNativeMethods.CFRetain(nativePath);
		}

		~Path() { Dispose(false); }

		private void Dispose(bool disposing)
		{
			var nativePath = Interlocked.Exchange(ref this.nativePath, IntPtr.Zero);
			data = null;

			if (nativePath != IntPtr.Zero)
				SafeNativeMethods.CFRelease(nativePath);
		}

		public void Dispose() { Dispose(true); }

		private void CreateNative()
		{
			lock (data)
			{
				if (nativePath != IntPtr.Zero) return;
				Interlocked.Exchange(ref this.nativePath, data.CreateNativePath());
			}
		}

		internal IntPtr NativePointer
		{
			get
			{
				if (nativePath == IntPtr.Zero) CreateNative();

				return nativePath;
			}
		}

		public PathData Data { get { return data; } }

		public bool Sealed { get { return data.Sealed; } }

		public void Seal() { data.Seal(); }

		public void MoveTo(Point p) { data.MoveTo(p); }

		public void LineTo(Point p) { data.LineTo(p); }

		public void QuadraticCurveTo(Point c, Point p) { data.QuadraticCurveTo(c, p); }

		public void CubicCurveTo(Point c1, Point c2, Point p) { data.CubicCurveTo(c1, c2, p); }

		public void ClosePath() { data.ClosePath(); }

		public object Clone() { return new Path(this); }
	}
}

