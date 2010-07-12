using System;
using System.Runtime.InteropServices;

namespace System.MacOS.AppKit
{
	[NativeClass("NSImage", "AppKit")]
	public class Image
	{
		#region Method Selector Ids
		
		static class Selectors
		{
			static class imageNamed { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("imageNamed:"); }
			
			public static IntPtr ImageNamed { get { return imageNamed.SelectorHandle; } }
		}
		
		#endregion
		
		#region Cache
		
		static readonly NativeObjectCache<Image> imageCache = new NativeObjectCache<Image>(i => i.NativePointer, i => new Image(i));
		
		internal static Image GetInstance(IntPtr nativePointer) { return imageCache.GetObject(nativePointer); }
		
		#endregion
		
		private static Image GetImage(IntPtr name)
		{
			return GetInstance(SafeNativeMethods.objc_msgSend(CommonClasses.NSImage, Selectors.ImageNamed, name));
		}
		
		// Image Template Constants
		private static readonly IntPtr NSImageNameQuickLookTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameQuickLookTemplate"));
		private static readonly IntPtr NSImageNameBluetoothTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameBluetoothTemplate"));
		private static readonly IntPtr NSImageNameIChatTheaterTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameIChatTheaterTemplate"));
		private static readonly IntPtr NSImageNameSlideshowTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameSlideshowTemplate"));
		private static readonly IntPtr NSImageNameActionTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameActionTemplate"));
		private static readonly IntPtr NSImageNameSmartBadgeTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameSmartBadgeTemplate"));
		private static readonly IntPtr NSImageNamePathTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNamePathTemplate"));
		private static readonly IntPtr NSImageNameInvalidDataFreestandingTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameInvalidDataFreestandingTemplate"));
		private static readonly IntPtr NSImageNameLockLockedTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameLockLockedTemplate"));
		private static readonly IntPtr NSImageNameLockUnlockedTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameLockUnlockedTemplate"));
		private static readonly IntPtr NSImageNameGoRightTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameGoRightTemplate"));
		private static readonly IntPtr NSImageNameGoLeftTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameGoLeftTemplate"));
		private static readonly IntPtr NSImageNameRightFacingTriangleTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameRightFacingTriangleTemplate"));
		private static readonly IntPtr NSImageNameLeftFacingTriangleTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameLeftFacingTriangleTemplate"));
		private static readonly IntPtr NSImageNameAddTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameAddTemplate"));
		private static readonly IntPtr NSImageNameRemoveTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameRemoveTemplate"));
		private static readonly IntPtr NSImageNameRevealFreestandingTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameRevealFreestandingTemplate"));
		private static readonly IntPtr NSImageNameFollowLinkFreestandingTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameFollowLinkFreestandingTemplate"));
		private static readonly IntPtr NSImageNameEnterFullScreenTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameEnterFullScreenTemplate"));
		private static readonly IntPtr NSImageNameExitFullScreenTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameExitFullScreenTemplate"));
		private static readonly IntPtr NSImageNameStopProgressTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameStopProgressTemplate"));
		private static readonly IntPtr NSImageNameStopProgressFreestandingTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameStopProgressFreestandingTemplate"));
		private static readonly IntPtr NSImageNameRefreshTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameRefreshTemplate"));
		private static readonly IntPtr NSImageNameRefreshFreestandingTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameRefreshFreestandingTemplate"));
		
		// Multiple Documents Drag Image
		private static readonly IntPtr NSImageNameMultipleDocuments = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameMultipleDocuments"));
		
		// Sharing Permissions Named Images
		private static readonly IntPtr NSImageNameUser = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameUser"));
		private static readonly IntPtr NSImageNameUserGroup = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameUserGroup"));
		private static readonly IntPtr NSImageNameEveryone = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameEveryone"));
		
		// System Entity Images
		private static readonly IntPtr NSImageNameBonjour = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameBonjour"));
		private static readonly IntPtr NSImageNameDotMac = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameDotMac"));
		private static readonly IntPtr NSImageNameComputer = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameComputer"));
		private static readonly IntPtr NSImageNameFolderBurnable = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameFolderBurnable"));
		private static readonly IntPtr NSImageNameFolderSmart = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameFolderSmart"));
		private static readonly IntPtr NSImageNameNetwork = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameNetwork"));
		
