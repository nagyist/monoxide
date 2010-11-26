using System;
using System.Security;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	partial class SafeNativeMethods
	{
		#region Private Methods

		// These methods are private CF methods.
		// But they are somehow documented, as the code for CoreFoundation is open source…
		// Also, WebKit nightly make use of them for the same purposes (cheat the process path)
		// We wouldn't need these if mono handled assemblies differently…

		#if ENABLE_PROCESS_TWEAKS
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern IntPtr _CFGetProcessPath(); // This returns a pointer to the internal process path string
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringResultMarshaler))] // This returns the internal process path string
		public static extern string _CFProcessPath();
		#endif

		#endregion

		#region CFType

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern IntPtr CFRetain(IntPtr cf);
		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern void CFRelease(IntPtr cf);

		#endregion

		#region CFBundle

		[SuppressUnmanagedCodeSecurity]
		[DllImport(CoreFoundation)]
		public static extern IntPtr CFBundleGetMainBundle();

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

