using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class PropertyGrid : OutlineViewBase<TextFieldCell>
	{
		private List<Object> selectedObjects = new List<Object>();
		
		public PropertyGrid()
		{
		}
		
		public object SelectedObject
		{
			get { return selectedObjects.Count > 0 ? selectedObjects[0] : null; }
			set
			{
				selectedObjects.Clear();
				
				if (value != null)
					selectedObjects.Add(value);
			}
		}
		
		public object[] SelectedObjects
		{
			get { return selectedObjects.ToArray(); }
			set
			{
				selectedObjects.Clear();
				
				if (value != null)
					selectedObjects.AddRange(value);
			}
		}
	}
}
