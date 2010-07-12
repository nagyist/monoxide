using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

namespace System.MacOS
{
	internal static partial class ObjectiveC
	{
		#region Resource Dictionaries
		
		private static readonly Dictionary<string, IntPtr> classDictionary = new Dictionary<string, IntPtr>();
		private static readonly Dictionary<string, IntPtr> selectorDictionary = new Dictionary<string, IntPtr>();
		private static readonly Dictionary<string, IntPtr> frameworkDictionary = new Dictionary<string, IntPtr>();
		
		private static readonly Dictionary<object, IntPtr> managedToNativeDictionary = new Dictionary<object, IntPtr>();
		private static readonly Dictionary<IntPtr, object> nativeToManagedDictionary = new Dictionary<IntPtr, object>();
		
		#endregion
		
		#region Framework Location Formats
		
		private static readonly string[] frameworkLocationFormats = {
			"/System/Library/Frameworks/{0}.framework/{0}",
			"/Library/Frameworks/{0}.framework/{0}",
			"~/Library/Frameworks/{0}.framework/{0}" };
		
		#endregion
		
		#region Objective-C Class Handles
		
		internal static class Classes
		{
			static class _NSObject { public static readonly IntPtr ClassHandle = GetClass("NSObject"); }
			static class _ClrObject { public static readonly IntPtr ClassHandle = GetNativeClass(typeof(object), true); }
			static class _NSString { public static readonly IntPtr ClassHandle = GetClass("NSString"); }
			static class _NSDate { public static readonly IntPtr ClassHandle = GetClass("NSDate"); }
			static class _NSArray { public static readonly IntPtr ClassHandle = GetClass("NSArray"); }
			static class _NSUrl { public static readonly IntPtr ClassHandle = GetClass("NSURL"); }
			static class _NSAutoreleasePool { public static readonly IntPtr ClassHandle = GetClass("NSAutoreleasePool"); }
			static class _NSNotification { public static readonly IntPtr ClassHandle = GetClass("NSNotification"); }
			
			public static IntPtr NSObject { get { return _NSObject.ClassHandle; } }
			public static IntPtr ClrObject { get { return _ClrObject.ClassHandle; } }
			public static IntPtr NSString { get { return _NSString.ClassHandle; } }
			public static IntPtr NSDate { get { return _NSDate.ClassHandle; } }
			public static IntPtr NSArray { get { return _NSArray.ClassHandle; } }
			public static IntPtr NSUrl { get { return _NSUrl.ClassHandle; } }
			public static IntPtr NSAutoreleasePool { get { return _NSAutoreleasePool.ClassHandle; } }
			public static IntPtr NSNotification { get { return _NSNotification.ClassHandle; } }
		}
		
		#endregion
		
		#region Objective-C Common Selector Ids
		
		internal static class Selectors
		{
			static class alloc { public static readonly IntPtr SelectorHandle = GetSelector("alloc"); }
			static class init { public static readonly IntPtr SelectorHandle = GetSelector("init"); }
			static class retain { public static readonly IntPtr SelectorHandle = GetSelector("retain"); }
			static class release { public static readonly IntPtr SelectorHandle = GetSelector("release"); }
			static class autorelease { public static readonly IntPtr SelectorHandle = GetSelector("autorelease"); }
			static class dealloc { public static readonly IntPtr SelectorHandle = GetSelector("dealloc"); }
			static class copy { public static readonly IntPtr SelectorHandle = GetSelector("copy"); }
			static class copyWithZone { public static readonly IntPtr SelectorHandle = GetSelector("copyWithZone:"); }
			static class description { public static readonly IntPtr SelectorHandle = GetSelector("description"); }
			static class hash { public static readonly IntPtr SelectorHandle = GetSelector("hash"); }
			static class isEqual { public static readonly IntPtr SelectorHandle = GetSelector("isEqual:"); }
			// NSAutoReleasePool
			static class drain { public static readonly IntPtr SelectorHandle = GetSelector("drain"); }
			
