using System;

namespace System.MacOS.AppKit
{
	public class ApplicationNotRunningException : Exception
	{
		public ApplicationNotRunningException()
			: base(Localization.GetExceptionText("ApplicationNotRunning")) {}
	}
}
