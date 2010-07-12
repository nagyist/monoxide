using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;

namespace System.MacOS.AppKit
{
	[NativeClass("NSApplication", "AppKit")]
	public class Application : EventTarget
	{
		#region Commands Class
		
		public static class Commands
		{
			public static readonly Command About = Localization.GetApplicationCommand("About");
			public static readonly Command Preferences = Localization.GetCommand("Preferences", new ShortcutKey(","));
			public static readonly Command Hide = Localization.GetApplicationCommand("Hide", new ShortcutKey("h"));
			public static readonly Command HideOtherApplications = Localization.GetCommand("HideOtherApplications", new ShortcutKey("h", ModifierKeys.Command | ModifierKeys.Alternate));
			public static readonly Command ShowAllApplications = Localization.GetCommand("ShowAllApplications", "UnhideAllApplications");
			public static readonly Command MiniaturizeAll = Localization.GetCommand("MiniaturizeAll");
			public static readonly Command Quit = Localization.GetApplicationCommand("Quit", "Terminate", new ShortcutKey("q"));
		}
		
		#endregion
		
		#region Native Interop
		
		#pragma warning disable 414
		
		// The run loop modes are in fact plain NSString values.
		// Their values are the symbol name, but I don't know if it's the reference or the value that matters.
		// Thus, the run loop modes are imported here, using the globally defined NSString reference, avoiding any problem.
		private static readonly IntPtr NSDefaultRunLoopMode = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("CoreFoundation", "kCFRunLoopDefaultMode"));
		private static readonly IntPtr NSRunLoopCommonModes = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("CoreFoundation", "kCFRunLoopCommonModes"));
		private static readonly IntPtr NSModalPanelRunLoopMode = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSModalPanelRunLoopMode"));
		
		#pragma warning restore 414
		
		private static readonly IntPtr nextEventMatchingMaskUntilDateInModeDequeue = ObjectiveC.GetSelector("nextEventMatchingMask:untilDate:inMode:dequeue:");
		private static readonly IntPtr sendEvent = ObjectiveC.GetSelector("sendEvent:");
		private static readonly IntPtr updateWindows = ObjectiveC.GetSelector("updateWindows");
		private static readonly IntPtr applicationShouldTerminate = ObjectiveC.GetSelector("applicationShouldTerminate:");
		
		/// <summary>Handles standard notifications and raise the corresponding events.</summary>
		/// <param name="self">A <see cref="IntPtr"/> representing the sender. Unused.</param>
		/// <param name="_cmd">A <see cref="IntPtr"/> representing the selector. Unused.</param>
		/// </param>
		/// <param name="aNotification">A <see cref="IntPtr"/> representing the notification object.</param>
		/// <remarks>
		/// The notifications can be regognized either by selector name or with the use of the NSNotification object.
		/// The current implementation relies on the NSNotification object, and ignores the selector used.
		/// </remarks>
		[SelectorStub("applicationWillFinishLaunching:")]
		[SelectorStub("applicationDidFinishLaunching:")]
		[SelectorStub("applicationDidBecomeActive:")]
		[SelectorStub("applicationDidResignActive:")]
		[SelectorStub("applicationDidHide:")]
		[SelectorStub("applicationDidUnhide:")]
		[SelectorStub("applicationWillTerminate:")]
		private static void HandleNotification(IntPtr self, IntPtr _cmd, IntPtr aNotification)
		{
			using (var pool = LocalAutoReleasePool.Create())
			{
				var notificationName = ObjectiveC.GetNotificationName(aNotification);
				
				Debug.WriteLine("Notification received: " + notificationName);
				
				switch (notificationName)
				{
					case "NSApplicationWillFinishLaunchingNotification": Current.OnLaunching(EventArgs.Empty); break;
					case "NSApplicationDidFinishLaunchingNotification": Current.OnLaunched(EventArgs.Empty); break;
					case "NSApplicationDidBecomeActiveNotification": Current.OnActivated(EventArgs.Empty); break;
					case "NSApplicationDidResignActiveNotification": Current.OnDeactivated(EventArgs.Empty); break;
					case "NSApplicationDidHideNotification": Current.OnHidden(EventArgs.Empty); break;
					case "NSApplicationDidUnhideNotification": Current.OnShown(EventArgs.Empty); break;
					case "NSApplicationWillTerminateNotification": Current.OnTerminating(EventArgs.Empty); break;
				}
			}
		}
		
		[SelectorStub("about:")]
		private static void About(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			Current.About(ObjectiveC.GetManagedObject(sender));
		}
		
		[SelectorStub("preferences:")]
		private static void Preferences(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			Current.Preferences(ObjectiveC.GetManagedObject(sender));
		}
		
		[SelectorStub("applicationShouldTerminate:")]
		private static IntPtr ApplicationShouldTerminate(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			return (IntPtr)SafeNativeMethods.ApplicationTerminateReply.TerminateNow;
		}
		
		[SelectorStub("applicationShouldOpenUntitledFile:")]
		private static bool ApplicationShouldOpenUntitledFile(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			return true;
		}
		
		[SelectorStub("applicationOpenUntitledFile:")]
		private static bool ApplicationOpenUntitledFile(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			using (var pool = LocalAutoReleasePool.Create())
				Current.OnNewFile(EventArgs.Empty);
			return true;
		}
		
		private static int ShowExceptionPanel(Exception exception)
		{
			using (var textView = new TextView() { Editable = true, Bounds = new Rectangle(0, 0, 400, 200) })
				using (var accessoryView = new ScrollView { DocumentView = textView, Scrollers = Axis.Both, Bounds = new Rectangle(0, 0, 400, 200) })
					using (var alert = new Alert())
					{
						alert.Style = AlertStyle.Warning;
						alert.MessageText = "An unhandled exception occured in your application.";
						alert.InformativeText = exception.Message;
						alert.AddButton("OK");
						alert.AccessoryView = accessoryView;
						textView.InsertText(exception.ToString());
						return alert.ShowDialog();
					}
		}
		
		[SelectorStub("run")]
		private static void Run(IntPtr self, IntPtr _cmd)
		{
			// I don't think CS0219 here is normal behavior here, and the code is working as intended, so I disable the warning.
			#pragma warning disable 219
			LocalAutoReleasePool pool;
			#pragma warning restore 219
			IntPtr distantFuture = SafeNativeMethods.objc_msgSend(ObjectiveC.Classes.NSDate, ObjectiveC.Selectors.DistantFuture);
			IntPtr runLoopMode = NSDefaultRunLoopMode;
			//IntPtr runLoopMode = ObjectiveC.StringToNativeString("kCFRunLoopDefaultMode"); // Hacky way of defining the loop mode
			// The documentation states that the value for the event mask is 0xFFFFFFFF, but maybe -1 would be ok too…
			IntPtr eventMask = unchecked((IntPtr)0xFFFFFFFFUL);
			
			if (running)
				throw new InvalidOperationException(Localization.GetExceptionText("ApplicationAlreadyRunning"));
			
			running = true;
			
			using (pool = LocalAutoReleasePool.Create())
				SafeNativeMethods.objc_msgSend(self, ObjectiveC.GetSelector("finishLaunching"));
			
			while (running)
			{
				using (pool = LocalAutoReleasePool.Create())
				{
					var @event = SafeNativeMethods.objc_msgSend(
						self, nextEventMatchingMaskUntilDateInModeDequeue,
						eventMask,
						distantFuture,
						runLoopMode,
						true);
					
					try
					{
						SafeNativeMethods.objc_msgSend(self, sendEvent, @event);
						SafeNativeMethods.objc_msgSend(self, updateWindows);
					}
					catch (Exception ex) // Something more is needed for correct error handling…
					{
						//ShowExceptionPanel(ex);
						//running = false;
						Console.Error.WriteLine(ex);
						Environment.FailFast(Localization.GetExceptionText("Fail"));
					}
				}
			}
			
			//ObjectiveC.ReleaseObject(runLoopMode);
		}
		
		[SelectorStub("isRunning")]
		private static bool IsRuning(IntPtr self, IntPtr _cmd)
		{
			return Current.IsRunning;
		}
		
		[SelectorStub("terminate:")]
		private static void Terminate(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			var reply = (SafeNativeMethods.ApplicationTerminateReply)SafeNativeMethods.objc_msgSend(
				SafeNativeMethods.objc_msgSend(self, CommonSelectors.Delegate),
				applicationShouldTerminate,
				self);
			
			switch (reply)
			{
				case SafeNativeMethods.ApplicationTerminateReply.TerminateCancel:
					break;
				case SafeNativeMethods.ApplicationTerminateReply.TerminateLater:
					break;
				case SafeNativeMethods.ApplicationTerminateReply.TerminateNow:
					// Try to avoid leak with a garbage-collectable AutoReleasePool.
					// It is uncertain wether this code really has an useful effect or not.
					var pool = new AutoReleasePool();
					Environment.Exit(0);
					GC.KeepAlive(pool);
					break;
			}
		}
		
		#endregion
		
		private static readonly object syncRoot = new object();
		public static Application Current { get; private set; }
		private static volatile bool running; // This is a shared flag…
		private bool canShowAllApplications; // This is kind of a hack, but necessary until there is something better.
		
		private volatile IntPtr nativePointer;
		/// <summary>Represents the application window for single window applications.</summary>
		private Window applicationWindow;
		private AutoReleasePool creationPool;
		
		public EventHandler Launching;
		public EventHandler Launched;
		public EventHandler Activated;
		public EventHandler Deactivated;
		public EventHandler Hidden;
		public EventHandler Shown;
		public EventHandler Terminating;
		public EventHandler NewFile;
		
		public Application()
		{
			// Setup an auto release pool that will be disposed on application run
			creationPool = new AutoReleasePool();
			lock (syncRoot)
				if (Current != null)
					throw new InvalidOperationException();
				else
					Current = this;
			// This should not be needed, but mono sucks here…
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(CoreFoundation.Locale.Current.ClrCultureName);
			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
		}
		
		internal sealed override IntPtr NativePointer
		{
			get
			{
				if (nativePointer == IntPtr.Zero)
					lock (syncRoot)
						if (nativePointer == IntPtr.Zero)
						{
							// This is needed to avoid memory leaks when calling NSApplication.sharedApplication.
							// It covers a bigger part of the code just in case it would be needed.
							using (var pool = LocalAutoReleasePool.Create())
							{
								// This is a "hack" for non-bundled applications
								if (Bundle.MainBundle == null || Bundle.MainBundle.Name == null)
								{
									Debug.WriteLine("Transforming the process to UI process.");
									long psn;
									// Transform the process to an UI-Application (with dock icon and menu)
									// This will actually prevent the icon from bouncing, but it is needed if the application is not started from a bundle.
									SafeNativeMethods.GetCurrentProcess(out psn);
									SafeNativeMethods.TransformProcessType(ref psn, SafeNativeMethods.ProcessApplicationTransformState.ProcessTransformToForegroundApplication);
									SafeNativeMethods.SetFrontProcess(ref psn);
								}
								// Gets the NSApplication singleton, store it, and assign the delegate to it
								nativePointer = SafeNativeMethods.objc_msgSend(NativeClass, ObjectiveC.GetSelector("sharedApplication"));
								Debug.WriteLine("The native application class is: " + SafeNativeMethods.object_getClassName(nativePointer));
								// Sets the application object as its own delegate… This seems to work even when not run from a bundle.
								SafeNativeMethods.objc_msgSend(nativePointer, CommonSelectors.SetDelegate, nativePointer);
								// Register the native application object
								ObjectiveC.RegisterObjectPair(this, nativePointer);
							}
						}
				return nativePointer;
			}
		}
		
		/// <summary>Gets a value indicating wether the main event loop is running.</summary>
		public bool IsRunning
		{
			get
			{
				return running;
			}
		}
		
		/// <summary>Gets the <see cref="DockTile"/> associated with the <c>Application</c>.</summary>
		public DockTile DockTile
		{
			get
			{
				return new DockTile(IntPtr.Zero);
			}
		}
		
		public Panel PreferencesPanel { get; set; }
		
		/// <summary>Gets the application's active window.</summary>
		/// <remarks>This is what is called the main window in native AppKit.</remarks>
		public Window MainWindow
		{
			get
			{
				return Window.GetInstance(SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("mainWindow")));
			}
		}
		
		/// <summary>Gets the application's key window.</summary>
		/// <remarks>The key window is the window receiving keyboard input.</remarks>
		public Window KeyWindow
		{
			get
			{
				return Window.GetInstance(SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("keyWindow")));
			}
		}
		
		public Menu MainMenu
		{
			get
			{
				return nativePointer != IntPtr.Zero ?
					Menu.GetInstance(SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("mainMenu"))) :
					null;
			}
		}
		
