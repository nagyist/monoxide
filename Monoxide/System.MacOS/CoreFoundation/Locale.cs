using System;
using System.Runtime.InteropServices;

namespace System.MacOS.CoreFoundation
{
	internal class Locale
	{
		private static readonly IntPtr kCFLocaleLanguageCode = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("CoreFoundation", "kCFLocaleLanguageCode"));
		private static readonly IntPtr kCFLocaleCountryCode = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("CoreFoundation", "kCFLocaleCountryCode"));
		
		private static class CurrentLocale { public static readonly Locale Instance = new Locale(SafeNativeMethods.CFLocaleCopyCurrent()); }
		
		public static Locale Current { get { return CurrentLocale.Instance; } }
		
		IntPtr nativePointer;
		
		private Locale(IntPtr nativePointer) { this.nativePointer = nativePointer; }
		
		public string LanguageCode { get { return GetStringValue(kCFLocaleLanguageCode); } }
		public string CountryCode { get { return GetStringValue(kCFLocaleCountryCode); } }
		
		public string ClrCultureName { get { return LanguageCode + "-" + CountryCode; } }
		
		private string GetStringValue(IntPtr key) { return SafeNativeMethods.CFLocaleGetStringValue(nativePointer, key); }
	}
}
