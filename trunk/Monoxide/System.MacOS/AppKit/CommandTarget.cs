using System;
using System.Diagnostics;

namespace System.MacOS.AppKit
{
	public abstract class CommandTarget : IDisposable
	{
		[SelectorStub("validateUserInterfaceItem:")]
		//[SelectorStub("validateMenuItem:")]
		private static bool ValidateUserInterfaceItem(IntPtr self, IntPtr _cmd, IntPtr item)
		{
			// If commandTarget ever happens to be null, it means the corresponding managed object has not been registered.
			// Since this would obviously be a bug, we leave the runtime free of generating a NullReferenceException.
			var commandTarget = ObjectiveC.GetManagedObject(self) as CommandTarget;
			var commandItem = ObjectiveC.GetManagedObject(item) as ICommandItem;
			
			if (commandItem != null)
				return commandTarget.ValidateItem(commandItem);
			else if (item != IntPtr.Zero && ObjectiveC.InstancesRespondToSelector(commandTarget.super.Class, "validateUserInterfaceItem:"))
			{
				if (commandTarget.super.Receiver == IntPtr.Zero) commandTarget.super.Receiver = commandTarget.NativePointer;
				return SafeNativeMethods.objc_msgSendSuper_get_Boolean(ref commandTarget.super, ObjectiveC.GetSelector("validateUserInterfaceItem:"), item);
			}
			else
				return false;
		}
		
		IntPtr nativeClass;
		internal SafeNativeMethods.objc_super super;
		bool disposed;
		
		public CommandTarget()
			: this(true) { }
		
		internal CommandTarget(bool subclass)
		{
			nativeClass = ObjectiveC.GetNativeClass(this.GetType(), subclass);
			super.Class = ObjectiveC.GetNativeBaseClass(nativeClass);
		}
		
		~CommandTarget()
		{
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (super.Receiver != IntPtr.Zero)
			{
				ObjectiveC.UnregisterObject(super.Receiver);
				ObjectiveC.ReleaseObject(super.Receiver);
				super.Receiver = IntPtr.Zero;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			disposed = true;
			GC.SuppressFinalize(this);
		}
		
		public bool Disposed { get { return disposed; } }
		
		internal bool Created { get { return super.Receiver != IntPtr.Zero; } }
		
		internal IntPtr NativeClass { get { return nativeClass; } }
		
		internal virtual IntPtr NativePointer
		{
			get
			{
				if (disposed)
					throw new ObjectDisposedException(GetType().Name);
				
				var nativePointer = ObjectiveC.GetNativeObject(this);
				
				if (nativePointer == IntPtr.Zero)
					ObjectiveC.RegisterObjectPair(this, nativePointer = ObjectiveC.AllocAndInitObject(NativeClass));
				
				return nativePointer;
			}
		}
		
		public virtual void Execute(Command command, object sender)
		{
			if (CommandCache.IsSupported(this.GetType(), command.Name))
				CommandCache.Invoke(command.Name, this, sender);
		}
		
		public virtual bool CanExecute(Command command) { return CommandCache.IsSupported(this.GetType(), command.Name); }
		public virtual bool ValidateCommand(Command command) { return CanExecute(command); }
		
		public virtual bool ValidateItem(ICommandItem item)
		{	
			if (item.Command != null)
				return ValidateCommand(item.Command);
			else if (ObjectiveC.InstancesRespondToSelector(super.Class, CommonSelectors.ValidateUserInterfaceItem))
			{
				if (super.Receiver == IntPtr.Zero) super.Receiver = NativePointer;
				using (var wrapper = ItemWrapper.Create(item))
					return SafeNativeMethods.objc_msgSendSuper_get_Boolean(ref super, CommonSelectors.ValidateUserInterfaceItem, wrapper.NativePointer);
			}
			else
				return false;
		}
	}
}