//		public static PresentationOptions SetPresentationOptions
//		{
//			get
//			{
//				return (PresentationOptions)SafeNativeMethods.objc_msgSend(nativePointer, ObjectiveC.GetSelector("presentationOptions"));
//			}
//			set
//			{
//				SafeNativeMethods.objc_msgSend(nativePointer, ObjectiveC.GetSelector("setPresentationOptions:"), (IntPtr)options);
//			}
//		}
		
		public CommandTarget GetTargetForCommand(Command command)
		{
			var keyWindow = KeyWindow;
			var mainWindow = MainWindow;
			
			if (keyWindow != null)
			{
				var nativePointer = SafeNativeMethods.objc_msgSend(keyWindow.NativePointer, CommonSelectors.FirstResponder);
				
				while (nativePointer != IntPtr.Zero)
				{
					var target = ObjectiveC.GetManagedObject(nativePointer) as CommandTarget;
					
					if (target != null)
						if (target.ValidateCommand(command))
							return target;
					
					nativePointer = SafeNativeMethods.objc_msgSend(nativePointer, CommonSelectors.NextResponder);
				}
				
				if (keyWindow.ValidateCommand(command))
					return keyWindow;
			}
			if (mainWindow != keyWindow && mainWindow != null)
			{
				var nativePointer = SafeNativeMethods.objc_msgSend(mainWindow.NativePointer, CommonSelectors.FirstResponder);
				
				while (nativePointer != IntPtr.Zero)
				{
					var target = ObjectiveC.GetManagedObject(nativePointer) as CommandTarget;
					
					if (target != null)
						if (target.ValidateCommand(command))
							return target;
					
					nativePointer = SafeNativeMethods.objc_msgSend(nativePointer, CommonSelectors.NextResponder);
				}
				
				if (mainWindow.ValidateCommand(command))
					return mainWindow;
			}
			if (ValidateCommand(command) || command == Commands.ShowAllApplications)
				return this;
			
			return null;
		}
		
		public static void Beep() { SafeNativeMethods.NSBeep(); }
		
		private void SetupMenuBar()
		{
			using (var pool = LocalAutoReleasePool.Create())
			{
				var servicesMenu = new MenuItem(MenuItemKind.Services);
				
				var appleMenu = new MenuItem(MenuItemKind.Application);
				#if TESTING
				appleMenu.MenuItems.Add(new MenuItem("\u26A0 Development version \u26A0") { Enabled = false } );
				appleMenu.MenuItems.Add(new SeparatorMenuItem());
				#endif
				appleMenu.MenuItems.Add(new MenuItem(Commands.About) { CommandTarget = this });
				appleMenu.MenuItems.Add(new SeparatorMenuItem());
				appleMenu.MenuItems.Add(new MenuItem(Commands.Preferences) { CommandTarget = this });
				appleMenu.MenuItems.Add(new SeparatorMenuItem());
				appleMenu.MenuItems.Add(servicesMenu);
				appleMenu.MenuItems.Add(new SeparatorMenuItem());
				appleMenu.MenuItems.Add(new MenuItem(Commands.Hide));
				appleMenu.MenuItems.Add(new MenuItem(Commands.HideOtherApplications));
				appleMenu.MenuItems.Add(new MenuItem(Commands.ShowAllApplications));
				appleMenu.MenuItems.Add(new SeparatorMenuItem());
				appleMenu.MenuItems.Add(new MenuItem(Commands.Quit));
				CustomizeApplicationMenu(appleMenu);
				
				var windowsMenu = new MenuItem(MenuItemKind.Windows);
				CustomizeWindowsMenu(windowsMenu);
				
				var helpMenu = new MenuItem(MenuItemKind.Help);
				CustomizeHelpMenu(helpMenu);
				
				var mainMenu = new Menu("MainMenu");
				mainMenu.MenuItems.Add(appleMenu);
				mainMenu.MenuItems.Add(windowsMenu);
				mainMenu.MenuItems.Add(helpMenu);
				
				// The setAppleMenu selector is not documented anymore, but strictly needed to build a correct menu
				SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("setAppleMenu:"), (appleMenu as Menu).NativePointer);
				SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("setServicesMenu:"), (servicesMenu as Menu).NativePointer);
				SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("setWindowsMenu:"), (windowsMenu as Menu).NativePointer);
				//SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("setHelpMenu:"), (helpMenu as Menu).NativePointer);
				SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("setMainMenu:"), mainMenu.NativePointer);
			}
		}
		
		protected virtual void CustomizeApplicationMenu(Menu menu) { }
		protected virtual void CustomizeWindowsMenu(Menu menu) { }
		protected virtual void CustomizeHelpMenu(Menu menu) { }
		
		public void ShowStandardAboutPanel()
		{
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("orderFrontStandardAboutPanel:"), IntPtr.Zero);
		}
		
		/// <summary>Runs an event loop.</summary>
		/// <remarks>This overload should be used for document-based applications.</remarks>
		public void Run()
		{
			// Call the Run method on the main thread
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.Selectors.PerformSelectorOnMainThreadWithObjectWaitUntilDone, ObjectiveC.GetSelector("run"), IntPtr.Zero, true);
		}
		
		/// <summary>Runs an event loop.</summary>
		/// <param name="mainWindow">A <see cref="Window"/> to be used as the main window.</param>
		/// <remarks>This overload should be used for non document-based applications.</remarks>
		public void Run(Window applicationWindow)
		{
			if (applicationWindow == null)
				throw new ArgumentNullException("applicationWindow");
			
			if (creationPool != null)
				creationPool.Dispose();
			creationPool = null;
			this.applicationWindow = applicationWindow;
			Run();
		}
		
		#region Command Methods
		
		public override bool CanExecute(Command command)
		{
			switch (command.Name)
			{
				case "Preferences":
					return PreferencesPanel != null;
				case "ShowAllApplications":
					return canShowAllApplications; // The value here may be wrong sometimes, as it's refreshed by ValidateItem
				default:
					return base.CanExecute(command);
			}
		}
		
		public override bool ValidateItem(ICommandItem item)
		{
			// Call the native Objective-C validation method for the ShowAllApplications command, as it seems there are no
			// other means of finding the statut of the command.
			if (item != null && item.Command == Commands.ShowAllApplications)
			{
				// Since the item is targeting CLR, wrap it in a NSValidatedInterfaceItem compatible object…
				if (item != null)
					using (var wrapper = ItemWrapper.Create(item))
						canShowAllApplications = SafeNativeMethods.objc_msgSendSuper_get_Boolean(ref super, CommonSelectors.ValidateUserInterfaceItem, wrapper.NativePointer);
			}
			return base.ValidateItem(item);
		}
		
		public void About(object sender)
		{
			OnAbout(EventArgs.Empty);
		}
		
		public void Preferences(object sender)
		{
			OnPreferences(EventArgs.Empty);
		}
		
		public void Hide(object sender)
		{
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("hide:"), IntPtr.Zero);
		}
		
		public void HideOtherApplications(object sender)
		{
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("hideOtherApplications:"), IntPtr.Zero);
		}
		
		public void ShowAllApplications(object sender)
		{
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("unhideAllApplications:"), IntPtr.Zero);
		}
		
		public void MiniaturizeAll(object sender)
		{
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("miniaturizeAll:"), IntPtr.Zero);
		}
		
		[Command("Quit")]
		public void Terminate(object sender)
		{
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("terminate:"), IntPtr.Zero);
		}
		
		#endregion
		
		#region Event Handlers
		
		protected virtual void OnLaunching(EventArgs e)
		{
			SetupMenuBar();
			if (applicationWindow != null)
				applicationWindow.ShowAndMakeKey();
			if (Launching != null)
				Launching(this, e);
		}
		
		protected virtual void OnLaunched(EventArgs e)
		{
			if (Launched != null)
				Launched(this, e);
		}
		
		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null)
				Activated(this, EventArgs.Empty);
		}
		
		protected virtual void OnDeactivated(EventArgs e)
		{
			if (Deactivated != null)
				Deactivated(this, EventArgs.Empty);
		}
		
		protected virtual void OnHidden(EventArgs e)
		{
			if (Hidden != null)
				Hidden(this, EventArgs.Empty);
		}
		
		protected virtual void OnShown(EventArgs e)
		{
			if (Shown != null)
				Shown(this, EventArgs.Empty);
		}
		
		protected virtual void OnTerminating(EventArgs e)
		{
			if (Terminating != null)
				Terminating(this, EventArgs.Empty);
		}
		
		protected virtual void OnNewFile(EventArgs e)
		{
			if (NewFile != null)
				NewFile(this, e);
		}
		
		protected virtual void OnAbout(EventArgs e)
		{
			ShowStandardAboutPanel();
		}
		
		protected virtual void OnPreferences(EventArgs e)
		{
			if (PreferencesPanel != null)
				PreferencesPanel.ShowAndMakeKey();
		}
		
		#endregion
	}
}
