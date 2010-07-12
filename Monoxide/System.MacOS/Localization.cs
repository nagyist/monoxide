using System;
using System.MacOS.AppKit;
using System.Resources;
using System.Diagnostics;
using System.Globalization;

namespace System.MacOS
{
	internal static class Localization
	{
		private static ResourceManager commandResources = new ResourceManager("System.MacOS.AppKit.Commands", typeof(Localization).Assembly);
		private static ResourceManager errorResources = new ResourceManager("System.MacOS.ErrorMessages", typeof(Localization).Assembly);
		private static string applicationName = Bundle.MainBundle.Name ?? Process.GetCurrentProcess().MainModule.ModuleName;
		
		public static string GetMenuTitle(string name)
		{
			return commandResources.GetString(name + "MenuTitle") ?? name;
		}
		
		public static string GetCommandTitle(string name)
		{
			return commandResources.GetString(name + "CommandTitle") ?? name;
		}
		
		public static string GetCommandDescription(string name)
		{
			return commandResources.GetString(name + "CommandDescription") ?? name;
		}
		
		public static Command GetCommand(string name)
		{
			return GetCommand(name, null);
		}
		
		internal static Command GetCommand(string name, string nativeName)
		{
			var command = nativeName != null ? Command.Register(name, nativeName) : Command.Register(name);
			
			command.Title = GetCommandTitle(name);
			command.Description = GetCommandDescription(name);
			
			return command;
		}
		
		public static Command GetCommand(string name, ShortcutKey shortcutKey)
		{
			return GetCommand(name, null, shortcutKey);
		}
		
		internal static Command GetCommand(string name, string nativeName, ShortcutKey shortcutKey)
		{
			var command = nativeName != null ? Command.Register(name, nativeName) : Command.Register(name);
			
			command.Title = GetCommandTitle(name);
			command.Description = GetCommandDescription(name);
			command.ShortcutKey = shortcutKey;
			
			return command;
		}
		
		public static Command GetApplicationCommand(string name)
		{
			return GetApplicationCommand(name, null);
		}
		
		internal static Command GetApplicationCommand(string name, string nativeName)
		{
			var command = nativeName != null ? Command.Register(name, nativeName) : Command.Register(name);
			var titleFormat = GetCommandTitle(name);
			var descriptionFormat = GetCommandDescription(name);
			
			command.Title = titleFormat != null && titleFormat.Length >= 3 ?
				string.Format(CultureInfo.CurrentCulture, titleFormat, applicationName) :
				null;
			command.Description = descriptionFormat != null && descriptionFormat.Length >= 3 ?
				string.Format(CultureInfo.CurrentCulture, descriptionFormat, applicationName) :
				null;
			
			return command;
		}
		
		public static Command GetApplicationCommand(string name, ShortcutKey shortcutKey)
		{
			return GetApplicationCommand(name, null, shortcutKey);
		}
		
		internal static Command GetApplicationCommand(string name, string nativeName, ShortcutKey shortcutKey)
		{
			var command = nativeName != null ? Command.Register(name, nativeName) : Command.Register(name);
			var titleFormat = GetCommandTitle(name);
			var descriptionFormat = GetCommandDescription(name);
			
			command.Title = titleFormat != null && titleFormat.Length >= 3 ?
				string.Format(CultureInfo.CurrentCulture, titleFormat, applicationName) :
				null;
			command.Description = descriptionFormat != null && descriptionFormat.Length >= 3 ?
				string.Format(CultureInfo.CurrentCulture, descriptionFormat, applicationName) :
				null;
			command.ShortcutKey = shortcutKey;
			
			return command;
		}
		
		public static string GetExceptionText(string name)
		{
			return errorResources.GetString(name);
		}
		
		public static string GetExceptionText(string name, params object[] args)
		{
			return string.Format(CultureInfo.CurrentCulture, errorResources.GetString(name), args);
		}
	}
}
