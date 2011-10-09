using System;
using System.MacOS.AppKit;
using System.Diagnostics;

namespace TestApplication
{
	public sealed class MyApplication : Application
	{
#if DEBUG
		/// <summary>Trace Listener used for debugging messages.</summary>
		/// <remarks>This is required to compense for the inability of MonoDevelop to display debug messages.</remarks>
		sealed class DebugListener : TraceListener
		{
			public override void Write(string message)
			{
				Console.Error.Write(message);
			}
			
			public override void WriteLine(string message)
			{
				Console.Error.WriteLine(message);
			}
		}
#endif
		
#if !DOCUMENT
		static MainWindow mainForm = new MainWindow();
#endif
		
		public static new class Commands
		{
			public static readonly Command CustomCommand = GetCommand("CustomCommand", "Custom Command");
			
			public static readonly Command New = GetCommand("New", "New", new ShortcutKey("n"));
			public static readonly Command Open = GetCommand("Open", "Open", new ShortcutKey("o"));
			public static readonly Command Save = GetCommand("Save", "Save", new ShortcutKey("s"));
			public static readonly Command SaveAs = GetCommand("SaveAs", "Save As…", new ShortcutKey("S"));
			public static readonly Command Close = GetCommand("Close", "Close", new ShortcutKey("w"));
			
			private static Command GetCommand(string name, string title)
			{
				var command = Command.Register(name);
				
				command.Title = title;
				
				return command;
			}
			
			private static Command GetCommand(string name, string title, ShortcutKey shortcutKey)
			{
				var command = Command.Register(name);
				
				command.Title = title;
				command.ShortcutKey = shortcutKey;
				
				return command;
			}
		}
		
		public static void Main()
		{
#if DEBUG
			Debug.Listeners.Add(new DebugListener());
#endif
#if DOCUMENT
			new MyApplication().Run();
#else
			new MyApplication().Run(mainForm);
#endif
		}
		
		MenuItem eventManagedMenuItem;
		int counter;
		
		public MyApplication()
		{
			PreferencesPanel = new PreferencesPanel();
		}
		
		protected override void CustomizeApplicationMenu(Menu menu)
		{
			eventManagedMenuItem = new MenuItem("Event MenuItem");
			eventManagedMenuItem.Click += delegate(object sender, EventArgs e)
			{
				Console.WriteLine("Menu Clicked");
				eventManagedMenuItem.Enabled = !eventManagedMenuItem.Enabled;
				counter++;
			};
			eventManagedMenuItem.Enabled = false;
			menu.MenuItems.Insert(0, new MenuItem(Commands.CustomCommand));
			menu.MenuItems.Insert(1, eventManagedMenuItem);
			menu.MenuItems.Insert(2, new SeparatorMenuItem());
		}
		
		private void BuildMenu()
		{
			Console.WriteLine("Building Application Menu");
			
			var fileMenu = new MenuItem("File");
			fileMenu.MenuItems.Add(new MenuItem(Commands.New));
			fileMenu.MenuItems.Add(new MenuItem(Commands.Open));
			fileMenu.MenuItems.Add(new SeparatorMenuItem());
			fileMenu.MenuItems.Add(new MenuItem(Commands.Save));
			fileMenu.MenuItems.Add(new MenuItem(Commands.SaveAs));
			fileMenu.MenuItems.Add(new SeparatorMenuItem());
			fileMenu.MenuItems.Add(new MenuItem(Commands.Close));
			
			MainMenu.MenuItems.Insert(1, fileMenu);
		}
		
		public override bool CanExecute(Command command)
		{
			if (command == Commands.CustomCommand)
				return !eventManagedMenuItem.Enabled;
			return base.CanExecute(command);
		}
		
		public override bool ValidateCommand(Command command)
		{
			if (command == Commands.CustomCommand)
				command.Title = "Custom Command " + counter.ToString();
			return base.ValidateCommand(command);
		}
		
		public void CustomCommand(object sender)
		{
			Console.WriteLine("Custom Command.");
			eventManagedMenuItem.Enabled = !eventManagedMenuItem.Enabled;
			counter++;
		}
		
		public void New(object sender)
		{
			new MainWindow().ShowAndMakeKey();
			Console.WriteLine("New.");
		}
		
		public void Open(object sender)
		{
			Console.WriteLine("Open.");
		}
		
		public void Save(object sender)
		{
			Console.WriteLine("Save.");
		}
		
		public void SaveAs(object sender)
		{
			Console.WriteLine("SaveAs.");
		}
		
		public void Close(object sender)
		{
			Console.WriteLine("Close.");
		}
		
#if DOCUMENT
		protected override void OnNewFile(EventArgs e)
		{
			new MainWindow().ShowAndMakeKey();
			base.OnNewFile (e);
		}
#endif
		
		protected override void OnLaunching(EventArgs e)
		{
			base.OnLaunching(e);
			Console.WriteLine("Application Launching.");
			BuildMenu();
		}
		
		protected override void OnLaunched(EventArgs e)
		{
			Console.WriteLine("Application Launched.");
			base.OnLaunched (e);
		}
		
		protected override void OnActivated(EventArgs e)
		{
			Console.WriteLine("Application Activated.");
#if !DOCUMENT
			Console.WriteLine("Main Window Title: " + mainForm.Title);
#endif
			base.OnActivated (e);
		}
		
		protected override void OnDeactivated(EventArgs e)
		{
			Console.WriteLine("Application Deactivated.");
			base.OnDeactivated (e);
		}
				
		protected override void OnHidden(EventArgs e)
		{
			Console.WriteLine("Application Hidden.");
			base.OnHidden(e);
		}
		
		protected override void OnShown(EventArgs e)
		{
			Console.WriteLine("Application Shown.");
			base.OnShown(e);
		}
		
		protected override void OnTerminating(EventArgs e)
		{
			Console.WriteLine("Application Terminating.");
			base.OnTerminating(e);
		}
	}
}