		// Toolbar Named Images
		private static readonly IntPtr NSImageNameUserAccounts = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameUserAccounts"));
		private static readonly IntPtr NSImageNamePreferencesGeneral = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNamePreferencesGeneral"));
		private static readonly IntPtr NSImageNameAdvanced = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameAdvanced"));
		private static readonly IntPtr NSImageNameInfo = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameInfo"));
		private static readonly IntPtr NSImageNameFontPanel = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameFontPanel"));
		private static readonly IntPtr NSImageNameColorPanel = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameColorPanel"));
		#if MACOSX_10_6
		private static readonly IntPtr NSImageNameFolder = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameFolder"));
		private static readonly IntPtr NSImageNameTrashEmpty = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameTrashEmpty"));
		private static readonly IntPtr NSImageNameTrashFull = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameTrashFull"));
		private static readonly IntPtr NSImageNameHomeTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameHomeTemplate"));
		private static readonly IntPtr NSImageNameBookmarksTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameBookmarksTemplate"));
		private static readonly IntPtr NSImageNameCaution = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameCaution"));
		private static readonly IntPtr NSImageNameStatusAvailable = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameStatusAvailable"));
		private static readonly IntPtr NSImageNameStatusPartiallyAvailable = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameStatusPartiallyAvailable"));
		private static readonly IntPtr NSImageNameStatusUnavailable = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameStatusUnavailable"));
		private static readonly IntPtr NSImageNameStatusNone = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameStatusNone"));
		private static readonly IntPtr NSImageNameApplicationIcon = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameApplicationIcon"));
		private static readonly IntPtr NSImageNameMenuOnStateTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameMenuOnStateTemplate"));
		private static readonly IntPtr NSImageNameMenuMixedStateTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameMenuMixedStateTemplate"));
		private static readonly IntPtr NSImageNameUserGuest = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameUserGuest"));
		private static readonly IntPtr NSImageNameMobileMe = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameMobileMe"));
		#endif
		
		// View Type Template Images
		private static readonly IntPtr NSImageNameIconViewTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameIconViewTemplate"));
		private static readonly IntPtr NSImageNameListViewTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameListViewTemplate"));
		private static readonly IntPtr NSImageNameColumnViewTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameColumnViewTemplate"));
		private static readonly IntPtr NSImageNameFlowViewTemplate = Marshal.ReadIntPtr(ObjectiveC.GetSymbol("AppKit", "NSImageNameFlowViewTemplate"));
		
		// Image Template Constants
		public static Image QuickLookTemplate { get { return GetImage(NSImageNameQuickLookTemplate); } }
		public static Image BluetoothTemplate { get { return GetImage(NSImageNameBluetoothTemplate); } }
		public static Image IChatTheaterTemplate { get { return GetImage(NSImageNameIChatTheaterTemplate); } }
		public static Image SlideshowTemplate { get { return GetImage(NSImageNameSlideshowTemplate); } }
		public static Image ActionTemplate { get { return GetImage(NSImageNameActionTemplate); } }
		public static Image SmartBadgeTemplate { get { return GetImage(NSImageNameSmartBadgeTemplate); } }
		public static Image PathTemplate { get { return GetImage(NSImageNamePathTemplate); } }
		public static Image InvalidDataFreestandingTemplate { get { return GetImage(NSImageNameInvalidDataFreestandingTemplate); } }
		public static Image LockLockedTemplate { get { return GetImage(NSImageNameLockLockedTemplate); } }
		public static Image LockUnlockedTemplate { get { return GetImage(NSImageNameLockUnlockedTemplate); } }
		public static Image GoRightTemplate { get { return GetImage(NSImageNameGoRightTemplate); } }
		public static Image GoLeftTemplate { get { return GetImage(NSImageNameGoLeftTemplate); } }
		public static Image RightFacingTriangleTemplate { get { return GetImage(NSImageNameRightFacingTriangleTemplate); } }
		public static Image LeftFacingTriangleTemplate { get { return GetImage(NSImageNameLeftFacingTriangleTemplate); } }
		public static Image AddTemplate { get { return GetImage(NSImageNameAddTemplate); } }
		public static Image RemoveTemplate { get { return GetImage(NSImageNameRemoveTemplate); } }
		public static Image RevealFreestandingTemplate { get { return GetImage(NSImageNameRevealFreestandingTemplate); } }
		public static Image FollowLinkFreestandingTemplate { get { return GetImage(NSImageNameFollowLinkFreestandingTemplate); } }
		public static Image EnterFullScreenTemplate { get { return GetImage(NSImageNameEnterFullScreenTemplate); } }
		public static Image ExitFullScreenTemplate { get { return GetImage(NSImageNameExitFullScreenTemplate); } }
		public static Image StopProgressTemplate { get { return GetImage(NSImageNameStopProgressTemplate); } }
		public static Image StopProgressFreestandingTemplate { get { return GetImage(NSImageNameStopProgressFreestandingTemplate); } }
		public static Image RefreshTemplate { get { return GetImage(NSImageNameRefreshTemplate); } }
		public static Image RefreshFreestandingTemplate { get { return GetImage(NSImageNameRefreshFreestandingTemplate); } }
		
