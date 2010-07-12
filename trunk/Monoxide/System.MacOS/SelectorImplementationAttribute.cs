using System;

namespace System.MacOS
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class SelectorImplementationAttribute : Attribute
	{
		public SelectorImplementationAttribute(string selector)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			Selector = ObjectiveC.GetSelector(selector);
		}
		
		public IntPtr Selector { get; private set; }
		
		public BridgeMode BridgeMode { get; set; }
	}
}
