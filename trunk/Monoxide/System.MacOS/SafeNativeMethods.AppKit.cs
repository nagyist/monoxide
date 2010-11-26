using System;
using System.Security;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	partial class SafeNativeMethods
	{
		public enum BackingStoreType
		{
			BackingStoreRetained = 0,
			BackingStoreNonretained = 1,
			BackingStoreBuffered = 2
		}

		public enum ApplicationTerminateReply {
			TerminateCancel = 0,
			TerminateNow = 1,
			TerminateLater = 2
		}

		public struct NSRange
		{
			public IntPtr location;
			public IntPtr length;
		}

		public static readonly IntPtr NSNotFound = ObjectiveC.LP64 ? (IntPtr)Int64.MaxValue : (IntPtr)Int32.MaxValue;

		[DllImport(AppKit)]
		[SuppressUnmanagedCodeSecurity]
		public static extern void NSBeep();
	}
}

