using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Unclassified.Util;

namespace SettingsDemo
{
	public class Test
	{
		private Action<PropertyChangedEventArgs> changeAction;

		public void RunFile()
		{
			File.Delete("settings.xml");

			ISettingsStore settingsStore = new FileSettingsStore("settings.xml");
			IAppSettings settings = SettingsAdapterFactory.New<IAppSettings>(settingsStore);

			AccessSettings(settingsStore, settings);

			settings.SettingsStore.Dispose();
		}

		public void RunRegistry()
		{
			Registry.CurrentUser.OpenSubKey(@"Software\Unclassified", true).DeleteSubKeyTree("SettingsDemo", false);

			ISettingsStore settingsStore = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemo");
			IAppSettings settings = SettingsAdapterFactory.New<IAppSettings>(settingsStore);

			AccessSettings(settingsStore, settings);

			settings.SettingsStore.Dispose();
		}

		private void AccessSettings(ISettingsStore settingsStore, IAppSettings settings)
		{
			Debug.Assert(settings.View.IndentSize == 15);

			settingsStore.Set("View.IndentSize", 50);
			Debug.Assert(settings.View.IndentSize == 50);

			settingsStore.Set("Culture", "fr-FR");
			Debug.Assert(settings.Culture == "fr-FR");

			Debug.Assert(settings.View.MonospaceFont == false);

			settings.View.IndentSize = 32;
			Debug.Assert(settings.View.IndentSize == 32);

			settings.PropertyChanged += settings_PropertyChanged;

			changeAction = e => Debug.Assert(false, "No change notification should occur.");
			settings.View.IndentSize = 32;

			changeAction = e => Debug.Assert(e.PropertyName == "Culture" && settings.Culture == "en-GB");
			settings.Culture = "en-GB";

			settings.PropertyChanged -= settings_PropertyChanged;

			settings.TestNumbers = new[] { 1, 2, 3, 5, 8 };
			Debug.Assert(settings.TestNumbers.Select(_ => _.ToString()).Aggregate((a, b) => a + ", " + b) == "1, 2, 3, 5, 8");

			settings.TestNumbers = settings.TestNumbers.Concat(new[] { 13, 21, 34 }).ToArray();
			Debug.Assert(settings.TestNumbers.Select(_ => _.ToString()).Aggregate((a, b) => a + ", " + b) == "1, 2, 3, 5, 8, 13, 21, 34");
		}

		private void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			changeAction(e);
		}
	}
}