			static class performSelectorOnMainThreadWithObjectWaitUntilDone { public static readonly IntPtr SelectorHandle = GetSelector("performSelectorOnMainThread:withObject:waitUntilDone:"); }
			
			static class instancesRespondToSelector { public static readonly IntPtr SelectorHandle = GetSelector("instancesRespondToSelector:"); }
			static class respondsToSelector { public static readonly IntPtr SelectorHandle = GetSelector("respondsToSelector:"); }
			static class isKindOfClass { public static readonly IntPtr SelectorHandle = GetSelector("isKindOfClass:"); }
			
			static class addObserverForKeyPathOptionsContext { public static readonly IntPtr SelectorHandle = GetSelector("addObserver:forKeyPath:options:context:"); }
			static class observeValueForKeyPathOfObjectChangeContext { public static readonly IntPtr SelectorHandle = GetSelector("observeValueForKeyPath:ofObject:change:context:"); }
						
			static class name { public static readonly IntPtr SelectorHandle = GetSelector("name"); }
			static class @object { public static readonly IntPtr SelectorHandle = GetSelector("object"); }
			static class userInfo { public static readonly IntPtr SelectorHandle = GetSelector("userInfo"); }
			
			// NSString,
			static class length { public static readonly IntPtr SelectorHandle = GetSelector("length"); }
			// NSURL
			static class urlWithString { public static readonly IntPtr SelectorHandle = GetSelector("URLWithString:"); }
			// NSDate
			static class distantFuture { public static readonly IntPtr SelectorHandle = GetSelector("distantFuture"); }
			// NSArray
			static class arrayWithObjectsCount { public static readonly IntPtr SelectorHandle = GetSelector("arrayWithObjects:count:"); }
			// NSDictionary
			static class objectForKey { public static readonly IntPtr SelectorHandle = GetSelector("objectForKey:"); }
			
			public static IntPtr Alloc { get { return alloc.SelectorHandle; } }
			public static IntPtr Init { get { return init.SelectorHandle; } }
			public static IntPtr Retain { get { return retain.SelectorHandle; } }
			public static IntPtr Release { get { return release.SelectorHandle; } }
			public static IntPtr AutoRelease { get { return autorelease.SelectorHandle; } }
			public static IntPtr Dealloc { get { return dealloc.SelectorHandle; } }
			public static IntPtr Copy { get { return copy.SelectorHandle; } }
			public static IntPtr CopyWithZone { get { return copyWithZone.SelectorHandle; } }
			public static IntPtr Description { get { return description.SelectorHandle; } }
			public static IntPtr Hash { get { return hash.SelectorHandle; } }
			public static IntPtr IsEqual { get { return isEqual.SelectorHandle; } }
			public static IntPtr Drain { get { return drain.SelectorHandle; } }
			
			public static IntPtr PerformSelectorOnMainThreadWithObjectWaitUntilDone { get { return performSelectorOnMainThreadWithObjectWaitUntilDone.SelectorHandle; } }
			
			public static IntPtr InstancesRespondToSelector { get { return instancesRespondToSelector.SelectorHandle; } }
			public static IntPtr RespondsToSelector { get { return respondsToSelector.SelectorHandle; } }
			public static IntPtr IsKindOfClass { get { return isKindOfClass.SelectorHandle; } }
			
			public static IntPtr AddObserverForKeyPathOptionsContext { get { return addObserverForKeyPathOptionsContext.SelectorHandle; } }
			public static IntPtr ObserveValueForKeyPathOfObjectChangeContext { get { return observeValueForKeyPathOfObjectChangeContext.SelectorHandle; } }
			
			public static IntPtr Name { get { return name.SelectorHandle; } }
			public static IntPtr Object { get { return @object.SelectorHandle; } }
			public static IntPtr UserInfo { get { return userInfo.SelectorHandle; } }
			
			public static IntPtr Length { get { return length.SelectorHandle; } }
			
			public static IntPtr UrlWithString { get { return urlWithString.SelectorHandle; } }
			
