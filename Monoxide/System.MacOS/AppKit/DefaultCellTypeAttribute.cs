using System;

namespace System.MacOS.AppKit
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultCellTypeAttribute : Attribute
	{
		public DefaultCellTypeAttribute(Type cellType) { CellType = cellType; }
		
		public Type CellType { get; private set; }
	}
}
