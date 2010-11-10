using System;
using System.Security;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	partial class SafeNativeMethods
	{
		public enum ProcessApplicationTransformState
		{
			ProcessTransformToForegroundApplication = 1
		}
		
		public enum OSResultCode
		{
			ProcessNotFound = -600,
			MemoryFragmentationError = -601,
			ApplicationModeError = -602,
			ProtocolError = -603,
			HardwareConfigurationError = -604,
			ApplicationMemoryFullErrError = -605,
			ApplicationIsDaemon = -606,
			WrongApplicationPlatform = -875,
			ApplicationVersionTooOld = -876,
			NotAppropriateForClassic = -877
		}
				
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

		[DllImport(AppKit)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode GetCurrentProcess (out long psn);
		[DllImport(AppKit)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode TransformProcessType ([In] ref long psn, ProcessApplicationTransformState type);
		[DllImport(AppKit)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode SetFrontProcess ([In] ref long psn);

		[DllImport(AppKit)]
		[SuppressUnmanagedCodeSecurity]
		public static extern void NSBeep();
	}
}

