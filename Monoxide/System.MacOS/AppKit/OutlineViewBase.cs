using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSOutlineView", "AppKit")]
	public class OutlineViewBase<TCell> : TableViewBase<TCell>
		where TCell : Cell, new()
	{
		#region DataSource Management
		
		static class DataSource
		{
			public static readonly IntPtr NativePointer = CreateOutlineViewDataSource();
			
			delegate IntPtr ChildOfItemDelegate(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr index, IntPtr item);
			delegate bool IsItemExpandableDelegate(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr item);
			delegate IntPtr NumberOfChildrenOfItemDelegate(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr item);
			delegate IntPtr ObjectValueForTableColumnByItemDelegate(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr tableColumn, IntPtr item);
			delegate void SetObjectValueForTableColumnByItemDelegate(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr @object, IntPtr tableColumn, IntPtr item);
			
			private static IntPtr CreateOutlineViewDataSource()
			{
				// Documentation here: http://developer.apple.com/mac/library/documentation/Cocoa/Reference/ObjCRuntimeRef/Reference/reference.html#//apple_ref/c/func/objc_allocateClassPair
				var dataSourceClass = SafeNativeMethods.objc_allocateClassPair(ObjectiveC.Classes.NSObject, "CLROutlineViewDataSource", IntPtr.Zero);
				// Add a method to the runtime-created class for each of the delegated event we want to intercept
				SafeNativeMethods.class_addMethod(dataSourceClass, ObjectiveC.GetSelector("outlineView:child:ofItem:"), (ChildOfItemDelegate)ChildOfItem, ObjectiveC.LP64 ? "@@:@L@" : "@@:@I@");
				SafeNativeMethods.class_addMethod(dataSourceClass, ObjectiveC.GetSelector("outlineView:isItemExpandable:"), (IsItemExpandableDelegate)IsItemExpandable, "c@:@@");
				SafeNativeMethods.class_addMethod(dataSourceClass, ObjectiveC.GetSelector("outlineView:numberOfChildrenOfItem:"), (NumberOfChildrenOfItemDelegate)NumberOfChildrenOfItem, ObjectiveC.LP64 ? "L@:@@" : "I@:@@");
				SafeNativeMethods.class_addMethod(dataSourceClass, ObjectiveC.GetSelector("outlineView:objectValueForTableColumn:byItem:"), (ObjectValueForTableColumnByItemDelegate)GetObjectValueForTableColumnByItem, "@@:@@@");
				SafeNativeMethods.class_addMethod(dataSourceClass, ObjectiveC.GetSelector("outlineView:setObjectValue:forTableColumn:byItem:"), (SetObjectValueForTableColumnByItemDelegate)SetObjectValueForTableColumnByItem, "v@:@@@@");
				// Register the newly created class
				SafeNativeMethods.objc_registerClassPair(dataSourceClass);
				// Return an allocated and initialized a instance of our newly created delegate class
				return SafeNativeMethods.objc_msgSend(ObjectiveC.AllocObject(dataSourceClass), ObjectiveC.Selectors.Init);
			}
			
			private static IntPtr ChildOfItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr index, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return IntPtr.Zero;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				return ObjectiveC.GetNativeObject(outlineViewControl.GetItemChild(@object, checked((int)index)));
			}
			
			private static bool IsItemExpandable(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return false;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				return outlineViewControl.IsItemExpandable(@object);
			}
			
			private static IntPtr NumberOfChildrenOfItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return IntPtr.Zero;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				return checked((IntPtr)outlineViewControl.GetItemChildCount(@object));
			}
			
			private static IntPtr GetObjectValueForTableColumnByItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr tableColumn, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return IntPtr.Zero;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				if (@object == null) return IntPtr.Zero;
				
				return ObjectiveC.AutoReleaseObject(ObjectiveC.StringToNativeString(outlineViewControl.GetItemText(@object)));
			}
			
			private static void SetObjectValueForTableColumnByItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr @object, IntPtr tableColumn, IntPtr item)
			{
			}
		}
		
		#endregion
		
		#region Event Handling & Dispatching
		
		static class Delegate
		{
			public static readonly IntPtr NativePointer = CreateOutlineViewDelegate();
			
			private static IntPtr CreateOutlineViewDelegate()
			{
				// Standard notification handler, that will handle most notification and dispatch them apropriately
				var notificationHandler = (SafeNativeMethods.StandardEventHandler)HandleNotification;
				// Documentation here: http://developer.apple.com/mac/library/documentation/Cocoa/Reference/ObjCRuntimeRef/Reference/reference.html#//apple_ref/c/func/objc_allocateClassPair
				var delegateClass = SafeNativeMethods.objc_allocateClassPair(ObjectiveC.Classes.NSObject, "CLROutlineViewDelegate", IntPtr.Zero);
				// Add a method to the runtime-created class for each of the delegated event we want to intercept
				SafeNativeMethods.class_addMethod(delegateClass, ObjectiveC.GetSelector("outlineViewSelectionDidChange:"), notificationHandler, "v@:@");
				// Register the newly created class
				SafeNativeMethods.objc_registerClassPair(delegateClass);
				// Return an allocated and initialized a instance of our newly created delegate class
				return SafeNativeMethods.objc_msgSend(ObjectiveC.AllocObject(delegateClass), ObjectiveC.Selectors.Init);
			}
			
			private static void HandleNotification(IntPtr self, IntPtr _cmd, IntPtr aNotification)
			{
				var outlineView = GetInstance(ObjectiveC.GetNotificationObject(aNotification)) as OutlineViewBase<TCell>;
				
				if (outlineView == null) return;
				
				switch (ObjectiveC.GetNotificationName(aNotification))
				{
					case "NSOutlineViewSelectionDidChangeNotification": outlineView.HandleSelectionChanged(); break;
				}
			}
		}
		
		#endregion
		
		#region Method Selector Ids
		
		static class Selectors
		{
			static class itemAtRow { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("itemAtRow:"); }
			static class rowForItem { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("rowForItem:"); }
			
			public static IntPtr ItemAtRow { get { return itemAtRow.SelectorHandle; } }
			public static IntPtr RowForItem { get { return rowForItem.SelectorHandle; } }
		}
		
		#endregion
		
		public event EventHandler SelectionChanged;
		
		internal override void OnCreated()
		{
			base.OnCreated();
			var column = SafeNativeMethods.objc_msgSend(ObjectiveC.AllocObject("NSTableColumn"), ObjectiveC.GetSelector("initWithIdentifier:"), IntPtr.Zero);
			SafeNativeMethods.objc_msgSend_set_String(SafeNativeMethods.objc_msgSend(column, ObjectiveC.GetSelector("headerCell")), ObjectiveC.GetSelector("setStringValue:"), "Topic");
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("addTableColumn:"), column);
			SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("setOutlineTableColumn:"), column);
			SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetDataSource, DataSource.NativePointer);
			SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetDelegate, Delegate.NativePointer);
		}
		
		private void HandleSelectionChanged()
		{
			OnSelectionChanged(EventArgs.Empty);
		}
		
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}
		
		protected virtual int GetItemChildCount(object item)
		{
			return 0;
		}
		
		protected virtual object GetItemChild(object item, int index)
		{
			return null;
		}
		
		protected virtual bool IsItemExpandable(object item)
		{
			return false;
		}
		
		protected virtual bool IsGroupItem(object item)
		{
			return false;
		}
		
		protected virtual string GetItemText(object item)
		{
			return item != null ? item.ToString() : null;
		}
		
		public object GetRowItem(int row)
		{
			if (!Created)
				throw new InvalidOperationException();
			
			return ObjectiveC.GetManagedObject(SafeNativeMethods.objc_msgSend(NativePointer, Selectors.ItemAtRow, checked((IntPtr)row)));
		}
		
		public int GetItemRow(object item)
		{
			if (!Created)
				throw new InvalidOperationException();
			
			return checked((int)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.RowForItem, ObjectiveC.GetNativeObject(item)));
		}
		
		public object SelectedItem
		{
			get
			{
				int row = SelectedRow;
				
				return row >= 0 ? GetRowItem(row) : null;
			}
		}
	}
}
