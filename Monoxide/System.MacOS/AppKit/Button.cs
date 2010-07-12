using System;

namespace System.MacOS.AppKit
{
	public sealed class Button : Button<ButtonCell> { }
	
	// TODO: Rethink the Button inheritance scheme. Getting rid of the ButtonBase class for now…
	[NativeClass("NSButton", "AppKit")]
	[DefaultCellType(typeof(ButtonCell))]
	public class Button<TCell> : Control<TCell>
		where TCell : ButtonCell, new()
	{
		public Button()
		{
			Bounds = new Rectangle(0, 0, 100, 32);
		}
		
		public string Title
		{
			get { return Cell.Title; }
			set { Cell.Title = value; }
		}
		
		public ButtonType ButtonType
		{
			get { return Cell.ButtonType; }
			set { Cell.ButtonType = value; }
		}
		
		public BezelStyle BezelStyle
		{
			get { return Cell.BezelStyle; }
			set { Cell.BezelStyle = value; }
		}
		
		public ButtonState State
		{
			get { return Cell.State; }
			set { Cell.State = value; }
		}
		
		public bool Checked
		{
			get { return State == ButtonState.Checked; }
			set { State = value ? ButtonState.Checked : ButtonState.Unchecked; }
		}
	}
}
