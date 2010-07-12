using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	[DefaultCellType(typeof(Cell))]
	public class Control<TCell> : Control
		where TCell : Cell, new()
	{
		#region NSControl Interop
		
		[SelectorStub("cellClass", Kind = StubKind.ClassMandatory)]
		private static IntPtr GetCellType(IntPtr self, IntPtr _cmd)
		{
			return ObjectiveC.GetNativeClass(typeof(TCell), true);
		}
		
		#endregion
		
		private TCell cell = new TCell();
		
		internal override void OnCreated()
		{
			base.OnCreated();
			cell.NativeCreated(SafeNativeMethods.objc_msgSend(NativePointer, ObjectiveC.GetSelector("cell")), true);
		}
		
		public new TCell Cell { get { return cell; } }
		
		internal sealed override Cell GetCellInternal() { return cell; }
		
		internal sealed override Type GetCellTypeInternal() { return typeof(TCell); }
		
		protected sealed override void OnAction(EventArgs e) { base.OnAction(e); }
		
		public override object Clone()
		{
			var clone = base.Clone() as Control<TCell>;
			
			clone.cell = cell.Clone() as TCell;
			
			return clone;
		}
	}
	
	// TODO: Handle correct item validation, compatible with CommandTarget validation
	[NativeClass("NSControl", "AppKit")]
	public class Control : View, ICommandItem
	{
		#region NSControl Interop
		
		[SelectorStub("setCellClass:", Kind = StubKind.ClassMandatory)]
		private static void SetCellType(IntPtr self, IntPtr _cmd, IntPtr cell) { }
		
		// TODO: Fully implement the Cell protocol on the C# side, and uncomment this
		//[SelectorStub("setCell:")]
		private static void SetCell(IntPtr self, IntPtr _cmd, IntPtr cell) { }
		
		#endregion
		
		#region Event Handling & Dispatching
		
		[SelectorStubAttribute("clrCommand:")]
		private static void HandleAction(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			Debug.Assert(self == sender);
			
			var control = GetInstance(sender) as Control;
			
			if (control == null) return;
			
			control.HandleAction();
		}
		
		[SelectorStubAttribute("validateItem:")]
		private static bool ValidateItem(IntPtr self, IntPtr _cmd, IntPtr nativeControl)
		{
			Debug.Assert(self == nativeControl);
			
			var control = GetInstance(nativeControl) as Control;
			
			if (control == null) return false;
			
			return true;
		}
		
		#endregion
		
		private Command command;
		private CommandTarget commandTarget;
		private EventHandler action;
		
		public Control() { }
		
		internal override void OnCreated()
		{
			if (Cell == null)
			{
				SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetTarget, NativePointer);
				SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetAction, CommonSelectors.ClrCommand);
			}
		}
		
		public Cell Cell { get { return GetCellInternal(); } }
		
		public Type CellType { get { return GetCellTypeInternal(); } }
		
		public event EventHandler Action
		{
			add
			{
				var cell = Cell as ActionCell;
				
				if (cell != null) cell.Action += value;
				else action += value;
			}
			remove
			{
				var cell = Cell as ActionCell;
				
				if (cell != null) cell.Action -= value;
				else action -= value;
			}
		}
		
		public Command Command
		{
			get
			{
				var cell = Cell as ActionCell;
				
				return cell != null ? cell.Command : command;
			}
			set
			{
				var cell = Cell as ActionCell;
				
				if (cell != null) cell.Command = value;
				else command = value;
			}
		}
		
		public CommandTarget CommandTarget
		{
			get
			{
				var cell = Cell as ActionCell;
				
				return cell != null ? cell.CommandTarget : commandTarget;
			}
			set
			{
				var cell = Cell as ActionCell;
				
				if (cell != null) cell.CommandTarget = value;
				else commandTarget = value;
			}
		}
		
		public object Tag { get; set; }
		
		internal virtual Cell GetCellInternal() { return null; }
		
		internal virtual Type GetCellTypeInternal() { return null; }
		
		private void HandleAction()
		{
			OnAction(EventArgs.Empty);
		}
		
		protected virtual void OnAction(EventArgs e)
		{
			if (action != null)
				action(this, e);
			
			if (Command != null)
			{
				var target = CommandTarget ?? Application.Current.GetTargetForCommand(Command);
				
				if (target != null)
					target.Execute(Command, this);
			}
		}
	}
}
