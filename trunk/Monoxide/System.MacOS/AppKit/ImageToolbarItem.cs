using System;

namespace System.MacOS.AppKit
{
	public class ImageToolbarItem : ToolbarItem
	{
		private Image image;
		
		public ImageToolbarItem() { }
		
		public ImageToolbarItem(string name)
			: base(name) { }
		
		public ImageToolbarItem(string name, IntPtr nativeName)
			: base(name, nativeName) { }
		
		internal override void OnCreated()
		{
			base.OnCreated();
			
			if (image != null)
				SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetImage, image.NativePointer);
		}
		
		public Image Image
		{
			get { return image; }
			set
			{
				if (value != image)
				{
					image = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, CommonSelectors.SetImage, image != null ? image.NativePointer : IntPtr.Zero);
				}
			}
		}
	}
}
