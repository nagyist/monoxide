using System;
using System.Collections.Generic;
using System.MacOS.AppKit;

namespace Monoxide
{
	internal sealed class CodeWindow : Window
	{
		TextView textView;

		public CodeWindow()
		{
			textView = new TextView();
			Content.Children.Add(new ScrollView() { DocumentView = textView });
		}
	}
}
