using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;

namespace System.MacOS.AppKit
{
	[NativeClass("NSMenuItem", "AppKit")]
	public class MenuItem : Menu, ICommandItem
	{
		#region NSMenuItem Interop
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		private static extern IntPtr objc_msgSend_initWithTitle_action_keyEquivalent(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string itemName, IntPtr anAction, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string charCode);
		
		#region Method Selector Ids
		
		private static class Selectors
		{
			static class initWithTitle_action_keyEquivalent { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithTitle:action:keyEquivalent:"); }
			static class separatorItem { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("separatorItem"); }
			
			static class submenu { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("submenu"); }
			static class setSubmenu { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setSubmenu:"); }
						
			static class keyEquivalent { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("keyEquivalent"); }
			static class setKeyEquivalent { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setKeyEquivalent:"); }
			
			static class keyEquivalentModifierMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("keyEquivalentModifierMask"); }
			static class setKeyEquivalentModifierMask { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setKeyEquivalentModifierMask:"); }
			
			public static IntPtr InitWithTitleActionKeyEquivalent { get { return initWithTitle_action_keyEquivalent.SelectorHandle; } }
			public static IntPtr SeparatorItem { get { return separatorItem.SelectorHandle; } }
			
			public static IntPtr Submenu { get { return submenu.SelectorHandle; } }
			public static IntPtr SetSubmenu { get { return setSubmenu.SelectorHandle; } }
			
			public static IntPtr KeyEquivalent { get { return keyEquivalent.SelectorHandle; } }
			public static IntPtr SetKeyEquivalent { get { return setKeyEquivalent.SelectorHandle; } }
			
			public static IntPtr KeyEquivalentModifierMask { get { return keyEquivalentModifierMask.SelectorHandle; } }
			public static IntPtr SetKeyEquivalentModifierMask { get { return setKeyEquivalentModifierMask.SelectorHandle; } }
		}
		
		#endregion
		
		#endregion

		#region Event Handling & Dispatching
		
		[SelectorStubAttribute("clrCommand:")]
		private static void HandleClick(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			Debug.Assert(self == sender);
			
			var menuItem = GetInstance(sender);
			
			if (menuItem == null) return;
			
			menuItem.HandleClick();
		}
		
		[SelectorStubAttribute("validateMenuItem:")]
		private static bool ValidateMenuItem(IntPtr self, IntPtr _cmd, IntPtr nativeMenuItem)
		{
			Debug.Assert(self == nativeMenuItem);
			
			var menuItem = GetInstance(nativeMenuItem);
			
			if (menuItem == null) return false;
			
			return menuItem.Validate();
		}
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<MenuItem> menuItemList = new NativeObjectCache<MenuItem>(m => m.NativePointer);
		
		internal static new MenuItem GetInstance(IntPtr nativePointer) { return menuItemList.GetObject(nativePointer); }
		
		#endregion
		
		private SafeNativeMethods.objc_super super;
		private MenuItemKind kind;
		private string toolTip;
		private bool enabled;
		private ShortcutKey shortcutKey;
		private Command command;
		
		public event EventHandler Click;
		
		#region Constructors
		
		public MenuItem()
			: this(MenuItemKind.Regular) { }
		
		internal MenuItem(MenuItemKind kind)
		{
			switch (kind)
			{
				case MenuItemKind.Regular:
					if (this.GetType() != typeof(MenuItem))
						kind = MenuItemKind.Custom;
					break;
				case MenuItemKind.Application:
					Title = "Apple";
					break;
				case MenuItemKind.Services:
					Title = Localization.GetMenuTitle("Services");
					break;
				case MenuItemKind.Windows:
					Title = Localization.GetMenuTitle("Windows");
					break;
				case MenuItemKind.Help:
					Title = Localization.GetMenuTitle("Help");
					break;
			}
			
			this.kind = kind;
			this.toolTip = string.Empty;
			this.enabled = kind != MenuItemKind.Separator;
		}
		
		public MenuItem(string title)
			: this(title, ShortcutKey.None) { }
		
		public MenuItem(string title, ShortcutKey shortcutKey)
			: this(title, shortcutKey, MenuItemKind.Regular) { }
		
		public MenuItem(string title, ShortcutKey shortcutKey, MenuItemKind kind)
		{
			if (kind == MenuItemKind.Regular && this.GetType() != typeof(MenuItem))
				kind = MenuItemKind.Custom;
			else if (kind == MenuItemKind.Separator)
				throw new InvalidOperationException();
			
			this.Title = title ?? string.Empty;
			this.toolTip = string.Empty;
			this.shortcutKey = shortcutKey;
			this.enabled = true;
		}
		
		public MenuItem(Command command)
			: this(command, null) { }
		
		public MenuItem(Command command, CommandTarget commandTarget)
		{
			if (command == null)
				throw new ArgumentNullException("command");
			this.kind = this.GetType() == typeof(MenuItem) ? MenuItemKind.Regular : MenuItemKind.Custom;
			this.Command = command;
			this.CommandTarget = commandTarget;
			this.Title = command.Title ?? string.Empty;
			this.toolTip = string.Empty;
			this.shortcutKey = command.ShortcutKey;
			this.enabled = true;
			command.PropertyChanged += HandleCommandPropertyChanged;
		}
		
