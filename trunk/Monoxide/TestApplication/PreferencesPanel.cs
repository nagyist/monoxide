using System;
using System.MacOS.AppKit;

namespace TestApplication
{
	public class PreferencesPanel : Panel
	{
		public PreferencesPanel()
		{
			Title = "Preferences";
			Style = WindowStyle.Titled | WindowStyle.Closable;
			CreateToolbarTemplate();
			Toolbar = new Toolbar() { TemplateName = "Preferences", Customizable = false };
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
