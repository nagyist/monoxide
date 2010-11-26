using System;
using System.Security;
using System.Runtime.InteropServices;

namespace System.MacOS
{
	partial class SafeNativeMethods
	{
		public enum OSType
		{
			gestaltSystemVersion = 0x73797376, /* sysv */
			gestaltSystemVersionMajor = 0x73797331, /* sys1 */
			gestaltSystemVersionMinor = 0x73797332, /* sys2 */
			gestaltSystemVersionBugFix = 0x73797333 /* sys3 */
		}

		[DllImport(CoreServices)]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode Gestalt(OSType selector, out int response);
	}
}
