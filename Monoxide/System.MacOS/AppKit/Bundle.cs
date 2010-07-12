using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSBundle", "AppKit")]
	public class Bundle : IDisposable
	{
		#region Method Selector Ids
		
		static class Selectors
		{
			static class mainBundle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("mainBundle"); }
			
			static class objectForInfoDictionaryKey { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("objectForInfoDictionaryKey:"); }
						
			public static IntPtr MainBundle { get { return mainBundle.SelectorHandle; } }
			
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
		
		public static Bundle MainBundle { get { return mainBundle.Instance; } }
		
		IntPtr nativePointer;
		bool disposed;
		
		public Bundle()
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
		
		public string Name
		{
			get
			{
				return GetValueForKey("CFBundleName") as string;
			}
		}
	}
}
