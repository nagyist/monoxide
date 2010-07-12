using System;

namespace System.MacOS
{
	public sealed class ClassNotFoundException : Exception
	{
		public ClassNotFoundException(string className)
			: base (Localization.GetExceptionText("ClassNotFound", className)) { ClassName = className; }
		
		public string ClassName { get; private set; }
	}
}
