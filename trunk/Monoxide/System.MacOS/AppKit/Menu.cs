using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSMenu", "AppKit")]
	public class Menu : ICloneable, IDisposable
	{
		#region MenuItemCollection Class
		
		public sealed class MenuItemCollection : IList<MenuItem>
		{
			private readonly Menu menu;
			
			internal MenuItemCollection(Menu menu) { this.menu = menu; }
			
			public MenuItem this[int index]
			{
				get { return menu.menuItemList[index]; }
				set { throw new System.NotImplementedException(); }
			}
			
			public int Count { get { return menu.menuItemList.Count; } }
			
			public bool IsReadOnly { get { return !menu.CanHaveMenuItems; } }
			
			public void Add(MenuItem item)
			{
				if (!menu.CanHaveMenuItems)
					throw new InvalidOperationException();
				if (item == null)
					throw new ArgumentNullException("item");
				
				item.Owner = menu;
				
				menu.menuItemList.Add(item);
				if (menu.Created)
				{
					// Adjusts the index based on the previous menu item position.
					// This preserves the managed MenuItem ordering, even if some native component modified the menu.
					int index = menu.menuItemList.Count - 1;
					
					if (index > 0)
						index = checked((int)SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.IndexOfItem, menu.menuItemList[index - 1].NativePointer) + 1);
					SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.InsertItemAtIndex, item.NativePointer, checked((IntPtr)index));
				}
			}
			
			public void Insert(int index, MenuItem item)
			{
				if (!menu.CanHaveMenuItems)
					throw new InvalidOperationException();
				if (item == null)
					throw new ArgumentNullException("item");
				if (item.Disposed)
					throw new ObjectDisposedException("item");
				
				item.Owner = menu;
				
				menu.menuItemList.Insert(index, item);
				if (menu.Created)
				{
					// Adjusts the index based on the previous menu item position.
					// This preserves the managed MenuItem ordering, even if some native component modified the menu.
					if (index > 0)
						index = checked((int)SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.IndexOfItem, menu.menuItemList[index - 1].NativePointer) + 1);
					SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.InsertItemAtIndex, item.NativePointer, checked((IntPtr)index));
				}
			}
			
			public bool Remove(MenuItem item)
			{
				if (menu.menuItemList.Remove(item))
				{
					if (menu.Created)
						SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.RemoveItem, item.NativePointer);
					item.Owner = null;
					return true;
				}
				else
					return false;
			}
			
			public void RemoveAt(int index)
			{
				var menuItem = menu.menuItemList[index];
				
				menu.menuItemList.RemoveAt(index);
				if (menu.Created)
					SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.RemoveItem, menuItem.NativePointer);
				menuItem.Owner = null;
			}
			
			public void Clear()
			{
				foreach (var menuItem in menu.menuItemList)
				{
					if (menu.Created)
						SafeNativeMethods.objc_msgSend(menu.NativePointer, Selectors.RemoveItem, menuItem.NativePointer);
					menuItem.Owner = null;
				}
				menu.menuItemList.Clear();
			}
			
			public bool Contains(MenuItem item) { return IndexOf(item) >= 0; }
			
			public int IndexOf(MenuItem item)
			{
				return menu.menuItemList.IndexOf(item);
			}
			
			public void CopyTo(MenuItem[] array, int arrayIndex) { menu.menuItemList.CopyTo(array, arrayIndex); }
			
			public List<MenuItem>.Enumerator GetEnumerator() { return menu.menuItemList.GetEnumerator(); }	
			IEnumerator<MenuItem> IEnumerable<MenuItem>.GetEnumerator() { return menu.menuItemList.GetEnumerator(); }
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return menu.menuItemList.GetEnumerator(); }
		}
		
		#endregion
		
		#region NSMenu Interop
		
		#region Method Selector Ids
		
		static class Selectors
		{
			static class initWithTitle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithTitle:"); }
			
			static class menuBarVisible { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("menuBarVisible"); }
			static class setMenuBarVisible { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setMenuBarVisible:"); }
			
			static class addItem { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("addItem:"); }
			static class insertItemAtIndex { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("insertItem:atIndex:"); }
			static class indexOfItem { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("indexOfItem:"); }
			static class itemAtIndex { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("itemAtIndex:"); }
			static class numberOfItems { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("numberOfItems"); }
			static class removeItem { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("removeItem:"); }
			static class removeItemAtIndex { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("removeItemAtIndex:"); }
			static class removeAllItems { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("removeAllItems"); }
			
			public static IntPtr InitWithTitle { get { return initWithTitle.SelectorHandle; } }
			
			public static IntPtr MenuBarVisible { get { return menuBarVisible.SelectorHandle; } }
			public static IntPtr SetMenuBarVisible { get { return setMenuBarVisible.SelectorHandle; } }
			
			public static IntPtr AddItem { get { return addItem.SelectorHandle; } }
			public static IntPtr InsertItemAtIndex { get { return insertItemAtIndex.SelectorHandle; } }
			public static IntPtr IndexOfItem { get { return indexOfItem.SelectorHandle; } }
			public static IntPtr ItemAtIndex { get { return itemAtIndex.SelectorHandle; } }
			public static IntPtr NumberOfItems { get { return numberOfItems.SelectorHandle; } }
			public static IntPtr RemoveItem { get { return removeItem.SelectorHandle; } }
			public static IntPtr RemoveItemAtIndex { get { return removeItemAtIndex.SelectorHandle; } }
			public static IntPtr RemoveAllItems { get { return removeAllItems.SelectorHandle; } }
		}
		
		#endregion
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<Menu> menuCache = new NativeObjectCache<Menu>(m => m.NativePointer);
		
		internal static Menu GetInstance(IntPtr nativePointer) { return menuCache.GetObject(nativePointer); }
		
		#endregion
		
		public static bool MenuBarVisible
		{
			get { return SafeNativeMethods.objc_msgSend_get_Boolean(CommonClasses.NSMenu, Selectors.MenuBarVisible); }
			set { SafeNativeMethods.objc_msgSend_set_Boolean(CommonClasses.NSMenu, Selectors.SetMenuBarVisible, value); }
		}
		
		private SafeNativeMethods.objc_super super;
		private List<MenuItem> menuItemList;
		private MenuItemCollection itemCollection;
		private bool disposed;
		private object owner;
		private string title;
		
		public Menu()
			: this(null) { }
		
		public Menu(string title)
		{
			menuItemList = new List<MenuItem>();
			itemCollection = new Menu.MenuItemCollection(this);
			this.title = title ?? string.Empty;
		}
		
		~Menu() { Dispose(false); disposed = true; }
		
		protected virtual void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				if (!(this is MenuItem))
					menuCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			disposed = true;
		}
		
		private void CreateNative()
		{
			if (disposed || Created) return;
			
			using (var pool = LocalAutoReleasePool.Create())
			{
				var nativeClass = ObjectiveC.GetNativeClass(this is MenuItem ? typeof(Menu) : this.GetType(), true);
				super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
				var nativePointer = SafeNativeMethods.objc_msgSend_get_ObjectFromKey(ObjectiveC.AllocObject(nativeClass), Selectors.InitWithTitle, title);
				// The menu is empty after creation, so out menu items will be placed at the top.
				foreach (var menuItem in menuItemList)
					SafeNativeMethods.objc_msgSend(nativePointer, Selectors.AddItem, menuItem.NativePointer);
				super.Receiver = nativePointer;
			}
			
			if (!(this is MenuItem))
				menuCache.RegisterObject(this);
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal bool Created { get { return super.Receiver != IntPtr.Zero; } }
		
		internal IntPtr NativePointer
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
		
		protected virtual bool CanHaveMenuItems { get { return true; } }
		
		public bool Owned { get { return owner != null; } }
		
		internal object Owner
		{
			get { return owner; }
			set
			{
				if (value != null && owner != null)
					throw new InvalidOperationException(Localization.GetExceptionText("MenuItemOwned"));
				
				owner = value;
			}
		}
		
		public string Title
		{
			get { return title; }
			set
			{
				if (value != title)
				{
					title = value ?? string.Empty;
					
					OnTitleChangedInternal();
				}
			}
		}
		
		public MenuItemCollection MenuItems { get { return itemCollection; } }
		
		internal virtual void OnTitleChangedInternal()
		{
			if (Created)
				SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetTitle, Title);
		}
		
		public virtual object Clone()
		{
			if (Disposed)
				throw new ObjectDisposedException(GetType().Name);
			
			var clone = MemberwiseClone() as Menu;
			
			clone.super = new SafeNativeMethods.objc_super();
			clone.menuItemList = new List<MenuItem>(menuItemList.Count);
			foreach (var item in menuItemList)
				clone.menuItemList.Add(item.Clone() as MenuItem);
			clone.itemCollection = new Menu.MenuItemCollection(clone);
			
			return clone;
		}
	}
}
