using System;

namespace System.MacOS
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class NativeClassAttribute : Attribute
	{
		public NativeClassAttribute(string nativeClass)
			: this(nativeClass, null) { }
		
		public NativeClassAttribute(string nativeClass, string framework)
		{
			if (nativeClass == null)
				throw new ArgumentNullException("nativeClass");
			
			if (framework != null) ObjectiveC.LoadFramework(framework);
			Class = ObjectiveC.GetClass(nativeClass);
			Framework = framework;
		}
		
		public IntPtr Class { get; private set; }
		public string Framework { get; private set; }
	}
}
