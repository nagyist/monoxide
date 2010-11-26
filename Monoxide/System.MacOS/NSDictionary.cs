using System;

namespace System.MacOS
{
	internal static class NSDictionary
	{
		internal static void SetValue(IntPtr dictionary, string key, IntPtr value)
		{
			SafeNativeMethods.objc_msgSend_set_ObjectForKey(dictionary, ObjectiveC.Selectors.SetObjectForKey, value, key);
		}

		internal static void SetValue(IntPtr dictionary, string key, string value)
		{
			SafeNativeMethods.objc_msgSend_set_ObjectForKey(dictionary, ObjectiveC.Selectors.SetObjectForKey, value, key);
		}
	}
}

