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
			NotAppropriateForClassic = -877,
			
			gestaltUnknownErr = -5550,
			gestaltUndefSelectorErr = -5551,
			gestaltDupSelectorErr = -5552,
			gestaltLocationErr = -5553
		}

		#if ENABLE_PROCESS_TWEAKS
		[DllImport(ApplicationServices)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode _RegisterApplication(IntPtr additionalAppInfoRef, [In] ref long psn);
		#endif
		[DllImport(ApplicationServices)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode GetCurrentProcess(out long psn);
		[DllImport(ApplicationServices)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode TransformProcessType([In] ref long psn, ProcessApplicationTransformState type);
		[DllImport(ApplicationServices)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode SetFrontProcess([In] ref long psn);
	}
}
