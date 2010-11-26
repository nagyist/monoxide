using System;

namespace System.MacOS
{
	internal static class OSVersion
	{
		public static readonly OperatingSystem Value = BuildOSVersion();

		private static OperatingSystem BuildOSVersion()
		{
			int major;
			int minor;
			int bugFix;

			SafeNativeMethods.Gestalt(SafeNativeMethods.OSType.gestaltSystemVersionMajor, out major);
			SafeNativeMethods.Gestalt(SafeNativeMethods.OSType.gestaltSystemVersionMinor, out minor);
			SafeNativeMethods.Gestalt(SafeNativeMethods.OSType.gestaltSystemVersionBugFix, out bugFix);

			return new OperatingSystem(PlatformID.MacOSX, new Version(major, minor, bugFix));
		}
	}
}

