using System;
using System.MacOS.AppKit;
using System.ComponentModel;

namespace TestApplication
{
	public class PreferencesPanel : Panel
	{
		class PropertyObject
		{
			[DisplayName("The Property Titi")]
			public string Titi { get; set; }
			[DisplayName("The Toto Property")]
			public string Toto { get; set; }
			[DisplayName("Tutu Property")]
			public string Tutu { get; set; }
			[DisplayName("Tata's Property")]
			public string Tata { get; set; }
			[DisplayName("Property of Tete")]
			public string Tete { get; set; }
		}

		public PreferencesPanel()
		{
			Title = "Preferences";
			Style = WindowStyle.Titled | WindowStyle.Closable;
			CreateToolbarTemplate();
			Toolbar = new Toolbar() { TemplateName = "Preferences", Customizable = false };
			Content.Children.Add(new PropertyGrid() { SelectedObject = new PropertyObject() });
		}
		
		private void CreateToolbarTemplate()
		{
			if (ToolbarTemplate.IsDefined("Preferences")) return;
			
			var items = new []
			{
				new ImageToolbarItem("General") { Label = "General", Image = Image.PreferencesGeneral }
			};
			
			ToolbarTemplate.TryDefine("Preferences", items, items);
		}
	}
}
