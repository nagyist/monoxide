using System;

namespace System.MacOS
{
	/// <summary>Describes the way a selector will be bridged.</summary>
	/// <remarks>
	/// Bridging is mainly used for overriding Objective-C objects' methods in the CLR.
	/// But bridging is costly, and thus should be avoided as much as possible.
	/// This enum, together with the <see cref="SelectorImplementationAttribute"/>, specifies how and when the bridging will
	/// be done.
	/// </remarks>
	internal enum BridgeMode
	{
		/// <summary>Specifies that the bridging will be done on method override.</summary>
		/// <remarks>
		/// The bridging can be done in any derived class if the decorated method is overriden.
		/// This is the default value.
		/// </remarks>
		Automatic = 0,
		/// <summary>Specifies that the bridging will be done on next method override.</summary>
		/// <remarks>This works like <c>Automatic</c>, delays the bridging to the next derived class.</remarks>
		Skip,
		/// <summary>Specifies that the bridging will always be done.</summary>
		/// <remarks>This will force the bridging directly in the class where it is used.</remarks>
		Force,
		/// <summary>Specifies that the bridging will never be done.</summary>
		/// <remarks>This can be used when sealing methods in derived classes.</remarks>
		None
	}
}
