using System;
using System.Runtime.InteropServices;

namespace System.MacOS.CoreGraphics
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix
	{
		public static readonly Matrix Identity = new Matrix(1, 0, 1, 0, 0, 0);

		public double A;
		public double B;
		public double C;
		public double D;
		public double TX;
		public double TY;

		public Matrix(double a, double b, double c, double d, double tx, double ty)
		{
			A = a;
			B = b;
			C = c;
			D = d;
			TX = tx;
			TY = ty;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MatrixF
	{
		public float A;
		public float B;
		public float C;
		public float D;
		public float TX;
		public float TY;

		public MatrixF(Matrix matrix)
		{
			A = (float)matrix.A;
			B = (float)matrix.B;
			C = (float)matrix.C;
			D = (float)matrix.D;
			TX = (float)matrix.TX;
			TY = (float)matrix.TY;
		}

		public static implicit operator Matrix(MatrixF matrix) { return new Matrix(matrix.A, matrix.B, matrix.C, matrix.D, matrix.TX, matrix.TY); }

		public static implicit operator MatrixF(Matrix matrix) { return new MatrixF(matrix); }
	}
}

