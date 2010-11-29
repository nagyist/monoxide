using System;
using System.ComponentModel;

namespace System.MacOS.AppKit
{
	public class Segment : ICloneable, INotifyPropertyChanged
	{
		private SegmentedCell cell;
		private string label;
		private string toolTip;
		private Image image;
		private Menu menu;
		private object tag;
		private ImageScaleMode imageScaleMode;
		private bool selected;
		private bool enabled = true;
		private double width;

		public event EventHandler Click;
		public event PropertyChangedEventHandler PropertyChanged;
		
		// TODO: Implement the command model here
		public Command Command { get; set; }
		
		public CommandTarget CommandTarget { get; set; }
		
		public string Label
		{
			get { return label; }
			set
			{
				if (value != label)
				{
					label = value;
					NotifyPropertyChanged("Label");
				}
			}
		}
		
		public string ToolTip
		{
			get { return toolTip; }
			set
			{
				if (value != toolTip)
				{
					toolTip = value;
					NotifyPropertyChanged("ToolTip");
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
		
		public Menu Menu
		{
			get { return menu; }
			set
			{
				if (value != menu)
				{
					if (value != null) value.Owner = this;
					if (menu != null) menu.Owner = null;
					menu = value;
					NotifyPropertyChanged("Menu");
				}
			}
		}
		
		public object Tag
		{
			get { return tag; }
			set
			{
				if (value != tag)
				{
					tag = value;
					NotifyPropertyChanged("Tag");
				}
			}
		}
		
		public ImageScaleMode ImageScaleMode
		{
			get { return imageScaleMode; }
			set
			{
				if (value != imageScaleMode)
				{
					imageScaleMode = value;
					NotifyPropertyChanged("ImageScaleMode");
				}
			}
		}
		
		public bool Selected
		{
			get { return selected; }
			set
			{
				if (value != selected)
				{
					selected = value;
					if (Cell != null) Cell.HandleSegmentSelectedChanged(this);
					NotifyPropertyChanged("Selected");
				}
			}
		}
		
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (value != enabled)
				{
					enabled = value;
					NotifyPropertyChanged("Enabled");
				}
			}
		}
		
		public double Width
		{
			get { return width; }
			set
			{
				if (value != width)
				{
					width = value;
					NotifyPropertyChanged("Width");
				}
			}
		}
		
		/// <summary>Gets or sets the cell containing this segment.</summary>
		/// <remarks>This property is internal and thus, no notification is sent when its value is changed.</remarks>
		internal SegmentedCell Cell
		{
			get { return cell; }
			set
			{
				if (value != null && cell != null)
					throw new InvalidOperationException(Localization.GetExceptionText("SegmentOwned"));
				if (value == null && cell == null)
					throw new InvalidOperationException(Localization.GetExceptionText("InternalError"));
				cell = value;
			}
		}

		/// <summary>Sets the selected property internally.</summary>
		/// <param name="value">A <see cref="System.Boolean"/> indicating the new selection status.</param>
		/// <remarks>
		/// 	Setting the selection status with this property will not raise internal selection change events.
		/// 	This should only be used by SegmentedCell for correct interop with Segment.
		/// </remarks>
		internal void SetSelectedInternal(bool value)
		{
			enabled = value;
			NotifyPropertyChanged("Enabled");
		}
		
		private void NotifyPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
			
			if (Command != null)
			{
				var target = CommandTarget ?? Application.Current.GetTargetForCommand(Command);
				
				if (target != null)
					target.Execute(Command, this);
			}
		}
		
		public object Clone()
		{
			var clone = MemberwiseClone() as Segment;
			
			clone.Cell = null;
			
			return clone;
		}
	}
}
