using System;

namespace System.MacOS.AppKit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class SupportedFileTypeAttribute : Attribute
	{
		public SupportedFileTypeAttribute(string extension)
		{
			Extension = extension;
		}

		public string Extension { get; private set; }
	}
}
