using System;
using System.Collections.Generic;
using System.MacOS.AppKit;
using System.Xaml;
using NSApplication = System.MacOS.AppKit.Application;

namespace Monoxide
{
	internal sealed class Application : NSApplication
	{
		static void Main()
		{
			new Application().Run();
		}

		protected override void OnNewFile(EventArgs e)
		{
			//new CodeWindow().Show();
			(XamlServices.Load("CodeWindow.xaml") as CodeWindow).Show();
		}
	}
}