			public static IntPtr DistantFuture { get { return distantFuture.SelectorHandle; } }
			
			public static IntPtr ArrayWithObjectsCount { get { return arrayWithObjectsCount.SelectorHandle; } }
			
			public static IntPtr ObjectForKey { get { return objectForKey.SelectorHandle; } }
		}
		
		#endregion
		
		#region String Interop
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		private static unsafe extern IntPtr objc_msgSend_initWithCharacters_length(IntPtr self, IntPtr sel, char* characters, int length);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		private static unsafe extern IntPtr objc_msgSend_initWithCharactersNoCopy_length_freeWhenDone(IntPtr self, IntPtr sel, char* characters, int length, [MarshalAs(UnmanagedType.U1)] bool flag);
		
		private static readonly IntPtr NSString_initWithCharacters_length = GetSelector("initWithCharacters:length:");
		private static readonly IntPtr NSString_initWithCharactersNoCopy_length_freeWhenDone = GetSelector("initWithCharactersNoCopy:length:freeWhenDone:");
		private static readonly IntPtr NSString_getCharacters_range = GetSelector("getCharacters:range:");
		
		#endregion
		
		public static bool LP64 { get { return sizeof(int) < IntPtr.Size; } }
		
		private static IntPtr appKitFrameworkHandle;
		private static IntPtr webKitFrameworkHandle;
				
		internal static void EnsureAppKitFrameworkIsLoaded()
		{
			// Despite appearances, this is thread-safe
			if (appKitFrameworkHandle == IntPtr.Zero)
				appKitFrameworkHandle = LoadFramework("AppKit");
		}
		
		internal static void EnsureWebKitFrameworkIsLoaded()
		{
			// Despite appearances, this is thread-safe
			if (webKitFrameworkHandle == IntPtr.Zero)
				webKitFrameworkHandle = LoadFramework("WebKit");
		}
		
		/// <summary>Loads the specified framework at runtime.</summary>
		/// <remarks>
		/// This will typically be used for cases like loading Cocoa at application startup.
		/// In the case of AppKit, the explicit imports in SafeNativeMethods will force the loading anyway.
		/// </remarks>
		/// <param name="name">A <see cref="System.String"/> specifying the name of the framework.</param>
		internal static IntPtr LoadFramework(string name)
		{
			IntPtr handle;
			
			if (!frameworkDictionary.TryGetValue(name, out handle))
				foreach (var locationFormat in frameworkLocationFormats)
				{
					var location = string.Format(CultureInfo.InvariantCulture, locationFormat, name);
					
					if (File.Exists(location) 
				    		&& (handle = SafeNativeMethods.dlopen(location, SafeNativeMethods.RuntimeLoadMode.Lazy | SafeNativeMethods.RuntimeLoadMode.Global)) != IntPtr.Zero)
					{
						frameworkDictionary.Add(name, handle);
						break;
					}
				}
			
			if (handle == IntPtr.Zero)
				throw new FileNotFoundException();
			
			return handle;
		}
		
		[DebuggerStepThroughAttribute]
		internal static IntPtr GetSymbol(string framework, string name)
		{
			return SafeNativeMethods.dlsym(LoadFramework(framework), name);
		}
		
		[DebuggerStepThroughAttribute]
		public static IntPtr GetClass(string name)
		{
			IntPtr @class;
			
			if (!classDictionary.TryGetValue(name, out @class))
			{
				if ((@class = SafeNativeMethods.objc_lookUpClass(name)) == IntPtr.Zero)
					throw new ClassNotFoundException(name);
				classDictionary.Add(string.Intern(name), @class);
			}
			return @class;
		}
		
