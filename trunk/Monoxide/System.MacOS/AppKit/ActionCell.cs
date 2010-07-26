using System;
using System.Diagnostics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSActionCell", "AppKit")]
	public class ActionCell : Cell, ICommandItem
	{
		#region Event Handling & Dispatching
		
		[SelectorStubAttribute("clrCommand:")]
		private static void HandleAction(IntPtr self, IntPtr _cmd, IntPtr sender)
		{
			Debug.Assert(SafeNativeMethods.objc_msgSend(self, Selectors.ControlView) == sender);
			
			var cell = GetInstance(self) as ActionCell;
			
			if (cell == null) return;
			
			cell.HandleAction();
		}
		
		[SelectorStubAttribute("validateItem:")]
		private static bool ValidateItem(IntPtr self, IntPtr _cmd, IntPtr nativeControl)
		{
			Debug.Assert(SafeNativeMethods.objc_msgSend(self, Selectors.ControlView) == nativeControl);
			
			var cell = GetInstance(self) as ActionCell;
			
			if (cell == null) return false;
			
			return true;
		}
		
		#endregion
		
		public event EventHandler Action;
		
		public ActionCell()
		{
		}
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetTarget, NativePointer);
			SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetAction, CommonSelectors.ClrCommand);
		}
		
		public Command Command { get; set; }
		
		public CommandTarget CommandTarget { get; set; }
		
		private void HandleAction()
		{
			OnAction(EventArgs.Empty);
		}
		
		protected virtual void OnAction(EventArgs e)
		{
			if (Action != null)
				Action(this, e);
			
			if (Command != null)
			{
				var target = CommandTarget ?? Application.Current.GetTargetForCommand(Command);
				
				if (target != null)
					target.Execute(Command, this);
			}
		}
	}
}
