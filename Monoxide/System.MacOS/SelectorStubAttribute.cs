using System;

namespace System.MacOS
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal class SelectorStubAttribute : Attribute
	{
		public SelectorStubAttribute(string selector)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			Selector = ObjectiveC.GetSelector(selector);
		}
		
		public IntPtr Selector { get; private set; }
		public StubKind Kind { get; set; }
	}
}
