using System;

namespace System.MacOS.AppKit
{
	public sealed class TableColumn : TableColumn<TextFieldCell, TableHeaderCell> { }

	[NativeClass("NSTableColumn", "AppKit")]
	public class TableColumn<TDataCell, THeaderCell> : TableColumn<TDataCell>
		where TDataCell : Cell, new()
		where THeaderCell : Cell, new()
	{
		THeaderCell headerCell = new THeaderCell();

		public new THeaderCell HeaderCell
		{
			get { return headerCell; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				base.HeaderCell = value;
			}
		}

		internal override Cell GetHeaderCellInternal() { return headerCell; }
		internal override void SetHeaderCellInternal(Cell cell) { headerCell = (THeaderCell)cell; }

		public override object Clone()
		{
			var clone = MemberwiseClone() as TableColumn<TDataCell, THeaderCell>;

			clone.headerCell = headerCell.Clone() as THeaderCell;

			return clone;
		}
	}

	[NativeClass("NSTableColumn", "AppKit")]
	public abstract class TableColumn<TCell> : IDisposable, ICloneable
		where TCell : Cell, new()
	{
		#region Cache
		
		private static readonly NativeObjectCache<TableColumn<TCell>> columnCache = new NativeObjectCache<TableColumn<TCell>>(c => c.NativePointer);

		internal static TableColumn<TCell> GetInstance(IntPtr nativePointer) { return columnCache.GetObject(nativePointer); }

		#endregion

		#region Method Selector Ids

		#warning Don't forget to remove "TableColumn<TCell>.Selectors." prefix once mcs is bugfixed !
		static class Selectors
		{
			static class initWithIdentifier { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("initWithIdentifier:"); }

			static class dataCell { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dataCell"); }
			static class setDataCell { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDataCell:"); }

			static class headerCell { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("headerCell"); }
			static class setHeaderCell { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setHeaderCell:"); }

			public static IntPtr InitWithIdentifier { get { return initWithIdentifier.SelectorHandle; } }

			public static IntPtr DataCell { get { return TableColumn<TCell>.Selectors.dataCell.SelectorHandle; } }
			public static IntPtr SetDataCell { get { return setDataCell.SelectorHandle; } }

			public static IntPtr HeaderCell { get { return headerCell.SelectorHandle; } }
			public static IntPtr SetHeaderCell { get { return setHeaderCell.SelectorHandle; } }
		}
		
		#endregion

		[SelectorStub("dataCellForRow:", Kind = StubKind.InstanceLazy)]
		private static IntPtr GetDataCell(IntPtr self, IntPtr sel, IntPtr row)
		{
			var column = GetInstance(self);
			var cell = column.GetDataCell(checked((int)row));
			return cell != null ? cell.NativePointer :  IntPtr.Zero;
		}

		private SafeNativeMethods.objc_super super;
		private bool disposed;
		private int width;
		private int minWidth;
		private int maxWidth;
		ColumnSizingOptions sizingOptions;
		private object owner;
		private TCell dataCell = new TCell();

		internal TableColumn() { }

		~TableColumn() { Dispose(false); }
		
		protected void Dispose(bool disposing)
		{
			if (super.Receiver != (IntPtr)0)
			{
				columnCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
			disposed = true;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		private void CreateNative()
		{
			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
			super.Receiver = SafeNativeMethods.objc_msgSend(ObjectiveC.AllocObject(nativeClass), Selectors.InitWithIdentifier, IntPtr.Zero);
			SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetDataCell, dataCell.NativePointer);
			SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.SetHeaderCell, GetHeaderCellInternal().NativePointer);
			columnCache.RegisterObject(this);
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal bool Created { get { return super.Receiver != (IntPtr)0; } }
		
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

		[SelectorImplementation("dataCellForRow:")]
		public virtual TCell GetDataCell(int row) { return dataCell; }

		public TCell DataCell
		{
			get { return dataCell; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				dataCell = value;
			}
		}

		public Cell HeaderCell
		{
			get { return GetHeaderCellInternal(); }
			set { SetHeaderCellInternal(value); }
		}

		internal abstract Cell GetHeaderCellInternal();
		internal abstract void SetHeaderCellInternal(Cell cell);

		public object Owner
		{
			get { return owner; }
			internal set
			{
				if (value != null && owner != null)
					throw new InvalidOperationException(Localization.GetExceptionText("ColumnOwned"));
				if (value == null && owner == null)
					throw new InvalidOperationException(Localization.GetExceptionText("InternalError"));
				owner = value;
			}
		}
		
		public void SizeToFit()
		{
		}

		public ColumnSizingOptions SizingOptions
		{
			get { return sizingOptions; }
			set { sizingOptions = value; }
		}
		
		public virtual object Clone()
		{
			var clone = MemberwiseClone() as TableColumn<TCell>;

			clone.dataCell = dataCell.Clone() as TCell;
			
			return clone;
		}
	}
}
