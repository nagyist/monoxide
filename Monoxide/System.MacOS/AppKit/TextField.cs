using System;

namespace System.MacOS.AppKit
{
	public sealed class TextField : TextField<TextFieldCell> { }
	
	[NativeClass("NSTextField")]
	[DefaultCellType(typeof(TextFieldCell))]
	public class TextField<TCell> : Control<TCell>
		where TCell : TextFieldCell, new()
	{
		bool editable;
		bool selectable;
		string text;
		
		public TextField()
		{
			editable = true;
			selectable = true;
		}
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, CommonSelectors.SetEditable, editable);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, CommonSelectors.SetSelectable, selectable);
			SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetStringValue, text ?? string.Empty);
		}
		
		public string Text
		{
			get
			{
				if (Created)
					text = SafeNativeMethods.objc_msgSend_get_String(NativePointer, CommonSelectors.StringValue);
				
				return text;
			}
			set
			{
				if (value != text)
				{
					text = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_String(NativePointer, CommonSelectors.SetStringValue, text);
				}
			}
		}
		
		public bool Editable
		{
			get
			{
				if (Created)
					editable = SafeNativeMethods.objc_msgSend_get_Boolean(NativePointer, CommonSelectors.IsEditable);
				
				return editable;
			}
			set
			{
				if (value != editable)
				{
					editable = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, CommonSelectors.SetEditable, value);
				}
			}
		}
	}
}
