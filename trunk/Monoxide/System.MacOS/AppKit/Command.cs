using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.MacOS.AppKit
{
	public sealed class Command : INotifyPropertyChanged
	{
		private static Dictionary<string, Command> commandDictionary = new Dictionary<string, Command>(100, StringComparer.InvariantCultureIgnoreCase);
		
		public static Command Register(string name)
		{
			Command command;
			
			if (name == null)
				throw new ArgumentNullException("name");
			
			lock (commandDictionary)
			{
				if (!commandDictionary.TryGetValue(name, out command))
				{
					command = new Command(name);
					command.Title = name;
					commandDictionary.Add(command.Name, command);
				}
			}
			
			return command;
		}
		
		internal static Command Register(string name, string nativeName)
		{
			Command command;
			
			if (name == null)
				throw new ArgumentNullException("name");
			if (nativeName == null)
				throw new ArgumentNullException("nativeName");
			
			lock (commandDictionary)
			{
				if (!commandDictionary.TryGetValue(name, out command))
				{
					command = new Command(name, nativeName);
					commandDictionary.Add(command.Name, command);
				}
				else // Ensure that the native name is correct
				{
					if (!IsValidName(nativeName))
						throw new NameFormatException(name, Localization.GetExceptionText("CommandNameFormat", name));
					command.SelectorName = GetSelectorName(nativeName);
				}
			}
			
			return command;
		}
		
		public static bool IsRegistered(string name)
		{
			lock (commandDictionary)
				return commandDictionary.ContainsKey(name);
		}
		
		public static bool IsValidName(string name) { return Casing.IsPascalCased(name); }
		
		public static string GetSelectorName(string name)
		{
			int upperCaseCount = 0;
			
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				
				if (c < 'A' || c > 'Z') break;
				
				upperCaseCount++;
			}
			
			if (upperCaseCount > 1) upperCaseCount = 0;
			
			var sb = new StringBuilder(name, name.Length);
			
			for (int i = 0; i < upperCaseCount; i++)
				sb[i] = char.ToLowerInvariant(sb[i]);
			
			sb.Append(':');
			
			return sb.ToString();
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private string title;
		private string description;
		private ShortcutKey shortcutKey;
		private Image image;
		
		private Command(string name)
		{
			if (!IsValidName(name))
				throw new NameFormatException(name, Localization.GetExceptionText("CommandNameFormat", name));
			
			Name = name;
			SelectorName = GetSelectorName(name);
		}
		
		private Command(string name, string nativeName)
		{
			if (!IsValidName(name))
				throw new NameFormatException(name, Localization.GetExceptionText("CommandNameFormat", name));
			if (!IsValidName(nativeName))
				throw new NameFormatException(name, Localization.GetExceptionText("CommandNameFormat", name));
			
			Name = name;
			SelectorName = GetSelectorName(nativeName);
		}
		
		public string Name { get; private set; }
		internal string SelectorName { get; private set; }
		
		public string Title
		{
			get { return title; }
			set
			{
				if (value != title)
				{
					title = value;
					
					NotifyPropertyChanged("Title");
				}
			}
		}
		
		public string Description
		{
			get { return description; }
			set
			{
				if (value != description)
				{
					description = value;
					
					NotifyPropertyChanged("Description");
				}
			}
		}
		
		public ShortcutKey ShortcutKey
		{
			get { return shortcutKey; }
			set
			{
				if (value != shortcutKey)
				{
					shortcutKey = value;
					
					NotifyPropertyChanged("ShortcutKey");
				}
			}
		}
		
		public Image Image
		{
			get { return image; }
			set
			{
				if (value != image)
				{
					image = value;
					
					NotifyPropertyChanged("Image");
				}
			}
		}
		
		private void NotifyPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
