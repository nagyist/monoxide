using System;

namespace System.MacOS.Foundation
{
	[NativeClass("NSURLProtocol", "Foundation")]
	public abstract class URLProtocol
	{
		public URLProtocol()
		{
		}

		public abstract void StartLoading();
		public abstract void StopLoading();
	}
}
