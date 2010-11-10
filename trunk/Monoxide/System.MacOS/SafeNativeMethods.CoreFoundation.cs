using System;
using System.Security;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	partial class SafeNativeMethods
	{
		#region CFType

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern IntPtr CFRetain(IntPtr cf);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern void CFRelease(IntPtr cf);

		#endregion

		#region CFLocale

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern IntPtr CFLocaleCopyCurrent();
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation, EntryPoint = "CFLocaleGetValue")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler), MarshalCookie = "n")]
		public static extern string CFLocaleGetStringValue(IntPtr locale, IntPtr key);

		#endregion
	}
}

