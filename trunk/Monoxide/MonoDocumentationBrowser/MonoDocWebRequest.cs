using System;
using System.IO;
using System.Net;
using Monodoc;

namespace MonoDocumentationBrowser
{
	public sealed class MonoDocWebRequest : WebRequest
	{
		private sealed class WebRequestCreator : IWebRequestCreate
		{
			private RootTree rootTree;

			public WebRequestCreator(RootTree rootTree) { this.rootTree = rootTree; }

			public WebRequest Create(Uri uri) { return new MonoDocWebRequest(uri, rootTree); }
		}

		public static void RegisterWithRootTree(RootTree rootTree)
		{
			WebRequest.RegisterPrefix("MonoDoc:", new WebRequestCreator(rootTree));
		}

		private Uri requestUri;
		private RootTree rootTree;
		private Func<WebResponse> getResponse;
		private Func<Stream> getRequestStream;

		private MonoDocWebRequest(Uri uri, RootTree rootTree)
		{
			requestUri = uri;
			this.rootTree = rootTree;
			getResponse = GetResponse;
			getRequestStream = GetRequestStream;
		}

		public override Uri RequestUri { get { return requestUri; } }

		public override WebHeaderCollection Headers { get; set; }

		public override string ContentType { get; set; }

		public override long ContentLength { get; set; }

		public override ICredentials Credentials { get; set; }

		public override bool PreAuthenticate { get; set; }

		public override string Method { get; set; }

		public override WebResponse GetResponse()
		{
			return new Response(requestUri, rootTree);
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			return getResponse.BeginInvoke(callback, state);
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			return getResponse.EndInvoke(asyncResult);
		}

		public override System.IO.Stream GetRequestStream()
		{
			return null;
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			return getRequestStream.BeginInvoke(callback, state);
		}

		public override System.IO.Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			return getRequestStream.EndInvoke(asyncResult);
		}
	}
}
