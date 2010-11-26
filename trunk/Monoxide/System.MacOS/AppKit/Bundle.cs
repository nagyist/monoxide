using System;
using System.IO;
using System.Reflection;

namespace System.MacOS.AppKit
{
	[NativeClass("NSBundle", "AppKit")]
	public class Bundle : IDisposable
	{
		#region Method Selector Ids
		
		static class Selectors
		{
			static class mainBundle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("mainBundle"); }
			static class bundleWithPath { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("bundleWithPath:"); }

			static class bundlePath { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("bundlePath"); }
			
			static class infoDictionary { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("infoDictionary"); }
			static class objectForInfoDictionaryKey { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("objectForInfoDictionaryKey:"); }
						
			public static IntPtr MainBundle { get { return mainBundle.SelectorHandle; } }
			public static IntPtr BundleWithPath { get { return bundleWithPath.SelectorHandle; } }

			public static IntPtr BundlePath { get { return bundlePath.SelectorHandle; } }
			
			public static IntPtr InfoDictionary { get { return infoDictionary.SelectorHandle; } }
			public static IntPtr ObjectForInfoDictionaryKey { get { return objectForInfoDictionaryKey.SelectorHandle; } }
		}
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<Bundle> bundleCache = new NativeObjectCache<Bundle>(b => b.NativePointer, p => new Bundle(p));
		
		internal static Bundle GetInstance(IntPtr nativePointer) { return bundleCache.GetObject(nativePointer); }
		
		#endregion
		
		static class mainBundle
		{
			public static readonly Bundle Instance = GetInstance(SafeNativeMethods.objc_msgSend(CommonClasses.NSBundle, Selectors.MainBundle));
		}

		static class entryAssemblyBundle
		{
			public static readonly Bundle Instance = GetAssemblyBundle(Assembly.GetEntryAssembly());
		}
		
		public static Bundle MainBundle { get { return mainBundle.Instance; } }

		public static Bundle FromPath(string path)
		{
			if (!Directory.Exists(path))
				throw new ArgumentException();
			return GetInstance(SafeNativeMethods.objc_msgSend_get_ObjectFromKey(CommonClasses.NSBundle, Selectors.BundleWithPath, path));
		}

		#warning Remove the System.IO. prefix when mcs is fixed…
		private static string DetectBundlePath(string path)
		{
			if (string.IsNullOrEmpty(path)) return null;

			if (File.Exists(path)) return DetectBundlePath(System.IO.Path.GetDirectoryName(path));

			do
			{
				var name = System.IO.Path.GetFileName(path);
	
				if (string.IsNullOrEmpty(name)) return DetectBundlePath(System.IO.Path.GetDirectoryName(path));
	
				if (name.Contains(".")) return path;

				path = System.IO.Path.GetDirectoryName(path);
			}
			while (!string.IsNullOrEmpty(path));

			return null;
		}

		private static Bundle GetAssemblyBundle(Assembly assembly)
		{
			return FromPath(DetectBundlePath(assembly.Location));
		}
		
		IntPtr nativePointer;
		bool disposed;
		
		private Bundle()
		{
			ObjectiveC.EnsureAppKitFrameworkIsLoaded();
			nativePointer = ObjectiveC.AllocObject("NSBundle");
			nativePointer = SafeNativeMethods.objc_msgSend(nativePointer, ObjectiveC.Selectors.Init);
			bundleCache.RegisterObject(this);
		}
		
		private Bundle(IntPtr nativePointer)
		{
			this.nativePointer = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~Bundle() { Dispose(false); }
		
		protected void Dispose(bool disposing)
		{
			if (nativePointer != IntPtr.Zero)
			{
				bundleCache.UnregisterObject(nativePointer);
				ObjectiveC.ReleaseObject(nativePointer);
				nativePointer = IntPtr.Zero;
			}
			disposed = true;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal IntPtr NativePointer
		{
			get
			{
				if (Disposed)
					throw new ObjectDisposedException(this.GetType().Name);
				
				return nativePointer;
			}
		}
		
		private string GetValueForKey(string key)
		{
			IntPtr objectNativePointer = SafeNativeMethods.objc_msgSend_get_ObjectFromKey(NativePointer, Selectors.ObjectForInfoDictionaryKey, key);
			if (ObjectiveC.Is(objectNativePointer, ObjectiveC.Classes.NSString))
				return ObjectiveC.NativeStringToString(objectNativePointer);
			else
				return null; // Unknown or unhandled data type
		}

		internal IntPtr GetDictionaryCopy()
		{
			return ObjectiveC.MutableCopy(SafeNativeMethods.objc_msgSend(NativePointer, Selectors.InfoDictionary));
		}
		
		public string Name { get { return GetValueForKey("CFBundleName") as string; } }

		public string Path { get { return SafeNativeMethods.objc_msgSend_get_String(NativePointer, Selectors.BundlePath); } }
	}
}
