using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSButtonCell", "AppKit")]
	public class ButtonCell : ActionCell
	{
		#region Method Selector Ids
		
		internal static class Selectors
		{
			static class bezelStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("bezelStyle"); }
			static class setBezelStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setBezelStyle:"); }
			static class buttonType { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("buttonType"); }
			static class setButtonType { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setButtonType:"); }
			static class state { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("state"); }
			static class setState { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setState:"); }
			
			public static IntPtr BezelStyle { get { return bezelStyle.SelectorHandle; } }
			public static IntPtr SetBezelStyle { get { return setBezelStyle.SelectorHandle; } }
			public static IntPtr ButtonType { get { return buttonType.SelectorHandle; } }
			public static IntPtr SetButtonType { get { return setButtonType.SelectorHandle; } }
			public static IntPtr State { get { return state.SelectorHandle; } }
			public static IntPtr SetState { get { return setState.SelectorHandle; } }
		}
		
		#endregion
		
		string title;
		ButtonType buttonType;
		BezelStyle bezelStyle;
		ButtonState buttonState;
		
		public ButtonCell()
		{
			title = "Button";
			buttonType = ButtonType.MomentaryLight;
			bezelStyle = BezelStyle.Rounded;
			buttonState = ButtonState.Unchecked;
		}
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetTitle, title);
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetBezelStyle, (IntPtr)bezelStyle);
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetButtonType, (IntPtr)buttonType);
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetState, (IntPtr)buttonState);
		}
		
		public string Title
		{
			get { return title; }
			set
			{
				title = value;
				
				if (Created && value != title)
					SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetTitle, title);
			}
		}
		
		public ButtonType ButtonType
		{
			get { return buttonType; }
			set
			{
				if (value != buttonType)
				{
					buttonType = value;
				
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetButtonType, (IntPtr)buttonType);
				}
			}
		}
		
		public BezelStyle BezelStyle
		{
			get { return bezelStyle; }
			set
			{
				if (value != bezelStyle)
				{
					bezelStyle = value;
				
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetBezelStyle, (IntPtr)bezelStyle);
				}
			}
		}
		
		public ButtonState State
		{
			get
			{
				if (Created)
					buttonState = (ButtonState)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.State);
				
				return buttonState;
			}
			set
			{
				if (value != buttonState)
				{
					buttonState = value;
				
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetState, (IntPtr)buttonState);
				}
			}
		}
	}
}
