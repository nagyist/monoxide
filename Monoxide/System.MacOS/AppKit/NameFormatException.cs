using System;

namespace System.MacOS.AppKit
{
	public class NameFormatException : FormatException
	{
		public NameFormatException(string name)
			: this(name, Localization.GetExceptionText("NameFormat", name)) { }
		
		public NameFormatException(string name, string message)
			: base(message) { Name = name; }
		
		public string Name { get; private set; }
	}
}
