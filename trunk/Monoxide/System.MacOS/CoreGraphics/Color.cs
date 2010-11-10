using System;
using System.Collections.Generic;
using System.Threading;

namespace System.MacOS.CoreGraphics
{
	public class Color : IDisposable
	{
		public struct ComponentList : IList<double>
		{
			private double[] components;

			internal ComponentList(Color color) { this.components = new double[0]; }

			public double this[int index] { get { return components[index]; } }

			double IList<double>.this[int index]
			{
				get { return this[index]; }
				set { throw new NotSupportedException(); }
			}

			int IList<double>.IndexOf(double item) { throw new NotSupportedException(); }

			void IList<double>.Insert(int index, double item) { throw new NotSupportedException(); }

			void IList<double>.RemoveAt(int index) { throw new NotSupportedException(); }

			public IEnumerator<double> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			void ICollection<double>.Add(double item) { throw new NotSupportedException(); }

			void ICollection<double>.Clear() { throw new NotSupportedException(); }

			bool ICollection<double>.Contains(double item) { throw new NotSupportedException(); }

			public void CopyTo(double[] array, int arrayIndex) { components.CopyTo(array, arrayIndex); }

			bool ICollection<double>.Remove(double item) { throw new NotSupportedException(); }

			public int Count { get { return 0; } }

			bool ICollection<double>.IsReadOnly { get { return true; } }
		}

		private static void ValidateRange(double value)
		{
			if (value < 0 || value > 1 || double.IsNaN(value))
				throw new ArgumentOutOfRangeException("value");
		}

		internal static IntPtr CreateFromRGBA(double r, double g, double b, double a)
		{
			ValidateRange(r);
			ValidateRange(g);
			ValidateRange(b);
			ValidateRange(a);

			return SafeNativeMethods.CGColorCreateGenericRGB(r, g, b, a);
		}

		private IntPtr nativePointer;
		private bool disposed;

		internal Color(IntPtr nativePointer, bool retain)
		{
			if (nativePointer == IntPtr.Zero)
				throw new NullReferenceException();

			if (retain) SafeNativeMethods.CFRetain(nativePointer);

			this.nativePointer = nativePointer;
		}

		protected Color(ColorSpace c, params double[] components)
		{
		}

		~Color() { Dispose(false); }

		protected virtual void Dispose(bool disposing)
		{
			IntPtr nativePointer = Interlocked.Exchange(ref this.nativePointer, IntPtr.Zero);
			if (nativePointer != IntPtr.Zero)
				SafeNativeMethods.CFRelease(nativePointer);
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
	}
}