		[DebuggerStepThroughAttribute]
		public static IntPtr GetSelector(string name)
		{
			IntPtr sel;
			
			if (!selectorDictionary.TryGetValue(name, out sel))
				selectorDictionary.Add(string.Intern(name), sel = SafeNativeMethods.sel_getUid(name));
			return sel;
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool InstancesRespondToSelector(string @class, string selector)
		{
			if (@class == null)
				throw new ArgumentNullException("class");
			if (selector == null)
				throw new ArgumentNullException("selector");
			
			return InstancesRespondToSelector(GetClass(@class), GetSelector(selector));
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool InstancesRespondToSelector(IntPtr @class, string selector)
		{
			if (@class == IntPtr.Zero)
				throw new ArgumentNullException("class");
			if (selector == null)
				throw new ArgumentNullException("selector");
			
			return InstancesRespondToSelector(@class, GetSelector(selector));
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool InstancesRespondToSelector(IntPtr @class, IntPtr selector)
		{
			if (@class == IntPtr.Zero)
				throw new ArgumentNullException("class");
			if (selector == IntPtr.Zero)
				throw new ArgumentNullException("selector");
			
			return SafeNativeMethods.objc_msgSend_get_Boolean(@class, Selectors.InstancesRespondToSelector, selector);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool RespondsToSelector(IntPtr @object, string selector)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			
			return RespondsToSelector(@object, GetSelector(selector));
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool RespondsToSelector(IntPtr @object, IntPtr selector)
		{
			if (selector == IntPtr.Zero)
				throw new ArgumentNullException("selector");
			
			return SafeNativeMethods.objc_msgSend_get_Boolean(@object, Selectors.RespondsToSelector, selector);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool Is(IntPtr @object, string @class)
		{
			if (@class == null)
				throw new ArgumentNullException("class");
			
			return Is(@object, GetClass(@class));
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static bool Is(IntPtr @object, IntPtr @class)
		{
			if (@class == IntPtr.Zero)
				throw new ArgumentNullException("class");
			
			return SafeNativeMethods.objc_msgSend_get_Boolean(@object, Selectors.IsKindOfClass, @class);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocObject(string @class) { return AllocObject(GetClass(@class)); }
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocObject(IntPtr @class)
		{
			if (@class == IntPtr.Zero)
				throw new ArgumentNullException("class");
			
			return SafeNativeMethods.objc_msgSend(@class, Selectors.Alloc);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocAndInitObject(string @class) { return AllocAndInitObject(GetClass(@class)); }
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocAndInitObject(IntPtr @class)
		{
			if (@class == IntPtr.Zero)
				throw new ArgumentNullException("class");
			
			return SafeNativeMethods.objc_msgSend(SafeNativeMethods.objc_msgSend(@class, Selectors.Alloc), Selectors.Init);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void ReleaseObject(IntPtr @object)
		{
			if (@object == IntPtr.Zero)
				throw new ArgumentNullException("object");
			
			SafeNativeMethods.objc_msgSend(@object, Selectors.Release);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr RetainObject(IntPtr @object)
		{
			if (@object == IntPtr.Zero)
				throw new ArgumentNullException("object");
			
			return SafeNativeMethods.objc_msgSend(@object, Selectors.Retain);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AutoReleaseObject(IntPtr @object)
		{
			if (@object == IntPtr.Zero)
				throw new ArgumentNullException("object");
			
			return SafeNativeMethods.objc_msgSend(@object, Selectors.AutoRelease);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr RetainAndAutoReleaseObject(IntPtr @object)
		{
			if (@object == IntPtr.Zero)
				throw new ArgumentNullException("object");
			
			return SafeNativeMethods.objc_msgSend(SafeNativeMethods.objc_msgSend(@object, Selectors.Retain), Selectors.AutoRelease);
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal unsafe static IntPtr StringToNativeStringNoCopy(string str)
		{
			if (str == null)
				return IntPtr.Zero;
			
			IntPtr nativePointer = AllocObject(Classes.NSString);
			
			fixed (char* characters = str)
				nativePointer =  objc_msgSend_initWithCharactersNoCopy_length_freeWhenDone(nativePointer, NSString_initWithCharactersNoCopy_length_freeWhenDone, characters, str.Length, false);
			
			return nativePointer;
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static IntPtr StringToNativeString(string str)
		{
			if (str == null)
				return IntPtr.Zero;
			
			IntPtr nativePointer = AllocObject(Classes.NSString);
			
			fixed (char* characters = str)
				nativePointer =  objc_msgSend_initWithCharacters_length(nativePointer, NSString_initWithCharacters_length, characters, str.Length);
			
			return nativePointer;
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public unsafe static string NativeStringToString(IntPtr nstr)
		{
			if (nstr == IntPtr.Zero)
				return null;
			
			int length;
				
			// Dirty way of converting data types…
			// This is maybe not very reliable but probably the fastest way of doing so…
			// There is a safer and more supported way if really needed: using an intermediate char buffer.
			try { length = checked((int)SafeNativeMethods.objc_msgSend(nstr, Selectors.Length)); }
			catch (OverflowException) { length = Int32.MaxValue; }
			string str = new string('\0', length);
			
			fixed (char *characters = str)
				SafeNativeMethods.objc_msgSend(nstr, NSString_getCharacters_range, (IntPtr)characters, IntPtr.Zero, (IntPtr)length);
			
			return str;
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr UriToNativeUrl(Uri uri)
		{
			return uri != null ?
				SafeNativeMethods.objc_msgSend_get_ObjectFromKey(Classes.NSUrl, Selectors.UrlWithString, uri.OriginalString) :
				IntPtr.Zero;
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static Uri NativeUrlToUri(IntPtr url)
		{
			throw new NotImplementedException();
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public unsafe static IntPtr ArrayToNativeArray(IntPtr[] array)
		{
			if (array == null)
				return IntPtr.Zero;
			
			fixed (IntPtr* arrayPointer = array)
				return SafeNativeMethods.objc_msgSend(Classes.NSArray, Selectors.ArrayWithObjectsCount, (IntPtr)arrayPointer, checked((IntPtr)array.Length));
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static IntPtr RegisterObjectPair(object @object, IntPtr nativePointer)
		{
			if (@object == null) return IntPtr.Zero;
			
			lock (managedToNativeDictionary)
			{
				managedToNativeDictionary.Add(@object, nativePointer);
				nativeToManagedDictionary.Add(nativePointer, @object);
			}
			
			return nativePointer;
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static void UnregisterObject(object @object)
		{
			IntPtr nativePointer;
			
			if (@object == null) return;
			
			lock (managedToNativeDictionary)
			{
				if (managedToNativeDictionary.TryGetValue(@object, out nativePointer))
				{
					managedToNativeDictionary.Remove(@object);
					nativeToManagedDictionary.Remove(nativePointer);
				}
			}
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr GetNativeObject(object @object)
		{
			IntPtr nativePointer;
			
			if (@object == null) return IntPtr.Zero;
			
			lock (managedToNativeDictionary)
			{
				if (!managedToNativeDictionary.TryGetValue(@object, out nativePointer))
				{
					if (GetNativeClass(@object.GetType(), false) != Classes.ClrObject) return IntPtr.Zero;
					
					nativePointer = SafeNativeMethods.objc_msgSend(AllocObject(Classes.ClrObject), Selectors.Init);
					managedToNativeDictionary.Add(@object, nativePointer);
					nativeToManagedDictionary.Add(nativePointer, @object);
				}
			}
			
			return nativePointer;
		}
		
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static object GetManagedObject(IntPtr nativePointer)
		{
			object @object;
			
			if (nativePointer == IntPtr.Zero) return null;
			
			lock (managedToNativeDictionary)
				nativeToManagedDictionary.TryGetValue(nativePointer, out @object);
			
			return @object;
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static string GetNotificationName(IntPtr notification)
		{
			return SafeNativeMethods.objc_msgSend_get_String(notification, Selectors.Name);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr GetNotificationObject(IntPtr notification)
		{
			return SafeNativeMethods.objc_msgSend(notification, Selectors.Object);
		}
		
		[DebuggerStepThroughAttribute]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr GetNotificationUserInfo(IntPtr notification)
		{
			return SafeNativeMethods.objc_msgSend(notification, Selectors.UserInfo);
		}
	}
}
