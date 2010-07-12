using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.MacOS.AppKit
{
	[NativeClass("NSToolbarItem", "AppKit")]
	public class ToolbarItem : ICommandItem, ICloneable, IDisposable
	{
		#region Selectors
		
		internal static class Selectors
		{
			static class initWithItemIdentifier { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithItemIdentifier:"); }
			
			static class label { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("label"); }
			static class setLabel { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setLabel:"); }
			
			static class paletteLabel { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("paletteLabel"); }
			static class setPaletteLabel { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setPaletteLabel:"); }
			
			static class view { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("view"); }
			static class setView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setView:"); }
			
			public static IntPtr InitWithItemIdentifier { get { return initWithItemIdentifier.SelectorHandle; } }
			
			public static IntPtr Label { get { return label.SelectorHandle; } }
			public static IntPtr SetLabel { get { return setLabel.SelectorHandle; } }
			
			public static IntPtr PaletteLabel { get { return paletteLabel.SelectorHandle; } }
			public static IntPtr SetPaletteLabel { get { return setPaletteLabel.SelectorHandle; } }
			
			public static IntPtr View { get { return view.SelectorHandle; } }
			public static IntPtr SetView { get { return setView.SelectorHandle; } }
		}
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<ToolbarItem> itemCache = new NativeObjectCache<ToolbarItem>(i => i.NativePointer, p => new ToolbarItem(p));
		
		internal static ToolbarItem GetInstance(IntPtr nativePointer) { return itemCache.GetObject(nativePointer); }
		
		#endregion
		
		#region Event Handling & Dispatching
		
		[SelectorStubAttribute("clrCommand:")]
		private static void HandleClick(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			Debug.Assert(self == sender);
			
			var item = GetInstance(sender);
			
			if (item == null) return;
			
			item.HandleClick();
		}
		
		[SelectorStubAttribute("validateUserInterfaceItem:")]
		private static bool ValidateItem(IntPtr self, IntPtr _cmd, IntPtr nativeItem)
		{
			Debug.Assert(self == nativeItem);
			
			var item = GetInstance(nativeItem);
			
			if (item == null) return false;
			
			return item.Validate();
		}
		
		#endregion
		
		#region NSToolbarItem Interop
		
		internal static readonly IntPtr NSToolbarSeparatorItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarSeparatorItemIdentifier"));
		internal static readonly IntPtr NSToolbarSpaceItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarSpaceItemIdentifier"));
		internal static readonly IntPtr NSToolbarFlexibleSpaceItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarFlexibleSpaceItemIdentifier"));
		internal static readonly IntPtr NSToolbarShowColorsItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarShowColorsItemIdentifier"));
		internal static readonly IntPtr NSToolbarShowFontsItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarShowFontsItemIdentifier"));
		internal static readonly IntPtr NSToolbarPrintItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarPrintItemIdentifier"));
		internal static readonly IntPtr NSToolbarCustomizeToolbarItemIdentifier = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSToolbarCustomizeToolbarItemIdentifier"));
		
		[SelectorStubAttribute("copyWithZone:")]
		private static IntPtr Clone(IntPtr self, IntPtr _cmd, IntPtr zone)
		{
			var item = GetInstance(self);
			
			if (item == null)
			{
				SafeNativeMethods.objc_super super;
				
				super.Class = CommonClasses.NSToolbarItem;
				super.Receiver = self;
				
				return SafeNativeMethods.objc_msgSendSuper(ref super, ObjectiveC.Selectors.CopyWithZone, zone);
			}
			
			return (item.Clone() as ToolbarItem).NativePointer;
		}
			
		[SelectorStubAttribute("allowsDuplicatesInToolbar")]
		private static bool AllowMultiple(IntPtr self, IntPtr _cmd)
		{
			return false;
		}
		
		#endregion
		
		public static bool IsValidName(string name) { return Casing.IsPascalCased(name); }
		
		public static readonly ToolbarItem SeparatorToolbarItem = new ImageToolbarItem("SeparatorToolbarItem", NSToolbarSeparatorItemIdentifier);
		public static readonly ToolbarItem SpaceToolbarItem = new ImageToolbarItem("SpaceToolbarItem", NSToolbarSpaceItemIdentifier);
		public static readonly ToolbarItem FlexibleSpaceToolbarItem = new ImageToolbarItem("FlexibleSpaceToolbarItem", NSToolbarFlexibleSpaceItemIdentifier);
		public static readonly ToolbarItem ColorsToolbarItem = new ImageToolbarItem("ColorsToolbarItem", NSToolbarShowColorsItemIdentifier);
		public static readonly ToolbarItem FontsToolbarItem = new ImageToolbarItem("FontsToolbarItem", NSToolbarShowFontsItemIdentifier);
		public static readonly ToolbarItem PrintToolbarItem = new ImageToolbarItem("PrintToolbarItem", NSToolbarPrintItemIdentifier);
		public static readonly ToolbarItem CustomizeToolbarItem = new ImageToolbarItem("CustomizeToolbarItem", NSToolbarCustomizeToolbarItemIdentifier);
		
		private SafeNativeMethods.objc_super super;
		private bool retained;
		private bool disposed;
		private bool enabled;
		private bool unique;
		private bool selectable;
		private int usageCount;
		private string name;
		private string label;
		private string paletteLabel;
		private IntPtr nativeName;
		private Command command;
		
		public event EventHandler Click;
		
		internal ToolbarItem() { enabled = true; }
		
		internal ToolbarItem(string name)
		{
			this.enabled = true;
			this.Name = name;
		}
		
		internal ToolbarItem(string name, IntPtr nativeName)
		{
			this.enabled = true;
			this.name = name;
			this.nativeName = ObjectiveC.RetainObject(nativeName);
		}
		
		private ToolbarItem(IntPtr nativePointer)
		{
			super.Class = ObjectiveC.GetNativeBaseClass(ObjectiveC.GetNativeClass(GetType(), false));
			super.Receiver = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
			retained = true;
		}
		
		~ToolbarItem()
		{
			Dispose(false);
			disposed = true;
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				itemCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
			if (nativeName != IntPtr.Zero)
			{
				ObjectiveC.ReleaseObject(nativeName);
				nativeName = IntPtr.Zero;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			disposed = true;
			GC.SuppressFinalize(this);
		}
		
		internal IntPtr Retain()
		{
			if (!retained)
			{
				retained = true;
				return ObjectiveC.RetainObject(NativePointer);
			}
			return NativePointer;
		}
		
		private void CreateNative()
		{
			if (usageCount != 0)
				throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemConsistency"));
			
			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
			super.Receiver = SafeNativeMethods.objc_msgSend(ObjectiveC.AllocObject(nativeClass), Selectors.InitWithItemIdentifier, NativeName);
			usageCount = -1;
			itemCache.RegisterObject(this);
			SafeNativeMethods.objc_msgSend_set_String(super.Receiver, Selectors.SetLabel, label);
			SafeNativeMethods.objc_msgSend_set_String(super.Receiver, Selectors.SetPaletteLabel, paletteLabel);
			SafeNativeMethods.objc_msgSend(super.Receiver, CommonSelectors.SetTarget, super.Receiver);
			SafeNativeMethods.objc_msgSend(super.Receiver, CommonSelectors.SetAction, CommonSelectors.ClrCommand);
			OnCreated();
		}
		
		internal virtual void OnCreated() { }
		
		public bool Disposed { get { return disposed; } }
		
		internal bool Created { get { return super.Receiver != IntPtr.Zero; } }
		
		internal IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);
				
				if (!Created)
					CreateNative();
				
				return super.Receiver;
			}
		}
		
		public string Name
		{
			get { return name; }
			set
			{
				if (usageCount != 0 || nativeName != IntPtr.Zero)
					throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemInUse"));
				if (!IsValidName(value))
					throw new NameFormatException(value, Localization.GetExceptionText("ToolbarItemNameFormat", value));
				
				name = value;
			}
		}
		
		internal IntPtr NativeName
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(GetType().Name);
				
				if (nativeName == IntPtr.Zero)
					nativeName = ObjectiveC.StringToNativeString("CLR" + name);
				
				return nativeName;
			}
		}
		
		public string Label
		{
			get { return label; }
			set
			{
				if (value != label)
				{
					bool duplicate = label == paletteLabel;
					
					label = value;
					if (duplicate)
						paletteLabel = value;
					
					if (Created)
					{
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetLabel, label);
						if (duplicate)
							SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetPaletteLabel, paletteLabel);
					}
				}
			}
		}
		
		public string PaletteLabel
		{
			get { return paletteLabel; }
			set
			{
				if (value != paletteLabel)
				{
					paletteLabel = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetPaletteLabel, paletteLabel);
				}
			}
		}
		
		public bool IsTemplate { get { return usageCount >= 0; } }
		
		public Command Command
		{
			get { return command; }
			set
			{
				command = value;
			}
		}
		
		public CommandTarget CommandTarget { get; set; }
		public object Tag { get; set; }
		
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (value != enabled)
				{
					enabled = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, CommonSelectors.SetEnabled, enabled);
				}
			}
		}
		
		public bool Selectable
		{
			get { return selectable; }
			set
			{
				if (usageCount != 0)
					throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemInUse"));
				
				selectable = value;
			}
		}
		
		public bool Unique
		{
			get { return unique; }
			set
			{
				if (usageCount != 0)
					throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemInUse"));
				
				unique = value;
			}
		}
		
		internal void AddedToTemplate()
		{
			if (usageCount < 0)
				throw new InvalidOperationException(Localization.GetExceptionText("NotTemplateToolbarItem"));
			usageCount++;
		}
		
		internal void RemovedFromTemplate()
		{
			if (usageCount < 0)
				throw new InvalidOperationException(Localization.GetExceptionText("NotTemplateToolbarItem"));
			else if (usageCount == 0)
				throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemConsistency"));
			usageCount--;
		}
		
		private void HandleClick() { OnClick(EventArgs.Empty); }
		
		private bool Validate()
		{
			if (Command != null)
			{
				var target = CommandTarget ?? Application.Current.GetTargetForCommand(Command);
				
				return target != null ? target.ValidateItem(this) : false;
			}
			else
				return enabled;
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
		
		public virtual object Clone()
		{
			if (Disposed)
				throw new ObjectDisposedException(GetType().Name);
			
			var clone = MemberwiseClone() as ToolbarItem;
			
			clone.retained = false;
			clone.usageCount = 0;
			clone.super.Receiver = IntPtr.Zero;
			// This will work even if nativeName is 0 (standard Objective-C feature)
			ObjectiveC.RetainObject(clone.nativeName);
			
			return clone;
		}
	}
}
