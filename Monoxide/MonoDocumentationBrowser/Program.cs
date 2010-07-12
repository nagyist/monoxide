using System;
using System.MacOS.AppKit;

namespace MonoDocumentationBrowser
{
	public static class Program
	{
		public static void Main()
		{
			var application = new Application();
			
			application.NewFile += Aplication_NewFile;
			application.Run();
		}
		
		private static void Aplication_NewFile(object sender, EventArgs e)
		{
			new BrowserWindow().ShowAndMakeKey();
		}
	}
}
