using System;

namespace System.MacOS.Foundation
{
	public struct URLProtocolClient
	{
		#region Method Selector Ids

		private static class Selectors
		{
			static class _URLProtocolCachedResponseIsValid { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:cachedResponseIsValid:"); }
			static class _URLProtocolDidCancelAuthenticationChallenge { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:didCancelAuthenticationChallenge:"); }
			static class _URLProtocolDidFailWithError { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:didFailWithError:"); }
			static class _URLProtocolDidLoadData { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:didLoadData:"); }
			static class _URLProtocolDidReceiveAuthenticationChallenge { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:didReceiveAuthenticationChallenge:"); }
			static class _URLProtocolDidReceiveResponseCacheStoragePolicy { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:didReceiveResponse:cacheStoragePolicy:"); }
			static class _URLProtocolWasRedirectedToRequestRedirectResponse { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocol:wasRedirectedToRequest:redirectResponse:"); }
			static class _URLProtocolDidFinishLoading { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("URLProtocolDidFinishLoading:"); }

			public static IntPtr URLProtocolCachedResponseIsValid { get { return _URLProtocolCachedResponseIsValid.SelectorHandle; } }
			public static IntPtr URLProtocolDidCancelAuthenticationChallenge { get { return _URLProtocolDidCancelAuthenticationChallenge.SelectorHandle; } }
			public static IntPtr URLProtocolDidFailWithError { get { return _URLProtocolDidFailWithError.SelectorHandle; } }
			public static IntPtr URLProtocolDidLoadData { get { return _URLProtocolDidLoadData.SelectorHandle; } }
			public static IntPtr URLProtocolDidReceiveAuthenticationChallenge { get { return _URLProtocolDidReceiveAuthenticationChallenge.SelectorHandle; } }
			public static IntPtr URLProtocolDidReceiveResponseCacheStoragePolicy { get { return _URLProtocolDidReceiveResponseCacheStoragePolicy.SelectorHandle; } }
			public static IntPtr URLProtocolWasRedirectedToRequestRedirectResponse { get { return _URLProtocolWasRedirectedToRequestRedirectResponse.SelectorHandle; } }
			public static IntPtr URLProtocolDidFinishLoading { get { return _URLProtocolDidFinishLoading.SelectorHandle; } }
		}

		#endregion

		private IntPtr nativePointer;

		internal URLProtocolClient(IntPtr nativePointer)
		{
			this.nativePointer = nativePointer;
		}
	}
}
