using System;

namespace System.MacOS.AppKit
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CommandAttribute : Attribute
	{
		public CommandAttribute(string commandName) { CommandName = commandName; }
		
		public string CommandName { get; private set; }
	}
}
