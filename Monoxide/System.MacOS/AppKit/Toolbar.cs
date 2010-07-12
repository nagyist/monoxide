using System;
using System.Diagnostics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSToolbar", "AppKit")]
	public class Toolbar : CommandTarget
	{
		#region Cache
		
		private static readonly NativeObjectCache<Toolbar> toolbarCache = new NativeObjectCache<Toolbar>(t => t.NativePointer);
		
		internal static Toolbar GetInstance(IntPtr nativePointer) { return toolbarCache.GetObject(nativePointer); }
		
		#endregion
		
		#region Method Selector Ids
		
		static class Selectors
		{
			static class initWithIdentifier { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithIdentifier:"); }
			
			static class identifier { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("identifier"); }
			
			static class displayMode { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("displayMode"); }
			static class setDisplayMode { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDisplayMode:"); }
			
			static class allowsUserCustomization { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("allowsUserCustomization"); }
			static class setAllowsUserCustomization { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAllowsUserCustomization:"); }
			
			public static IntPtr InitWithIdentifier { get { return initWithIdentifier.SelectorHandle; } }
			
			public static IntPtr Identifier { get { return identifier.SelectorHandle; } }
			
			public static IntPtr DisplayMode { get { return displayMode.SelectorHandle; } }
			public static IntPtr SetDisplayMode { get { return setDisplayMode.SelectorHandle; } }
			
			public static IntPtr AllowsUserCustomization { get { return allowsUserCustomization.SelectorHandle; } }
			public static IntPtr SetAllowsUserCustomization { get { return setAllowsUserCustomization.SelectorHandle; } }
		}
		
		#endregion
		
		private static readonly IntPtr ItemKey = ObjectiveC.StringToNativeString("item");
		
		private static ToolbarTemplate GetTemplate(IntPtr toolbar)
		{
			var templateName = ObjectiveC.NativeStringToString(SafeNativeMethods.objc_msgSend(toolbar, Selectors.Identifier));
			
			if (templateName.StartsWith("palette for ", StringComparison.Ordinal))
				templateName = templateName.Substring(12);
			else if (templateName.StartsWith("default palette for ", StringComparison.Ordinal))
				templateName = templateName.Substring(20);
			
			return ToolbarTemplate.Get(templateName);
		}
		
		[SelectorStubAttribute("toolbar:itemForItemIdentifier:willBeInsertedIntoToolbar:")]
		private static IntPtr GetItemForIdentifier(IntPtr self, IntPtr _cmd, IntPtr toolbar, IntPtr itemIdentifier, bool flag)
		{
			var template = GetTemplate(toolbar);
			
			if (template == null) return IntPtr.Zero;
			
			var name = ObjectiveC.NativeStringToString(itemIdentifier);
			
			if (name.StartsWith("CLR", StringComparison.Ordinal))
				name = name.Substring(3);
			else
				return IntPtr.Zero;
			
			ToolbarItem item;
			
			if (template.TryGetItem(name, out item))
				return (item.Clone() as ToolbarItem).Retain();
			else
				return IntPtr.Zero;
		}
		
		[SelectorStubAttribute("toolbarAllowedItemIdentifiers:")]
		private static IntPtr GetAllowedItemIdentifiers(IntPtr self, IntPtr _cmd, IntPtr toolbar)
		{
			var template = GetTemplate(toolbar);
			
			if (template == null) return IntPtr.Zero;
			
			var items = template.Items.ToArray();
			var itemNames = new IntPtr[items.Length];
			
			for (int i = 0; i < itemNames.Length; i++)
				itemNames[i] = items[i].NativeName;
			
			return ObjectiveC.ArrayToNativeArray(itemNames);
		}
		
		[SelectorStubAttribute("toolbarDefaultItemIdentifiers:")]
		private static IntPtr GetDefaultItemIdentifiers(IntPtr self, IntPtr _cmd, IntPtr toolbar)
		{
			var template = GetTemplate(toolbar);
			
			if (template == null) return IntPtr.Zero;
			
			var items = template.DefaultItems.ToArray();
			var itemNames = new IntPtr[items.Length];
			
			for (int i = 0; i < itemNames.Length; i++)
				itemNames[i] = items[i].NativeName;
			
			return ObjectiveC.ArrayToNativeArray(itemNames);
		}
		
		[SelectorStubAttribute("toolbarSelectableItemIdentifiers:")]
		private static IntPtr GetSelectableItemIdentifiers(IntPtr self, IntPtr _cmd, IntPtr toolbar)
		{
			return ObjectiveC.ArrayToNativeArray(new IntPtr[0]);
		}
		
		[SelectorStubAttribute("toolbarWillAddItem:")]
		[SelectorStubAttribute("toolbarDidRemoveItem:")]
		private static void HandleNotification(IntPtr self, IntPtr _cmd, IntPtr aNotification)
		{
			var notificationName = ObjectiveC.GetNotificationName(aNotification);
			var toolbar = GetInstance(ObjectiveC.GetNotificationObject(aNotification));
			var item = ToolbarItem.GetInstance(SafeNativeMethods.objc_msgSend(ObjectiveC.GetNotificationUserInfo(aNotification), ObjectiveC.Selectors.ObjectForKey, ItemKey));
				
			Debug.WriteLine("Notification received: " + notificationName);
			
			switch (notificationName)
			{
				case "NSToolbarWillAddItemNotification": toolbar.HandleItemAdding(item); break;
				case "NSToolbarDidRemoveItemNotification": toolbar.HandleItemRemoved(item); break;
			}
		}
		
		bool customizable;
		ToolbarDisplayStyle displayStyle;
		string templateName;
		
		public Toolbar()
		{
			templateName = Guid.NewGuid().ToString();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				toolbarCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
		}
		
		private void CreateNative()
		{
			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
			super.Receiver = SafeNativeMethods.objc_msgSend_get_ObjectFromKey(ObjectiveC.AllocObject(nativeClass), Selectors.InitWithIdentifier, templateName);
			toolbarCache.RegisterObject(this);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetAllowsUserCustomization, customizable);
			SafeNativeMethods.objc_msgSend(super.Receiver, CommonSelectors.SetDelegate, super.Receiver);
		}
		
		internal override IntPtr NativePointer
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
		
		public ToolbarDisplayStyle DisplayStyle
		{
			get
			{
				return displayStyle;
			}
			set
			{
				displayStyle = value;
			}
		}
		
		public string TemplateName
		{
			get { return templateName; }
			set
			{
				if (Created)
					throw new InvalidOperationException();
				
				if (value == null)
					throw new ArgumentNullException("value");
				
				templateName = value;
			}
		}
		
		public bool Customizable
		{
			get
			{
				return customizable;
			}
			set
			{
				if (value != customizable)
				{
					customizable = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetAllowsUserCustomization, customizable);
				}
			}
		}
		
		private void HandleItemAdding(ToolbarItem item)
		{
		}
		
		private void HandleItemRemoved(ToolbarItem item)
		{
		}
	}
}
