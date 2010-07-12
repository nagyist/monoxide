using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSTextView", "AppKit")]
	public class TextView : View
	{
		bool editable;
		bool selectable;
		
		public TextView()
		{
			editable = true;
			selectable = true;
		}
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, CommonSelectors.SetEditable, editable);
			SafeNativeMethods.objc_msgSend_set_Boolean(NativePointer, CommonSelectors.SetSelectable, selectable);
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
		
		public void InsertText(string text)
		{
			SafeNativeMethods.objc_msgSend_set_String(NativePointer, ObjectiveC.GetSelector("insertText:"), text);
		}
	}
}
