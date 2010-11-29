using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSOutlineView", "AppKit")]
	public class OutlineViewBase<TCell> : TableViewBase<TCell>
		where TCell : Cell, new()
	{
		#region DataSource Management
		
		struct DataSource
		{
			[SelectorStubAttribute("outlineView:child:ofItem:", Kind = StubKind.ClassMandatory)]
			private static IntPtr ChildOfItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr index, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return IntPtr.Zero;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				return ObjectiveC.GetNativeObject(outlineViewControl.GetItemChild(@object, checked((int)index)));
			}
			
			[SelectorStubAttribute("outlineView:isItemExpandable:", Kind = StubKind.ClassMandatory)]
			private static bool IsItemExpandable(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return false;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				return outlineViewControl.IsItemExpandable(@object);
			}
			
			[SelectorStubAttribute("outlineView:numberOfChildrenOfItem:", Kind = StubKind.ClassMandatory)]
			private static IntPtr NumberOfChildrenOfItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return IntPtr.Zero;
				
				var @object = ObjectiveC.GetManagedObject(item);
				
				return checked((IntPtr)outlineViewControl.GetItemChildCount(@object));
			}
			
			[SelectorStubAttribute("outlineView:objectValueForTableColumn:byItem:", Kind = StubKind.ClassMandatory)]
			private static IntPtr GetObjectValueForTableColumnByItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr tableColumn, IntPtr item)
			{
				var outlineViewControl = View.GetInstance(outlineView) as OutlineViewBase<TCell>;
				
				if (outlineViewControl == null) return IntPtr.Zero;

				var column = TableColumn<TCell>.GetInstance(tableColumn) as TableColumn<TCell>;
				
				var @object = ObjectiveC.GetManagedObject(item);
				if (@object == null) return IntPtr.Zero;

				var text = outlineViewControl.GetItemText(@object, column);
				return text != null ? ObjectiveC.AutoReleaseObject(ObjectiveC.StringToNativeString(text)) : IntPtr.Zero;
			}
			
			[SelectorStubAttribute("outlineView:setObjectValue:forTableColumn:byItem:", Kind = StubKind.ClassMandatory)]
			private static void SetObjectValueForTableColumnByItem(IntPtr self, IntPtr _cmd, IntPtr outlineView, IntPtr @object, IntPtr tableColumn, IntPtr item)
			{
			}
		}
		
		#endregion
		
		#region Event Handling & Dispatching
		
		struct Delegate
		{
			[SelectorStub("outlineViewSelectionDidChange:", Kind = StubKind.ClassMandatory)]
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
			static class setOutlineTableColumn { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setOutlineTableColumn:"); }
			
			public static IntPtr ItemAtRow { get { return itemAtRow.SelectorHandle; } }
			public static IntPtr RowForItem { get { return rowForItem.SelectorHandle; } }
			public static IntPtr SetOutlineTableColumn { get { return setOutlineTableColumn.SelectorHandle; } }
		}
		
		#endregion
		
		public event EventHandler SelectionChanged;
		
		internal override void OnCreated()
		{
			base.OnCreated();
			if (Columns.Count > 0) SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetOutlineTableColumn, Columns[0].NativePointer);
			SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetDataSource, ObjectiveC.GetNativeClass(typeof(DataSource), true));
			SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetDelegate, ObjectiveC.GetNativeClass(typeof(Delegate), true));
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
