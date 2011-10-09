using System;
using System.Text;
using System.Runtime.InteropServices;

namespace System.MacOS.Foundation
{
	[NativeClass("NSData", "Foundation")]
	internal sealed class Data
	{
		#region Method Selector Ids
		
		private static class Selectors
		{
			static class dataWithBytesLength { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dataWithBytes:length:"); }
			static class dataWithBytesNoCopyLengthFreeWhenDone { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("dataWithBytesNoCopy:length:freeWhenDone:"); }

			public static IntPtr DataWithBytesLength { get { return dataWithBytesLength.SelectorHandle; } }
			public static IntPtr DataWithBytesNoCopyLengthFreeWhenDone { get { return dataWithBytesNoCopyLengthFreeWhenDone.SelectorHandle; } }
		}
		
		#endregion

		internal static readonly IntPtr Class = ObjectiveC.GetNativeClass(typeof(Data), false);

		public static unsafe IntPtr Create(byte* bytes, int length)
		{
			return SafeNativeMethods.objc_msgSend(Class, Selectors.DataWithBytesLength, (IntPtr)bytes, checked((IntPtr)length));
		}

		public static unsafe IntPtr Create(byte[] bytes)
		{
			fixed (byte* bytesPointer = bytes)
				return Create(bytesPointer, bytes.Length);
		}

		public static unsafe IntPtr Create(string text)
		{
			fixed (char* charsPointer = text)
				return Create((byte*)charsPointer, sizeof(char) * text.Length);
		}

		public static unsafe IntPtr Create(string text, Encoding encoding)
		{
			// Avoid unnecessary allocations by allocating the native buffer directly in managed code
			var byteCount = encoding.GetByteCount(text);
			var bytesPointer = (byte*)Marshal.AllocHGlobal(byteCount);

			fixed (char* charsPointer = text)
				encoding.GetBytes(charsPointer, text.Length, bytesPointer, byteCount);

			return SafeNativeMethods.objc_msgSend(Class, Selectors.DataWithBytesNoCopyLengthFreeWhenDone, (IntPtr)bytesPointer, checked((IntPtr)byteCount), true);
		}
	}
}
