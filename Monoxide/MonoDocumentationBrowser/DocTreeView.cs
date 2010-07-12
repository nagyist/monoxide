using System;
using System.MacOS.AppKit;
using Monodoc;

namespace MonoDocumentationBrowser
{
	public class DocTreeView : OutlineViewBase<TextFieldCell>
	{
		Tree tree;
		
		public DocTreeView(Tree tree)
		{
			this.tree = tree;
		}
		
		protected override object GetItemChild(object item, int index)
		{
			var node = item as Node ?? tree;
			
			if (node == null)
				return tree.Nodes[index];
			else
				return node.Nodes[index];
		}
		
		protected override int GetItemChildCount(object item)
		{
			var node = item as Node ?? tree;
			
			if (node == null)
				return tree.Nodes.Count;
			else
				return node.Nodes.Count;
		}
		
		protected override bool IsItemExpandable(object item)
		{
			var node = item as Node ?? tree;
			
			return node.Nodes.Count != 0;
		}
		
		protected override string GetItemText (object item)
		{
			return (item as Node ?? tree).Caption;
		}
	}
}
