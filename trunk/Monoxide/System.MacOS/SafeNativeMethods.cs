using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace System.MacOS
{
	static class SafeNativeMethods
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
		
		[Flags]
		public enum RuntimeLoadMode
		{
			Lazy = 1,
			Now = 2,
			Global = 4,
			Local = 8
		}
		
		public enum ApplicationTerminateReply {
			TerminateCancel = 0,
			TerminateNow = 1,
			TerminateLater = 2
		}
		
		public struct objc_super
		{
			public IntPtr Receiver;
			public IntPtr Class;
			
			public objc_super(IntPtr receiver, IntPtr @class)
			{
				Receiver = receiver;
				Class = @class;
			}
		}
		
		[SuppressUnmanagedCodeSecurity]
		public delegate void StandardEventHandler(IntPtr self, IntPtr _cmd, IntPtr obj);
		[SuppressUnmanagedCodeSecurity]
		public delegate IntPtr EventHandlerWithIntPtrResult(IntPtr self, IntPtr _cmd, IntPtr obj);
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public delegate bool EventHandlerWithBooleanResult(IntPtr self, IntPtr _cmd, IntPtr obj);
		
		[DllImport("libSystem")]
        [SuppressUnmanagedCodeSecurity]
		public static extern IntPtr dlopen(string path, RuntimeLoadMode mode);
		[DllImport("libSystem")]
        [SuppressUnmanagedCodeSecurity]
		public static extern IntPtr dlsym(IntPtr handle, string name);
		[DllImport("libSystem")]
        [SuppressUnmanagedCodeSecurity]
		public static extern int dlclose(IntPtr handle);
		
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode GetCurrentProcess (out long psn);
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode TransformProcessType ([In] ref long psn, ProcessApplicationTransformState type);
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		[SuppressUnmanagedCodeSecurity]
		public static extern OSResultCode SetFrontProcess ([In] ref long psn);
		
		[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void NSBeep();
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		public static extern IntPtr CFLocaleCopyCurrent();
		[SuppressUnmanagedCodeSecurity]
		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", EntryPoint = "CFLocaleGetValue")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler), MarshalCookie = "n")]
		public static extern string CFLocaleGetStringValue(IntPtr locale, IntPtr key);
		
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringResultMarshaler))]
		public static extern string sel_getName(IntPtr sel);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr sel_registerName([In] string str);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr sel_getUid([In] string str);
		
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public extern static IntPtr object_getClass(IntPtr obj);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringResultMarshaler))]
		public extern static string object_getClassName(IntPtr obj);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public extern static IntPtr object_getInstanceVariable(IntPtr obj, string name, out IntPtr value);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public extern static IntPtr object_getIvar(IntPtr obj, IntPtr ivar);
		
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_lookUpClass([In] string name);
		
		#region Generic Message Sending
		
		// These functions should be used with great care, it is easy to mess-up the stack by calling a wrong overload.
		// If a class-specific objc_msgSend version is needed, declare it in the appropriate place and use it instead of these. 
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, IntPtr obj);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, IntPtr obj, [MarshalAs(UnmanagedType.I1)] bool b);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, IntPtr obj1, IntPtr obj2);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, IntPtr obj1, IntPtr obj2, [MarshalAs(UnmanagedType.I1)] bool b);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, IntPtr obj1, IntPtr obj2, IntPtr obj3);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, IntPtr obj1, IntPtr obj2, IntPtr obj3, [MarshalAs(UnmanagedType.I1)] bool b);
		// Mono don't support __arglist for interop yet...
		//[DllImport("libobjc")]
		//[SuppressUnmanagedCodeSecurity]
		//public static extern IntPtr objc_msgSend(IntPtr self, IntPtr sel, __arglist);
		
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSendSuper(ref objc_super super, IntPtr sel);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSendSuper(ref objc_super super, IntPtr sel, IntPtr obj);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSendSuper(ref objc_super super, IntPtr sel, IntPtr obj1, IntPtr obj2);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSendSuper(ref objc_super super, IntPtr sel, IntPtr obj1, IntPtr obj2, IntPtr obj3);

		#region Property Getters and Setters
		
		// These objc_msgSend versions can be used with Objective-C getters and setters, and other methods following the same prototype.
		// Getters and setters are usually named <property> and set<Property>, respectively.
		
		#region Boolean
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSend_get_Boolean(IntPtr self, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSend_get_Boolean(IntPtr self, IntPtr sel, IntPtr obj);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSend_get_Boolean(IntPtr self, IntPtr sel, IntPtr obj1, IntPtr obj2);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSend_get_Boolean(IntPtr self, IntPtr sel, IntPtr obj1, IntPtr obj2, IntPtr obj3);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_Boolean(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.I1)] bool b);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_Boolean(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.I1)] bool b, IntPtr obj);
		
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSendSuper_get_Boolean(ref objc_super super, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSendSuper_get_Boolean(ref objc_super super, IntPtr sel, IntPtr obj);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSendSuper_get_Boolean(ref objc_super super, IntPtr sel, IntPtr obj1, IntPtr obj2);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool objc_msgSendSuper_get_Boolean(ref objc_super super, IntPtr sel, IntPtr obj1, IntPtr obj2, IntPtr obj3);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSendSuper_set_Boolean(ref objc_super super, IntPtr sel, [MarshalAs(UnmanagedType.I1)] bool b);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSendSuper_set_Boolean(ref objc_super super, IntPtr sel, [MarshalAs(UnmanagedType.I1)] bool b, IntPtr obj);
		
		#endregion
		
		#region String
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler), MarshalCookie = "n")]
		public static extern string objc_msgSend_get_String(IntPtr self, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_String(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string str);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_String(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string str, IntPtr obj);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSend_get_ObjectFromKey(IntPtr self, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string key);
		
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler), MarshalCookie = "n")]
		public static extern string objc_msgSendSuper_get_String(ref objc_super super, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSendSuper_set_String(ref objc_super super, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string str);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSendSuper_set_String(ref objc_super super, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string str, IntPtr obj);
		[DllImport("libobjc", EntryPoint = "objc_msgSendSuper")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_msgSendSuper_get_ObjectFromKey(ref objc_super super, IntPtr sel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeStringMarshaler))] string key);
		
		#endregion
		
		#region Int32
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int objc_msgSend_get_Int32(IntPtr self, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_Int32(IntPtr self, IntPtr sel, int i);
		
		#endregion
		
		#region Int64
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern long objc_msgSend_get_Int64(IntPtr self, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_Int64(IntPtr self, IntPtr sel, long l);
		
		#endregion
		
		#region Rectangle
		
		[DllImport("libobjc", EntryPoint = "objc_msgSend_stret")]
		[SuppressUnmanagedCodeSecurity]
		public static extern AppKit.RectangleF objc_msgSend_get_Rectangle_32(IntPtr self, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSend_stret")]
		[SuppressUnmanagedCodeSecurity]
		public static extern AppKit.Rectangle objc_msgSend_get_Rectangle_64(IntPtr self, IntPtr sel);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_Rectangle_32(IntPtr self, IntPtr sel, AppKit.RectangleF rect);
		[DllImport("libobjc", EntryPoint = "objc_msgSend")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_msgSend_set_Rectangle_64(IntPtr self, IntPtr sel, AppKit.Rectangle rect);
		
		#endregion
		
		#endregion
		
		#endregion
		
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public extern static IntPtr objc_getClass([In] string name);
		
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr objc_allocateClassPair(IntPtr superclass, [In] string name, IntPtr extraBytes);
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		public static extern void objc_registerClassPair(IntPtr cls);
		
		[DllImport("libobjc")] 
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringResultMarshaler))]
		public extern static string class_getName(IntPtr @class);
		[DllImport("libobjc")] 
		[SuppressUnmanagedCodeSecurity]
		public extern static IntPtr class_getInstanceSize(IntPtr @class); // This could be used to inform the GC of allocated objects…
		[DllImport("libobjc")]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool class_addMethod(IntPtr cls, IntPtr sel, [In, MarshalAs(UnmanagedType.FunctionPtr)] Delegate imp, [In] string types);
	}
}
