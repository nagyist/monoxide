using System;

namespace System.MacOS
{
	internal static class CommonSelectors
	{		
		static class firstResponder { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("firstResponder"); }
		static class nextResponder { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("nextResponder"); }
		
		static class title { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("title"); }
		static class setTitle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setTitle:"); }
		
		static class toolTip { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("toolTip"); }
		static class setToolTip { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setToolTip:"); }
		
		static class font { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("font"); }
		static class setFont { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setFont:"); }

		static class image { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("image"); }
		static class setImage { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setImage:"); }
			
		static class isEnabled { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isEnabled"); }
		static class setEnabled { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setEnabled:"); }
			
		static class isVisible { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isVisible"); }
		static class setVisible { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setVisible:"); }
		
		static class isEditable { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isEditable"); }
		static class setEditable { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setEditable:"); }
		
		static class isSelectable { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("isSelectable"); }
		static class setSelectable { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setSelectable:"); }
		
		static class dataSource { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dataSource"); }
		static class setDataSource { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDataSource:"); }
		
		static class @delegate { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("delegate"); }
		static class setDelegate { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setDelegate:"); }
		
		static class target { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("target"); }
		static class setTarget { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setTarget:"); }

		static class action { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("action"); }
		static class setAction { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAction:"); }
		
		static class stringValue { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("stringValue"); }
		static class setStringValue { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setStringValue:"); }
		
		static class objectValue { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("objectValue"); }
		static class setObjectValue { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setObjectValue:"); }

		static class attributedStringValue { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("attributedStringValue"); }
		static class setAttributedStringValue { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setAttributedStringValue:"); }

		static class controlView { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("controlView"); }
		
		static class clrCommand { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("clrCommand:"); }
		
		static class validateUserInterfaceItem { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("validateUserInterfaceItem:"); }
		
		public static IntPtr FirstResponder { get { return firstResponder.SelectorHandle; } }
		public static IntPtr NextResponder { get { return nextResponder.SelectorHandle; } }
		
		public static IntPtr Title { get { return title.SelectorHandle; } }
		public static IntPtr SetTitle { get { return setTitle.SelectorHandle; } }
		
		public static IntPtr ToolTip { get { return toolTip.SelectorHandle; } }
		public static IntPtr SetToolTip { get { return setToolTip.SelectorHandle; } }
			
		public static IntPtr Font { get { return font.SelectorHandle; } }
		public static IntPtr SetFont { get { return setFont.SelectorHandle; } }
		
		public static IntPtr Image { get { return image.SelectorHandle; } }
		public static IntPtr SetImage { get { return setImage.SelectorHandle; } }
			
		public static IntPtr IsEnabled { get { return isEnabled.SelectorHandle; } }
		public static IntPtr SetEnabled { get { return setEnabled.SelectorHandle; } }
			
		public static IntPtr IsVisible { get { return isVisible.SelectorHandle; } }
		public static IntPtr SetVisible { get { return setVisible.SelectorHandle; } }
		
		public static IntPtr IsEditable { get { return isEditable.SelectorHandle; } }
		public static IntPtr SetEditable { get { return setEditable.SelectorHandle; } }
			
		public static IntPtr IsSelectable { get { return isSelectable.SelectorHandle; } }
		public static IntPtr SetSelectable { get { return setSelectable.SelectorHandle; } }
		
		public static IntPtr DataSource { get { return dataSource.SelectorHandle; } }
		public static IntPtr SetDataSource { get { return setDataSource.SelectorHandle; } }

		public static IntPtr Delegate { get { return @delegate.SelectorHandle; } }
		public static IntPtr SetDelegate { get { return setDelegate.SelectorHandle; } }
		
		public static IntPtr Target { get { return target.SelectorHandle; } }
		public static IntPtr SetTarget { get { return setTarget.SelectorHandle; } }
		
		public static IntPtr Action { get { return action.SelectorHandle; } }
		public static IntPtr SetAction { get { return setAction.SelectorHandle; } }
		
		public static IntPtr StringValue { get { return stringValue.SelectorHandle; } }
		public static IntPtr SetStringValue { get { return setStringValue.SelectorHandle; } }
		public static IntPtr AttributedStringValue { get { return attributedStringValue.SelectorHandle; } }
		public static IntPtr SetAttributedStringValue { get { return setAttributedStringValue.SelectorHandle; } }
		public static IntPtr ObjectValue { get { return objectValue.SelectorHandle; } }
		public static IntPtr SetObjectValue { get { return setObjectValue.SelectorHandle; } }

		public static IntPtr ControlView { get { return controlView.SelectorHandle; } }
		
		public static IntPtr ClrCommand { get { return clrCommand.SelectorHandle; } }
		
		public static IntPtr ValidateUserInterfaceItem { get { return validateUserInterfaceItem.SelectorHandle; } }
	}
}
