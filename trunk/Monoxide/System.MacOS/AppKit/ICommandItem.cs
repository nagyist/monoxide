using System;

namespace System.MacOS.AppKit
{
	public interface ICommandItem
	{
		Command Command { get; }
		object Tag { get; }
	}
}
