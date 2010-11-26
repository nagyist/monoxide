using System;

namespace System.MacOS
{
	partial class ObjectiveC
	{
		/// <summary>Encapsulate a CLR string object in an Objective-C NSString object.</summary>
		/// <remarks>
		/// This will map to the Objective-C CLRString class.
		/// This class is static because System.String is the real wrapped class.
		/// </remarks>
		[NativeClass("NSString")]
		internal static class String
		{
			[SelectorStub("length")]
			private static IntPtr Length(IntPtr self, IntPtr sel)
			{
				string @string = GetManagedObject(self) as string;
	
				return (IntPtr)@string.Length;
			}
	
			[SelectorStub("characterAtIndex:")]
			private static char CharacterAtIndex(IntPtr self, IntPtr sel, IntPtr index)
			{
				string @string = GetManagedObject(self) as string;
	
				try { return @string[checked((int)index)]; }
				catch { /* TODO: Throw Objective-C exception… */ throw; }
			}
	
			[SelectorStub("getCharacters:Range:")]
			private unsafe static void GetCharacters(IntPtr self, IntPtr sel, char* buffer, SafeNativeMethods.NSRange range)
			{
				string @string = GetManagedObject(self) as string;
	
				// TODO: Check parameters
				fixed (char* stringPointer = @string)
					Memory.Copy(buffer, stringPointer + checked((int)range.location), checked((int)range.length));
			}
	
			public static IntPtr NativeAddRef(string @string)
			{
				IntPtr value = GetNativeObject(@string);
				return IntPtr.Zero;
			}
	
			public static void NativeRelease(string @string)
			{
			}
		}
	}
}
