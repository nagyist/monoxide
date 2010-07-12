using System;
using System.ComponentModel;
using System.MacOS.AppKit;
using System.MacOS.WebKit;

namespace TestApplication
{
	public class MainWindow : Window
	{
		Button button1;
		Button button2;
		Button checkBox;
		WebView webView;
		
		public MainWindow()
		{
			CreateToolbarTemplate();
			Style |= WindowStyle.UnifiedTitleAndToolbar;
			Toolbar = new Toolbar() { TemplateName = "Main", Customizable = true };
			button1 = new Button() { Title = "Click me \u263A", Bounds = new Rectangle(180, 0, 100, 32), LayoutOptions = LayoutOptions.Top | LayoutOptions.Left };
			button1.Action += HandleButton1Action;
			button2 = new Button() { Title = "\u26A0 Don't click me \u2620", Bounds = new Rectangle(280, 0, 200, 32), LayoutOptions = LayoutOptions.Top | LayoutOptions.Left };
			button2.Action += HandleButton2Action;
			checkBox = new Button() { Title = "Closable", ButtonType = ButtonType.Switch, Bounds = new Rectangle(110, 100, 100, 24)};
			webView = new WebView() { Bounds = new Rectangle(0, 160, 480, 140), LayoutOptions = LayoutOptions.Width | LayoutOptions.Height };
#if DOCUMENT
			checkBox.Checked = true;
#endif
			Title = "Hello From C#";
			Content.Children.Add(button1);
			Content.Children.Add(button2);
			Content.Children.Add(checkBox);
			Content.Children.Add(new ColorWell() { Bounds = new Rectangle(0, 32, 100, 100) });
			Content.Children.Add(new ColorWell() { Bounds = new Rectangle(200, 32, 100, 100) });
			Content.Children.Add(new SearchField() { Bounds = new Rectangle(100, 32, 100, 22) });
			Content.Children.Add(new TextField() { Bounds = new Rectangle(100, 64, 100, 22) });
			Content.Children.Add(new ComboBox() { Bounds = new Rectangle(10, 132, 280, 22) });
			Content.Children.Add(webView);
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
					myItem3
				},
				new []
				{
					ToolbarItem.ColorsToolbarItem,
					myItem1,
					myItem3
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
