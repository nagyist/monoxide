using System;

namespace System.MacOS.AppKit
{
	public struct ShortcutKey : IEquatable<ShortcutKey>
	{
		public static readonly ShortcutKey None = new ShortcutKey();
		
		string key;
		ModifierKeys modifiers;
		
		public ShortcutKey(string key)
			: this(key, ModifierKeys.Command) { }
		
		public ShortcutKey(string key, ModifierKeys modifiers)
		{
			if (key != null && (key.Length > 2 || (key.Length == 2 && char.IsSurrogatePair(key, 0))))
				throw new ArgumentOutOfRangeException("key");
			
			this.key = key != null && key.Length > 0 ? key : null;
			this.modifiers = modifiers;
		}
		
		public string Key
		{
			get { return key; }
			set
			{
				if (value.Length > 2 || (value.Length == 2 && char.IsSurrogatePair(value, 0)))
					throw new ArgumentOutOfRangeException("value");
				
				key = value != null && value.Length > 0 ? value : null;
			}
		}
		
		public ModifierKeys Modifiers
		{
			get { return modifiers; }
			set { modifiers = value; }
		}
		
		public bool Equals(ShortcutKey other)
		{
			// Remember that key can be null, but never empty, as enforced in the property setter
			return other.key == key && other.modifiers == modifiers;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is ShortcutKey)
				return Equals((ShortcutKey)obj);
			else
				return base.Equals(obj);
		}
		
		public override int GetHashCode()
		{
			return (key ?? string.Empty).GetHashCode() ^ modifiers.GetHashCode();
		}
		
		public static bool operator == (ShortcutKey a, ShortcutKey b)
		{
			return a.Equals(b);
		}
		
		public static bool operator != (ShortcutKey a, ShortcutKey b)
		{
			return !a.Equals(b);
		}
	}
}
