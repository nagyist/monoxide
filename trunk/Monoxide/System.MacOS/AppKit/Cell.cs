using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSCell", "AppKit")]
	public class Cell : ICloneable
	{
		#region Method Selector Ids
		
		internal static class Selectors
		{
			static class controlView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("controlView"); }
			static class setControlView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setControlView:"); }
			
			public static IntPtr ControlView { get { return controlView.SelectorHandle; } }
			public static IntPtr SetControlView { get { return setControlView.SelectorHandle; } }
		}
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<Cell> cellList = new NativeObjectCache<Cell>(c => c.NativePointer);
		
		internal static Cell GetInstance(IntPtr nativePointer) { return cellList.GetObject(nativePointer); }
		
		#endregion
		
		internal SafeNativeMethods.objc_super super;
		private bool disposed;
		private Menu menu;
		private LayoutDirection layoutDirection;
		private object value;
		
		public Cell()
		{
		}
		
		~Cell()
		{
			Dispose(false);
			disposed = true;
		}
		
		protected void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				cellList.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			disposed = true;
			GC.SuppressFinalize(this);
		}
		
		private void CreateNative()
		{
			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
			super.Receiver = ObjectiveC.AllocAndInitObject(nativeClass);
			cellList.RegisterObject(this);
			OnCreated();
		}
		
		internal void NativeCreated(IntPtr nativePointer, bool retain)
		{
			super.Class = ObjectiveC.GetNativeBaseClass(ObjectiveC.GetNativeClass(this.GetType(), true));
			super.Receiver = retain ? ObjectiveC.RetainObject(nativePointer) : nativePointer;
			cellList.RegisterObject(this);
			OnCreated();
		}
		
		internal virtual void OnCreated() { }
		
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
		
		public Control Control
		{
			// Here we use super.Receiver directly, as the value has to be directly fetched from the native NSCell anyway
			get { return View.GetInstance(SafeNativeMethods.objc_msgSend(super.Receiver, Selectors.ControlView)) as Control; }
		}
		
		public Menu Menu
		{
			get { return Menu; }
			set
			{
				if (value != menu)
				{
					if (value != null) value.Owner = this;
					if (menu != null) menu.Owner = null;
					
					menu = value;
				}
			}
		}
		
		#if MACOSX_10_6
		public LayoutDirection LayoutDirection
		{
			get { return layoutDirection; }
			set
			{
				if (value != layoutDirection)
				{
					layoutDirection = value;
				}
			}
		}
		#endif
		
		public object Value
		{
			get { return value; }
			set
			{
				if (value != this.value)
				{
					this.value = value;
					
					if (Created)
					{
						switch (this.value != null ? Type.GetTypeCode(this.value.GetType()) : TypeCode.DBNull)
						{
							case TypeCode.String:
								SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetStringValue, this.value as string);
								break;
							case TypeCode.DBNull:
								SafeNativeMethods.objc_msgSend(NativePointer, IntPtr.Zero);
								break;
							default:
								break;
						}
					}
				}
			}
		}
		
		public object Tag { get; set; }
		
		public virtual object Clone()
		{
			var clone = MemberwiseClone() as Cell;
			
			clone.super.Receiver = IntPtr.Zero;
			if (menu != null) clone.menu = menu.Clone() as Menu;
			
			var cloneableValue = value as ICloneable;
			
			if (cloneableValue != null) clone.value = cloneableValue.Clone();
			
			return clone;
		}
	}
}
