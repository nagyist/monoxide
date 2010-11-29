using System;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	[NativeClass("NSTableView", "AppKit")]
	public class TableViewBase<TCell> : Control<TCell>
		where TCell : Cell, new()
	{
		#region ColumnCollection Class
		
		public sealed class ColumnCollection : IList<TableColumn<TCell>>
		{
			private TableViewBase<TCell> tableView;
			
			internal ColumnCollection(TableViewBase<TCell> tableView) { this.tableView = tableView; }
			
			public TableColumn<TCell> this[int index]
			{
				get
				{
					return tableView.columnList[index];
				}
				set
				{
					tableView.columnList[index] = value;
				}
			}
			
			public int Count
			{
				get
				{
					return tableView.columnList.Count;
				}
			}
			
			public bool IsReadOnly { get { return false; } }
			
			public void Add(TableColumn<TCell> item)
			{
				item.Owner = tableView;
				tableView.columnList.Add(item);
			}

			public int IndexOf(TableColumn<TCell> item)
			{
				return tableView.columnList.IndexOf(item);
			}

			public void Insert(int index, TableColumn<TCell> item)
			{
				item.Owner = tableView;
				tableView.columnList.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(TableColumn<TCell> item)
			{
				return tableView.columnList.Contains(item);
			}

			public void CopyTo(TableColumn<TCell>[] array, int arrayIndex)
			{
				tableView.columnList.CopyTo(array, arrayIndex);
			}

			public bool Remove(TableColumn<TCell> item)
			{
				throw new NotImplementedException();
			}

			public IEnumerator<TableColumn<TCell>> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}
		
		#endregion
		
		#region Method Selector Ids
		
		static class Selectors
		{
			static class addTableColumn { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("addTableColumn:"); }
			static class selectedRow { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("selectedRow"); }
			static class selectedRowIndexes { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("selectedRowIndexes"); }
			
			public static IntPtr AddTableColumn { get { return addTableColumn.SelectorHandle; } }
			public static IntPtr SelectedRow { get { return selectedRow.SelectorHandle; } }
			public static IntPtr SelectedRowIndexes { get { return selectedRowIndexes.SelectorHandle; } }
		}
		
		#endregion
		
		private List<TableColumn<TCell>> columnList;
		private ColumnCollection columnCollection;
		
		public TableViewBase()
		{
			columnList = new List<TableColumn<TCell>>(1);
			columnCollection = new ColumnCollection(this);
		}

		internal override void OnCreated()
		{
			base.OnCreated();
			foreach (var column in columnList)
				SafeNativeMethods.objc_msgSend(NativePointer, Selectors.AddTableColumn, column.NativePointer);
		}

		public ColumnCollection Columns { get { return columnCollection; } }
		
		public int SelectedRow
		{
			get
			{
				return Created ?
					checked((int)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SelectedRow)) :
					-1;
			}
		}

		protected virtual string GetItemText(object item, TableColumn<TCell> column)
		{
			return item != null ? item.ToString() : null;
		}
		
		public override object Clone()
		{
			var clone = base.Clone() as TableViewBase<TCell>;
			
			clone.columnList = new List<TableColumn<TCell>>(columnList.Count);
			clone.columnCollection = new ColumnCollection(clone);
			foreach (var column in columnList)
				clone.columnCollection.Add(column.Clone() as TableColumn<TCell>);
			
			return clone;
		}

		internal virtual void ColumnAdded(int index) { }
		internal virtual void ColumnRemoved(int index, TableColumn<Cell> oldColumn) { }
		internal virtual void ColumnChanged(int index, TableColumn<Cell> oldColumn) { }
	}
}
