using System;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	// TODO: Add localized exception messages.
	public sealed class ToolbarTemplate
	{
		#region ToolbarItemCollection Class
				
		public sealed class ToolbarItemCollection : IList<ToolbarItem>
		{
			ToolbarTemplate template;
			
			internal ToolbarItemCollection(ToolbarTemplate template) { this.template = template; }
			
			public ToolbarItem this[int index]
			{
				get
				{
					lock (template.syncRoot)
						return template.itemList[index];
				}
				set
				{
					if (value == null)
						throw new ArgumentNullException("value");
					
					lock (template.syncRoot)
					{
						var oldItem = template.itemList[index];
						
						if (value == oldItem) return;
						
						if (!value.IsTemplate)
							throw new InvalidOperationException(Localization.GetExceptionText("NotTemplateToolbarItem"));
						
						if (value.Name == oldItem.Name)
							template.itemDictionary[oldItem.Name] = value;
						else if (template.itemDictionary.ContainsKey(value.Name))
							throw new ArgumentException(Localization.GetExceptionText("ToolbarItemNameConflict"));
						else
							template.itemDictionary.Add(value.Name, value);
						
						try { value.AddedToTemplate(); }
						catch (InvalidOperationException ex) // Whenever this happens, someone has done something terribly wrong !
						{
							template.itemDictionary[oldItem.Name] = oldItem;
							throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemConsistency"), ex);
						}
						template.itemList[index] = value;
						
						template.defaultItemList.RemoveAll(i => i == oldItem);
					}
				}
			}
			
			public ToolbarItem this[string name]
			{
				get
				{
					lock (template.syncRoot)
						return template.itemDictionary[name];
				}
			}
			
			public int Count
			{
				get
				{
					lock (template.syncRoot)
						return template.itemList.Count;
				}
			}
			
			public bool IsReadOnly { get { return false; } }
			
			public bool Contains(string name)
			{
				lock (template.syncRoot)
					return template.itemDictionary.ContainsKey(name);
			}
			
			public bool Contains(ToolbarItem item)
			{
				if (item == null) return false;
				
				lock (template.syncRoot)
				{
					ToolbarItem containedItem;
					
					return template.itemDictionary.TryGetValue(item.Name, out containedItem) && item == containedItem;
				}
			}
			
			public int IndexOf(ToolbarItem item)
			{
				lock (template.syncRoot)
					return template.itemList.IndexOf(item);
			}
			
			public void Add(ToolbarItem item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				lock (template.syncRoot)
				{
					if (!item.IsTemplate)
						throw new InvalidOperationException(Localization.GetExceptionText("NotTemplateToolbarItem"));
					template.itemDictionary.Add(item.Name, item);
					template.itemList.Add(item);
					try { item.AddedToTemplate(); }
					catch (InvalidOperationException ex) // Whenever this happens, someone has done something terribly wrong !
					{
						template.itemDictionary[item.Name] = item;
						throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemConsistency"), ex);
					}
				}
			}
			
			public void AddRange(ToolbarItem[] items)
			{
				lock (template.syncRoot)
					foreach (var item in items)
						Add(item);
			}
			
			public void Insert(int index, ToolbarItem item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				lock (template.syncRoot)
				{
					if (!item.IsTemplate)
						throw new InvalidOperationException(Localization.GetExceptionText("NotTemplateToolbarItem"));
					template.itemDictionary.Add(item.Name, item);
					try { template.itemList.Insert(index, item); }
					catch
					{
						template.itemDictionary.Remove(item.Name);
						throw;
					}
					try { item.AddedToTemplate(); }
					catch (InvalidOperationException ex) // Whenever this happens, someone has done something terribly wrong !
					{
						template.itemDictionary[item.Name] = item;
						throw new InvalidOperationException(Localization.GetExceptionText("ToolbarItemConsistency"), ex);
					}
				}
			}
			
			public void RemoveAt(int index)
			{
				lock (template.syncRoot)
				{
					var item = template.itemList[index];
					
					template.itemList.RemoveAt(index);
					template.itemDictionary.Remove(item.Name);
					template.defaultItemList.RemoveAll(i => i == item);
					
					item.RemovedFromTemplate();
				}
			}
			
			public bool Remove(ToolbarItem item)
			{
				lock (template.syncRoot)
				{
					ToolbarItem dictionaryItem;
					
					if (item != null
						&& template.itemDictionary.TryGetValue(item.Name, out dictionaryItem)
						&& item == dictionaryItem
					    && template.itemList.Remove(item))
					{
						template.itemDictionary.Remove(item.Name);
						template.defaultItemList.RemoveAll(i => i == item);
						item.RemovedFromTemplate();
						
						return true;
					}
				}
				return false;
			}
			
			public void Clear()
			{
				lock (template.syncRoot)
				{
					// It is safe to call this before true removal, since the implementation is internal and well known
					foreach (var item in template.itemList)
						item.RemovedFromTemplate();
					template.itemList.Clear();
					template.itemDictionary.Clear();
				}
			}
			
			public void CopyTo(ToolbarItem[] array, int arrayIndex)
			{
				lock (template.syncRoot)
					template.itemList.CopyTo(array, arrayIndex);
			}
			
			public ToolbarItem[] ToArray()
			{
				lock (template.syncRoot)
					return template.itemList.ToArray();
			}
			
			public IEnumerator<ToolbarItem> GetEnumerator()
			{
				ToolbarItem[] items;
				
				lock (template.syncRoot)
					items = template.itemList.ToArray();
				
				foreach (var item in items)
					yield return item;
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}
		
		#endregion
		
		#region DefaultToolbarItemCollection Class
		
		public class DefaultToolbarItemCollection : IList<ToolbarItem>
		{
			ToolbarTemplate template;
			
			internal DefaultToolbarItemCollection(ToolbarTemplate template) { this.template = template; }
			
			// TODO: implement item unicity
			public ToolbarItem this[int index]
			{
				get
				{
					lock (template.syncRoot)
						return template.defaultItemList[index];
				}
				set
				{
					if (value == null)
						throw new ArgumentNullException("value");
					
					lock (template.syncRoot)
					{
						var oldItem = template.defaultItemList[index];
						
						if (value == oldItem) return;
						
						if (template.itemCollection.Contains(value))
							template.defaultItemList[index] = value;
					}
				}
			}
			
			public int Count
			{
				get
				{
					lock (template.syncRoot)
						return template.defaultItemList.Count;
				}
			}
			
			public bool IsReadOnly { get { return false; } }
			
			public bool Contains(ToolbarItem item)
			{
				lock (template.syncRoot)
					return template.defaultItemList.Contains(item);
			}
			
			public int IndexOf(ToolbarItem item)
			{
				lock (template.syncRoot)
					return template.defaultItemList.IndexOf(item);
			}
			
			// TODO: implement item unicity
			public void Add(ToolbarItem item)
			{
				lock (template.syncRoot)
					if (template.itemCollection.Contains(item))
						template.defaultItemList.Add(item);
			}
			
			public void AddRange(ToolbarItem[] items)
			{
				lock (template.syncRoot)
					foreach (var item in items)
						Add(item);
			}
			
			// TODO: implement item unicity
			public void Insert(int index, ToolbarItem item)
			{
				lock (template.syncRoot)
					if (template.itemCollection.Contains(item))
						template.defaultItemList.Insert(index, item);
			}
			
			public bool Remove(ToolbarItem item)
			{
				lock (template.syncRoot)
					return template.defaultItemList.Remove(item);
			}
						
			public void RemoveAt(int index)
			{
				lock (template.syncRoot)
					template.defaultItemList.RemoveAt(index);
			}
			
			public void Clear()
			{
				lock (template.syncRoot)
					template.defaultItemList.Clear();
			}
			
			public void CopyTo(ToolbarItem[] array, int arrayIndex)
			{
				lock (template.syncRoot)
					template.defaultItemList.CopyTo(array, arrayIndex);
			}
			
			public ToolbarItem[] ToArray()
			{
				lock (template.syncRoot)
					return template.defaultItemList.ToArray();
			}
			
			public IEnumerator<ToolbarItem> GetEnumerator()
			{
				ToolbarItem[] items;
				
				lock (template.syncRoot)
					items = template.defaultItemList.ToArray();
				
				foreach (var item in items)
					yield return item;
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}
		
		#endregion
		
		private static readonly Dictionary<string, ToolbarTemplate> templateDictionary = new Dictionary<string, ToolbarTemplate>(2, StringComparer.InvariantCultureIgnoreCase);
		
		public static ToolbarTemplate Get(string name)
		{
			ToolbarTemplate template;
			
			lock (templateDictionary)
				templateDictionary.TryGetValue(name, out template);
			
			return template;
		}
		
		public static bool IsDefined(string name)
		{
			lock (templateDictionary)
				return templateDictionary.ContainsKey(name);
		}
		
		public static bool TryDefine(string name, ToolbarItem[] allowedItems, ToolbarItem[] defaultItems)
		{
			lock (templateDictionary)
				if (!IsDefined(name))
				{
					var template = new ToolbarTemplate(name);
					
					template.Items.AddRange(allowedItems);
					template.DefaultItems.AddRange(defaultItems);
					
					templateDictionary.Add(name, template);
					
					return true;
				}
				else
					return false;
		}
		
		public static bool IsValidName(string name) { return Casing.IsPascalCased(name); }
		
		private readonly object syncRoot;
		private readonly List<ToolbarItem> itemList;
		private readonly Dictionary<string, ToolbarItem> itemDictionary;
		private readonly List<ToolbarItem> defaultItemList;
		private readonly ToolbarItemCollection itemCollection;
		private readonly	 DefaultToolbarItemCollection defaultItemCollection;
		private readonly string name;
		
		private ToolbarTemplate(string name)
		{
			if (!IsValidName(name))
				throw new NameFormatException(name);
			
			this.syncRoot = new object();
			this.itemList = new List<ToolbarItem>();
			this.itemDictionary = new Dictionary<string, ToolbarItem>(StringComparer.InvariantCultureIgnoreCase);
			this.defaultItemList = new List<ToolbarItem>();
			this.itemCollection = new ToolbarItemCollection(this);
			this.defaultItemCollection = new DefaultToolbarItemCollection(this);
			this.name = name;
		}
		
		public string Name { get { return name; } }
		public ToolbarItemCollection Items { get { return itemCollection; } }
		public DefaultToolbarItemCollection DefaultItems { get { return defaultItemCollection; } }
		
		public bool TryGetItem(string name, out ToolbarItem item)
		{
			lock (syncRoot)
				return itemDictionary.TryGetValue(name, out item);
		}
	}
}