		// Multiple Documents Drag Image
		public static Image MultipleDocuments { get { return GetImage(NSImageNameMultipleDocuments); } }
		
		// Sharing Permissions Named Images
		public static Image User { get { return GetImage(NSImageNameUser); } }
		public static Image UserGroup { get { return GetImage(NSImageNameUserGroup); } }
		public static Image Everyone { get { return GetImage(NSImageNameEveryone); } }
		
		// System Entity Images
		public static Image Bonjour { get { return GetImage(NSImageNameBonjour); } }
		public static Image DotMac { get { return GetImage(NSImageNameDotMac); } }
		public static Image Computer { get { return GetImage(NSImageNameComputer); } }
		public static Image FolderBurnable { get { return GetImage(NSImageNameFolderBurnable); } }
		public static Image FolderSmart { get { return GetImage(NSImageNameFolderSmart); } }
		public static Image Network { get { return GetImage(NSImageNameNetwork); } }
		
		// Toolbar Named Images
		public static Image UserAccounts { get { return GetImage(NSImageNameUserAccounts); } }
		public static Image PreferencesGeneral { get { return GetImage(NSImageNamePreferencesGeneral); } }
		public static Image Advanced { get { return GetImage(NSImageNameAdvanced); } }
		public static Image Info { get { return GetImage(NSImageNameInfo); } }
		public static Image FontPanel { get { return GetImage(NSImageNameFontPanel); } }
		public static Image ColorPanel { get { return GetImage(NSImageNameColorPanel); } }
		#if MACOSX_10_6
		public static Image Folder { get { return GetImage(NSImageNameFolder); } }
		public static Image TrashEmpty { get { return GetImage(NSImageNameTrashEmpty); } }
		public static Image TrashFull { get { return GetImage(NSImageNameTrashFull); } }
		public static Image HomeTemplate { get { return GetImage(NSImageNameHomeTemplate); } }
		public static Image BookmarksTemplate { get { return GetImage(NSImageNameBookmarksTemplate); } }
		public static Image Caution { get { return GetImage(NSImageNameCaution); } }
		public static Image StatusAvailable { get { return GetImage(NSImageNameStatusAvailable); } }
		public static Image StatusPartiallyAvailable { get { return GetImage(NSImageNameStatusPartiallyAvailable); } }
		public static Image StatusUnavailable { get { return GetImage(NSImageNameStatusUnavailable); } }
		public static Image StatusNone { get { return GetImage(NSImageNameStatusNone); } }
		public static Image ApplicationIcon { get { return GetImage(NSImageNameApplicationIcon); } }
		public static Image MenuOnStateTemplate { get { return GetImage(NSImageNameMenuOnStateTemplate); } }
		public static Image MenuMixedStateTemplate { get { return GetImage(NSImageNameMenuMixedStateTemplate); } }
		public static Image UserGuest { get { return GetImage(NSImageNameUserGuest); } }
		public static Image MobileMe { get { return GetImage(NSImageNameMobileMe); } }
		#endif
		
		// View Type Template Images
		public static Image IconViewTemplate { get { return GetImage(NSImageNameIconViewTemplate); } }
		public static Image ListViewTemplate { get { return GetImage(NSImageNameListViewTemplate); } }
		public static Image ColumnViewTemplate { get { return GetImage(NSImageNameColumnViewTemplate); } }
		public static Image FlowViewTemplate { get { return GetImage(NSImageNameFlowViewTemplate); } }
		
		SafeNativeMethods.objc_super super;
		bool disposed;
		
//		public Image()
//		{
//			var nativeClass = ObjectiveC.GetNativeClass(this.GetType(), true);
//			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
//			super.Receiver = ObjectiveC.AllocAndInitObject(nativeClass);
//		}
		
		private Image(IntPtr nativePointer)
		{
			super.Class = ObjectiveC.GetNativeBaseClass(ObjectiveC.GetNativeClass(this.GetType(), false));
			super.Receiver = nativePointer;
			ObjectiveC.RetainObject(nativePointer);
		}
		
		~Image() { Dispose(false); }
		
		protected void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				imageCache.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
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
				
				return super.Receiver;
			}
		}
	}
}
