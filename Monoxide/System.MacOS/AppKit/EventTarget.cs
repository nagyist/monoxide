using System;

namespace System.MacOS.AppKit
{
	[NativeClass("NSResponder", "AppKit")]
	public abstract class EventTarget : CommandTarget
	{
		public EventTarget()
			: this(true) { }
		
		internal EventTarget(bool subclass)
			: base(subclass) { }
	}
}
