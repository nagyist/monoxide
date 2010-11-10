using System;
using System.ComponentModel;
using System.MacOS.AppKit;
using System.MacOS.WebKit;
using System.MacOS.CoreGraphics;

namespace TestApplication
{
	public class MainWindow : Window
	{
		Button button1;
		Button button2;
		Button checkBox;
		WebView webView;
		DrawableView paintedView;
		
		public MainWindow()
		{
			CreateToolbarTemplate();
			Style |= WindowStyle.UnifiedTitleAndToolbar;
			Toolbar = new Toolbar() { TemplateName = "Main", Customizable = true };
			button1 = new Button() { Title = "Click me \u263A", Width = 100, Margin = new Thickness(double.NaN, double.NaN, 200, 0) };
			button1.Action += HandleButton1Action;
			button2 = new Button() { Title = "\u26A0 Don't click me \u2620", Width = 200, Margin = new Thickness(double.NaN, double.NaN, 0, 0) };
			button2.Action += HandleButton2Action;
			checkBox = new Button() { Title = "Closable", ButtonType = ButtonType.Switch, Width = 100, Height = 24, Margin = new Thickness(120, double.NaN, double.NaN, 100) };
			paintedView = new DrawableView() { Height = 100, Margin = new Thickness(0, double.NaN, 0, 160) };
			paintedView.Draw += HandlePaintedViewDraw;
			webView = new WebView() { Margin = new Thickness(0, 0, 0, 260) };
#if DOCUMENT
//			checkBox.Checked = true;
#endif
			Title = "Hello From C#";
			Content.Children.AddRange
			(
				button1,
				button2,
				checkBox,
				new ColorWell() { Width = 100, Height = 100, Margin = new Thickness(10, double.NaN, double.NaN, 32) },
				new ColorWell() { Width = 100, Height = 100, Margin = new Thickness(double.NaN, double.NaN, 10, 32) },
				new SearchField() { Width = double.NaN, Height = 22, Margin = new Thickness(120, double.NaN, 120, 32) },
				new TextField() { Width = double.NaN, Height = 22, Margin = new Thickness(120, double.NaN, 120, 64) },
				new ComboBox() { Width = double.NaN, Height = 22, Margin = new Thickness(10, double.NaN, 10, 132) },
				paintedView,
				webView
			);
		}
		
		private void CreateToolbarTemplate()
		{
			if (ToolbarTemplate.IsDefined("Main")) return;
			
			var segmentedControl = new SegmentedControl() { Style = SegmentStyle.TextureRounded, SelectionMode = ItemSelectionMode.None };
			segmentedControl.Segments.Add(new Segment() { Image = Image.GoLeftTemplate });
			segmentedControl.Segments.Add(new Segment() { Image = Image.GoRightTemplate });
			var myItem1 = new ImageToolbarItem("Foo") { Label = "Foo", Image = Image.Info };
			var myItem2 = new ImageToolbarItem("Bar") { Label = "Bar", Image = Image.UserAccounts };
			var myItem3 = new ViewToolbarItem("FooBar") { Label = "FooBar", View = segmentedControl };
			var myItem4 = new ViewToolbarItem("Search") { Label = "Search", View = new SearchField() };
			
			ToolbarTemplate.TryDefine
			(
				"Main",
				new []
				{
					ToolbarItem.ColorsToolbarItem,
					ToolbarItem.FontsToolbarItem,
					ToolbarItem.SeparatorToolbarItem,
					ToolbarItem.SpaceToolbarItem,
					ToolbarItem.FlexibleSpaceToolbarItem,
					myItem1,
					myItem2,
					myItem3,
					myItem4
				},
				new []
				{
					ToolbarItem.ColorsToolbarItem,
					myItem1,
					myItem3,
					ToolbarItem.FlexibleSpaceToolbarItem,
					myItem4
				}
			);
		}

		private void HandleButton1Action(object sender, EventArgs e)
		{
			Console.WriteLine("Button 1 Clicked");
			//throw new InvalidOperationException("This is a dummy error to scare you !");
			Application.Current.ShowStandardAboutPanel();
		}
		
		private void HandleButton2Action(object sender, EventArgs e)
		{
			Console.WriteLine("Button 2 Clicked");
			using (var alert = new Alert())
			{
				alert.Style = AlertStyle.Critical;
				alert.ShowSuppressionButton = true;
				alert.MessageText = "\u2623 You are doomed ! \u2623";
				alert.InformativeText = "You're a bad guy ! \u2639" + Environment.NewLine + "You shouldn't have clicked this button." + Environment.NewLine + "Enjoy you fate. \u2604";
				alert.AddButton("Whatever");
				while (!alert.Suppress)
				{
					Application.Beep();
					alert.ShowDialog();
				}
			}
		}

		private void HandlePaintedViewDraw (object sender, DrawEventArgs e)
		{
			e.Context.FillColor = new RGBColor(1, 0, 0, 1);
			e.Context.FillRectangle(e.Bounds);
			e.Context.FillColor = new RGBColor(0, 0, 1, 1);
			e.Context.FillRectangle(paintedView.ActualWidth / 4, paintedView.ActualHeight / 4, paintedView.ActualWidth / 2, paintedView.ActualHeight / 2);
			e.Context.FillColor = new RGBColor(0, 1, 0, 1);
			e.Context.FillRectangle(paintedView.ActualWidth / 8, paintedView.ActualHeight / 4, paintedView.ActualWidth / 8, paintedView.ActualHeight / 2);
			e.Context.FillRectangle(6 * paintedView.ActualWidth / 8, paintedView.ActualHeight / 4, paintedView.ActualWidth / 8, paintedView.ActualHeight / 2);
			e.Context.FillColor = new RGBColor(1, 1, 0, 1);
			e.Context.FillEllipse(3 * paintedView.ActualWidth / 8, paintedView.ActualHeight / 8, paintedView.ActualWidth / 4, 6 * paintedView.ActualHeight / 8);
		}
		
		public bool CanClose { get { return checkBox.Checked; } }
		
		protected override void OnLoad (EventArgs e)
		{
			webView.SetMainFrameUrl("http://www.perdu.com/");
			base.OnLoad(e);
		}
		
		protected override void OnMinimizing(EventArgs e)
		{
			Console.WriteLine("Window Minimizing");
			base.OnMinimizing(e);
		}
		
		protected override void OnMinimized(EventArgs e)
		{
			Console.WriteLine("Window Minimized");
			base.OnMinimizing(e);
		}
		
		protected override void OnClosed(EventArgs e)
		{
			Console.WriteLine("Window Closed");
			base.OnClosed (e);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			Console.WriteLine("Window Closing");
			if (e.Cancel = !CanClose)
				Application.Beep();
			base.OnClosing (e);
		}

	}
}
