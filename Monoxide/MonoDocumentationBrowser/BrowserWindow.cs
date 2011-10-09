using System;
using System.Collections.Generic;
using System.MacOS.AppKit;
using System.MacOS.WebKit;
using Monodoc;

namespace MonoDocumentationBrowser
{
	public class BrowserWindow : Window
	{
		SplitView splitView;
		ScrollView scrollView;
		DocTreeView treeView;
		WebView webView;
		RootTree rootTree;
		Uri url;
		string htmlContent;
		
		public BrowserWindow()
		{
			rootTree = RootTree.LoadTree(@"/Library/Frameworks/Mono.framework/Versions/Current/lib/monodoc");
			MonoDocWebRequest.RegisterWithRootTree(rootTree);
			treeView = new DocTreeView(rootTree);
			treeView.SelectionChanged += treeView_SelectionChanged;
			scrollView = new ScrollView() { Scrollers = Axis.Both };
			scrollView.DocumentView = treeView;
			webView = new WebView();
//			splitView = new SplitView()
//			{
//				Margin = new Thickness(0, 0, 0, Window.SmallBottomBarHeight),
//				Width = double.NaN,
//				Height = double.NaN,
//				Orientation = Orientation.Horizontal,
//				DividerStyle = DividerStyle.Thin,
//			};
//			splitView.Children.Add(scrollView);
//			splitView.Children.Add(webView);
//			Content.Children.Add(splitView);
			scrollView.HorizontalAlignment = HorizontalAlignment.Left;
			scrollView.Width = 250;
			scrollView.Margin = new Thickness(4, 4, 4, Window.SmallBottomBarHeight + 4);
			webView.HorizontalAlignment = HorizontalAlignment.Right;
			webView.Margin = new Thickness(258, 4, 4, Window.SmallBottomBarHeight + 4);
			Content.Children.Add(scrollView);
			Content.Children.Add(webView);
			CreateToolbarTemplate();
			Toolbar = new Toolbar() { Customizable = true, TemplateName = "Main" };
			BottomBarHeight = Window.SmallBottomBarHeight;
			Title = "Mono Documentation Browser";
		}
		
		private void CreateToolbarTemplate()
		{
			if (ToolbarTemplate.IsDefined("Main")) return;
			
			var segmentedControl = new SegmentedControl() { Style = SegmentStyle.TextureRounded, SelectionMode = ItemSelectionMode.None };
			segmentedControl.Segments.Add(new Segment() { Image = Image.GoLeftTemplate });
			segmentedControl.Segments.Add(new Segment() { Image = Image.GoRightTemplate });
			var backForwardItem = new ViewToolbarItem("Foo") { PaletteLabel = "Back/Forward", View = segmentedControl };
			
			ToolbarTemplate.TryDefine
			(
				"Main",
				new []
				{
					backForwardItem,
					ToolbarItem.SeparatorToolbarItem,
					ToolbarItem.SpaceToolbarItem,
					ToolbarItem.FlexibleSpaceToolbarItem
				},
				new []
				{
					backForwardItem
				}
			);
		}
		
		private void treeView_SelectionChanged(object sender, EventArgs e)
		{
			var node = treeView.SelectedItem as Node;
			
			if (node == null) return;
			
			try
			{
				Console.WriteLine("Visiting URL: {0}", node.PublicUrl);
				url = new Uri("MonoDoc:///" + Uri.EscapeDataString(node.PublicUrl));
			}
			catch (Exception ex) { Console.WriteLine(ex); }
			
			if (node.tree.HelpSource != null)
				htmlContent = node.tree.HelpSource.GetText(node.PublicUrl, out node);
			else
				htmlContent = rootTree.RenderUrl(node.PublicUrl, out node);
			
			webView.MainFrame.SetHtmlContent(htmlContent, new Uri("http://toto/"));
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			webView.SetMainFrameUrl("about:blank");
		}
	}
}
