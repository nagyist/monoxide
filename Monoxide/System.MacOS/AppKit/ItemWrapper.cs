using System;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	internal struct ItemWrapper : IDisposable
	{
		private static readonly Dictionary<IntPtr, ICommandItem> itemDictionary = new Dictionary<IntPtr, ICommandItem>();
		
		[SelectorStub("target")]
		private static IntPtr GetCommandTarget(IntPtr self, IntPtr _cmd)
		{
			var item = itemDictionary[self] as MenuItem;
			
			if (item != null && item.Command != null)
				return item.CommandTarget.NativePointer;
			else
				return IntPtr.Zero;
		}
		
		[SelectorStub("action")]
		private static IntPtr GetCommand(IntPtr self, IntPtr _cmd)
		{
			var item = itemDictionary[self];
			
			if (item != null && item.Command != null)
				return ObjectiveC.GetSelector(item.Command.SelectorName);
			else
				return IntPtr.Zero;
		}
		
		[SelectorStub("tag")]
		private static IntPtr GetTag(IntPtr self, IntPtr _cmd)
		{
			return IntPtr.Zero;
		}
		
		public static ItemWrapper Create(ICommandItem item)
		{
			var @this = new ItemWrapper();
			
			@this.NativePointer = ObjectiveC.AllocAndInitObject(ObjectiveC.GetNativeClass(typeof(ItemWrapper), true));
			lock (itemDictionary) itemDictionary.Add(@this.NativePointer, item);
			
			return @this;
		}
		
		public void Dispose()
		{
			if (NativePointer != IntPtr.Zero)
			{
				lock (itemDictionary) itemDictionary.Remove(NativePointer);
				ObjectiveC.ReleaseObject(NativePointer);
				NativePointer = IntPtr.Zero;
			}
		}
		
		internal IntPtr NativePointer { get; private set; }
	}
}
