using System;
using System.IO;
using System.Net;
using System.Text;
using Monodoc;

namespace MonoDocumentationBrowser
{
	public sealed class Response : WebResponse
	{
		private Uri responseUri;
		private Node node;
		private WebHeaderCollection headerCollection;
		private Stream responseStream;

		internal Response(Uri uri, RootTree rootTree)
		{
			responseUri = uri;
			headerCollection = new WebHeaderCollection();
			headerCollection.Add("Content-Type", "text/html; Charset=utf-16");
			var data = Encoding.Unicode.GetBytes(rootTree.RenderUrl(uri.ToString(), out node));
			responseStream = new MemoryStream(data, false);
		}

		public override Uri ResponseUri { get { return responseUri; } }

		public override WebHeaderCollection Headers
		{
			get { return headerCollection; }
		}

		public override string ContentType
		{
			get { return "text/html"; }
		}

		public override long ContentLength
		{
			get { return responseStream.Length; }
		}

		public Node Node { get { return node; } }

		public override Stream GetResponseStream()
		{
			return responseStream;
		}
	}
}

