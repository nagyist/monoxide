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
			treeView = new DocTreeView(rootTree);
			treeView.SelectionChanged += treeView_SelectionChanged;
			scrollView = new ScrollView() { Scrollers = Axis.Both };
			scrollView.DocumentView = treeView;
			webView = new WebView();
			splitView = new SplitView()
			{
				Bounds = new Rectangle(0, Window.SmallBottomBarHeight, 480, 360 - Window.SmallBottomBarHeight),
				LayoutOptions = LayoutOptions.Width | LayoutOptions.Height,
				Orientation = Orientation.Horizontal,
				DividerStyle = DividerStyle.Thin,
			};
			splitView.Children.Add(scrollView);
			splitView.Children.Add(webView);
			Content.Children.Add(splitView);
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
				url = new Uri(node.URL);
				Console.WriteLine("Visiting URL: {0}", url);
			}
			catch { }
			
			if (node.tree.HelpSource != null)
				htmlContent = node.tree.HelpSource.GetText(node.URL, out node);
			else
				htmlContent = rootTree.RenderUrl(node.URL, out node);
			
			webView.MainFrame.SetHtmlContent(htmlContent, new Uri("http://toto/"));
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			webView.SetMainFrameUrl("about:blank");
		}
	}
}