		#endregion
		
		//~MenuItem() { Dispose(false); } // SHould not be needed anymore
		
		protected override void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				menuItemList.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
			base.Dispose(disposing); // The submenu shall be disposed after the parent menu item.
		}
		
		private void CreateNative()
		{
			if (Disposed || Created) return;
			
			using (var pool = LocalAutoReleasePool.Create())
			{
				var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
				super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
				
				if (kind == MenuItemKind.Separator)
					ObjectiveC.RetainObject(super.Receiver = SafeNativeMethods.objc_msgSend(nativeClass, Selectors.SeparatorItem));
				else
				{
					super.Receiver = objc_msgSend_initWithTitle_action_keyEquivalent(ObjectiveC.AllocObject(nativeClass), Selectors.InitWithTitleActionKeyEquivalent, Title, IntPtr.Zero, shortcutKey.Key ?? string.Empty);
					menuItemList.RegisterObject(this);
					if (shortcutKey.Key != null)
						SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetKeyEquivalentModifierMask, (IntPtr)shortcutKey.Modifiers);
					SafeNativeMethods.objc_msgSend_set_String(super.Receiver, CommonSelectors.SetToolTip, toolTip);
					SafeNativeMethods.objc_msgSend_set_Boolean(super.Receiver, CommonSelectors.SetEnabled, enabled);
					if (!IsSpecial && MenuItems.Count == 0)
					{
						SafeNativeMethods.objc_msgSendSuper(ref super, CommonSelectors.SetTarget, super.Receiver);
						SafeNativeMethods.objc_msgSendSuper(ref super, CommonSelectors.SetAction, CommonSelectors.ClrCommand);
					}
					else
						SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetSubmenu, base.NativePointer);
				}
			}
		}
		
		internal new bool Created { get { return super.Receiver != IntPtr.Zero; } }
		
		internal new IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new InvalidOperationException();
				
				if (!Created)
					CreateNative();
				
				return super.Receiver;
			}
		}
		
		public MenuItemKind Kind { get { return kind; } }
		
		public bool IsSpecial
		{
			get
			{
				return kind == MenuItemKind.Application
					|| kind == MenuItemKind.Services
					|| kind == MenuItemKind.Windows
					|| kind == MenuItemKind.Help;
			}
		}
		
		protected override bool CanHaveMenuItems { get { return kind != MenuItemKind.Separator && kind != MenuItemKind.Services; } }
		
		public Menu Parent { get { return Owner as Menu; } }
		
		public string ToolTip
		{
			get { return toolTip; }
			set
			{
				if (kind == MenuItemKind.Separator)
					throw new InvalidOperationException();
				
				if (value != toolTip)
				{
					toolTip = value ?? string.Empty;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetToolTip, toolTip);
				}
			}
		}
		
		public ShortcutKey ShortcutKey
		{
			get { return shortcutKey; }
			set
			{
				if (kind == MenuItemKind.Separator)
					throw new InvalidOperationException();
				
				if (value.Key != shortcutKey.Key)
					SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetKeyEquivalent, shortcutKey.Key ?? string.Empty);
				if (value.Modifiers != shortcutKey.Modifiers)
					SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetKeyEquivalentModifierMask, (IntPtr)shortcutKey.Modifiers);
				
				shortcutKey = value;
			}
		}
		
		public Image Image { get; set; }
		
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (kind == MenuItemKind.Separator)
					throw new InvalidOperationException();
				
				enabled = value;
			}
		}
		
		public Command Command
		{
			get { return command; }
			set
			{
				if (value == command) return;
				if (command != null) command.PropertyChanged -= HandleCommandPropertyChanged;
				if ((command = value) != null) command.PropertyChanged += HandleCommandPropertyChanged;
			}
		}
		
		public CommandTarget CommandTarget { get; set; }
		
		public object Tag { get; set; }
		
		internal override void OnTitleChangedInternal ()
		{
			base.OnTitleChangedInternal();
			if (Created)
				SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetTitle, Title);
		}
		
		private void HandleCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var command = sender as Command;
			
			switch (e.PropertyName)
			{
				case "Title":
					Title = command.Title;
					break;
				case "Description":
					ToolTip = command.Description;
					break;
				case "Image":
					Image = command.Image;
					break;
				case "ShortcutKey":
					ShortcutKey = command.ShortcutKey;
					break;
			}
		}
		
		private void HandleClick()
		{
			OnClick(EventArgs.Empty);
		}
		
		private bool Validate()
		{
			if (Command != null)
			{
				var target = CommandTarget ?? Application.Current.GetTargetForCommand(Command);
				
				return target != null ? target.ValidateItem(this) : false;
			}
			else
				return enabled/* && Click != null*/;
		}
		
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
			
			if (Command != null)
			{
				var target = CommandTarget ?? Application.Current.GetTargetForCommand(Command);
				
				if (target != null)
					target.Execute(Command, this);
			}
		}
		
		public override object Clone()
		{
			var clone = base.Clone() as MenuItem;
			
			super = new SafeNativeMethods.objc_super();
			//parent = null;
			
			return clone;
		}
	}
}
