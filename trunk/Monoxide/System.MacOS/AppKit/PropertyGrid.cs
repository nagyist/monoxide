using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Collections.Generic;

namespace System.MacOS.AppKit
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class PropertyGrid : View
	{
		private sealed class InnerView : OutlineViewBase<TextFieldCell>
		{
			private PropertyGrid propertyGrid;
			private TableColumn nameColumn;
			private TableColumn valueColumn;

			public InnerView(PropertyGrid propertyGrid)
			{
				this.propertyGrid = propertyGrid;
				nameColumn = new TableColumn();
				nameColumn.HeaderCell.Value = "Name";
				valueColumn = new TableColumn();
				valueColumn.HeaderCell.Value = "Value";
				Columns.Add(nameColumn);
				Columns.Add(valueColumn);
			}

			protected override int GetItemChildCount(object item)
			{
				return item == null && propertyGrid.properties != null ? propertyGrid.properties.Count : 0;
			}

			protected override object GetItemChild(object item, int index)
			{
				return item == null ? propertyGrid.properties[index] : null;
			}
	
			protected override bool IsItemExpandable(object item)
			{
				return item == null;
			}
	
			protected override string GetItemText(object item, TableColumn<TextFieldCell> column)
			{
				if (column == nameColumn)
					return (item as PropertyDescriptor).DisplayName;
				else
					return null;
			}
		}

		private List<Object> selectedObjects;
		private PropertyDescriptorCollection properties;
		private ScrollView scrollView;
		private InnerView outlineView;
		
		public PropertyGrid()
		{
			selectedObjects = new List<Object>();
			outlineView = new InnerView(this);
			scrollView = new ScrollView();
			scrollView.DocumentView = outlineView;
			Children.Add(scrollView);
		}
		
		public object SelectedObject
		{
			get { return selectedObjects.Count > 0 ? selectedObjects[0] : null; }
			set
			{
				selectedObjects.Clear();
				
				if (value != null)
				{
					selectedObjects.Add(value);
					properties = TypeDescriptor.GetProperties(value);
				}
				else properties = null;
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

				properties = null;
			}
		}
	}
}
