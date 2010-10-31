using System;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	public sealed class Alert : IDisposable
	{
		#region Method Selector Ids

		#warning Don't forget to remove "Alert.Selectors." prefix once dmcs is bugfixed !
		static class Selectors
		{
			static class alertStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("alertStyle"); }
			static class setAlertStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAlertStyle:"); }
			static class showsHelp { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("showsHelp"); }
			static class setShowsHelp { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setShowsHelp:"); }
			static class showsSuppressionButton { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("showsSuppressionButton"); }
			static class setShowsSuppressionButton { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setShowsSuppressionButton:"); }
			static class informativeText { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("informativeText"); }
			static class setInformativeText { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setInformativeText:"); }
			static class messageText { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("messageText"); }
			static class setMessageText { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setMessageText:"); }
			static class icon { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("icon"); }
			static class setIcon { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setIcon:"); }
			static class accessoryView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("accessoryView"); }
			static class setAccessoryView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAccessoryView:"); }
			static class suppressionButton { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("suppressionButton"); }
			
			static class addButtonWithTitle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("addButtonWithTitle:"); }
			static class runModal { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("runModal"); }

			public static IntPtr AlertStyle { get { return Alert.Selectors.alertStyle.SelectorHandle; } }
			public static IntPtr SetAlertStyle { get { return setAlertStyle.SelectorHandle; } }
			public static IntPtr ShowsHelp { get { return showsHelp.SelectorHandle; } }
			public static IntPtr SetShowsHelp { get { return setShowsHelp.SelectorHandle; } }
			public static IntPtr ShowsSuppressionButton { get { return showsSuppressionButton.SelectorHandle; } }
			public static IntPtr SetShowsSuppressionButton { get { return setShowsSuppressionButton.SelectorHandle; } }
			public static IntPtr InformativeText { get { return Alert.Selectors.informativeText.SelectorHandle; } }
			public static IntPtr SetInformativeText { get { return setInformativeText.SelectorHandle; } }
			public static IntPtr MessageText { get { return Alert.Selectors.messageText.SelectorHandle; } }
			public static IntPtr SetMessageText { get { return setMessageText.SelectorHandle; } }
			public static IntPtr Icon { get { return Alert.Selectors.icon.SelectorHandle; } }
			public static IntPtr SetIcon { get { return setIcon.SelectorHandle; } }
			public static IntPtr AccessoryView { get { return Alert.Selectors.accessoryView.SelectorHandle; } }
			public static IntPtr SetAccessoryView { get { return setAccessoryView.SelectorHandle; } }
			public static IntPtr SuppressionButton { get { return suppressionButton.SelectorHandle; } }
			
			public static IntPtr RunModal { get { return runModal.SelectorHandle; } }
			public static IntPtr AddButtonWithTitle { get { return addButtonWithTitle.SelectorHandle; } }
		}
		
		#endregion
		
		IntPtr nativePointer;
		bool disposed;
		AlertStyle alertStyle;
		string messageText;
		string informativeText;
		bool showSuppressionButton;
		bool suppress;
		Image icon;
		View accessoryView;
		List<string> buttons = new List<string>();
		
		public Alert()
		{
		}
		
		private Alert(IntPtr nativePointer)
		{
			this.nativePointer = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~Alert()
		{
			Dispose(false);
		}
		
		private void Dispose(bool disposing)
		{
			if (nativePointer != IntPtr.Zero)
			{
				ObjectiveC.ReleaseObject(nativePointer);
				nativePointer = IntPtr.Zero;
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
			using (var pool = LocalAutoReleasePool.Create())
			{
				nativePointer = ObjectiveC.AllocObject(CommonClasses.NSAlert);
				nativePointer = SafeNativeMethods.objc_msgSend(nativePointer, ObjectiveC.Selectors.Init);
				SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetAlertStyle, (IntPtr)alertStyle);
				SafeNativeMethods.objc_msgSend_set_String(nativePointer, Selectors.SetMessageText, messageText);
				SafeNativeMethods.objc_msgSend_set_String(nativePointer, Selectors.SetInformativeText, informativeText);
				SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetShowsSuppressionButton, showSuppressionButton);
				SafeNativeMethods.objc_msgSend(
							SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SuppressionButton),
							ButtonCell.Selectors.SetState,
							(IntPtr)(suppress ? ButtonState.Checked : ButtonState.Unchecked));
				if (accessoryView != null)
					SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetAccessoryView, accessoryView.NativePointer);
				foreach (var buttonTitle in buttons)
					SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.AddButtonWithTitle, buttonTitle);
			}
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal bool Created { get { return nativePointer != IntPtr.Zero; } }
		
		internal IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);
				
				if (!Created)
					CreateNative();
				
				return nativePointer;
			}
		}
		
		public AlertStyle Style
		{
			get
			{
				if (Created)
					alertStyle = (AlertStyle)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.AlertStyle);
				
				return alertStyle;
			}
			set
			{
				if (value != alertStyle)
				{
					alertStyle = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetAlertStyle, (IntPtr)alertStyle);
				}
			}
		}
		
		public string MessageText
		{
			get
			{
				if (Created)
					messageText = SafeNativeMethods.objc_msgSend_get_String(NativePointer, Selectors.MessageText);
				
				return messageText;
			}
			set
			{
				if (value != messageText)
				{
					messageText = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetMessageText, messageText);
				}
			}
		}
		
		public string InformativeText
		{
			get
			{
				if (Created)
					informativeText = SafeNativeMethods.objc_msgSend_get_String(NativePointer, Selectors.InformativeText);
				
				return informativeText;
			}
			set
			{
				if (value != informativeText)
				{
					informativeText = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.SetInformativeText, informativeText);
				}
			}
		}
		
		public Image Icon
		{
			get
			{
				return icon;
			}
			set
			{
				icon = value;
			}
		}
		
		public bool ShowSuppressionButton // Note I didn't put the 's' after "Show", this is on purpose.
		{
			get
			{
				if (Created)
					showSuppressionButton = SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, Selectors.ShowsSuppressionButton);
				
				return showSuppressionButton;
			}
			set
			{
				if (value != showSuppressionButton)
				{
					showSuppressionButton = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, Selectors.SetShowsSuppressionButton, showSuppressionButton);
				}
			}
		}
		
		public bool Suppress
		{
			get
			{
				if (Created)
					suppress = (ButtonState)SafeNativeMethods.objc_msgSend(
							SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SuppressionButton),
							ButtonCell.Selectors.State) == ButtonState.Checked;
				
				return suppress;
			}
			set
			{
				if (value != suppress)
				{
					suppress = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(
							SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SuppressionButton),
							ButtonCell.Selectors.SetState,
							(IntPtr)(suppress ? ButtonState.Checked : ButtonState.Unchecked));
				}
			}
		}
		
		public View AccessoryView
		{
			get
			{
				return accessoryView;
			}
			set
			{
				if (value != accessoryView)
				{
					accessoryView = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetAccessoryView, accessoryView != null ? accessoryView.NativePointer : IntPtr.Zero);
				}
			}
		}
		
		public void AddButton(string title)
		{
			buttons.Add(title);
			if (Created)
				SafeNativeMethods.objc_msgSend_set_String(NativePointer, Selectors.AddButtonWithTitle, title);
		}
		
		public int ShowDialog()
		{
			return checked((int)SafeNativeMethods.objc_msgSend(NativePointer, Selectors.RunModal) - 1000);
		}
	}
}
