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
			File.Delete("settings.xml.bak");

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
			Debug.Assert(settings.GetCultureName() == "français (France)");

			Debug.Assert(settings.View.MonospaceFont == false);
			Debug.Assert(settings.View.FontName() == "Segoe UI");

			settings.View.IndentSize = 32;
			Debug.Assert(settings.View.IndentSize == 32);

			settings.View.MonospaceFont = true;
			Debug.Assert(settings.View.MonospaceFont == true);
			Debug.Assert(settings.View.FontName() == "Consolas");

			settings.PropertyChanged += settings_PropertyChanged;

			changeAction = e => Debug.Assert(false, "No change notification should occur.");
			settings.View.IndentSize = 32;

			changeAction = e => Debug.Assert(e.PropertyName == "View.IndentSize" && settings.View.IndentSize == 33);
			settings.View.IndentSize = 33;

			changeAction = e => Debug.Assert(e.PropertyName == "Culture" && settings.Culture == "en-GB");
			settings.Culture = "en-GB";

			settings.PropertyChanged -= settings_PropertyChanged;

			settings.TestNumbers = new[] { 1, 2, 3, 5, 8 };
			Debug.Assert(settings.TestNumbers.Select(_ => _.ToString()).Aggregate((a, b) => a + ", " + b) == "1, 2, 3, 5, 8");

			settings.TestNumbers = settings.TestNumbers.Concat(new[] { 13, 21, 34 }).ToArray();
			Debug.Assert(settings.TestNumbers.Select(_ => _.ToString()).Aggregate((a, b) => a + ", " + b) == "1, 2, 3, 5, 8, 13, 21, 34");
			Debug.Assert(settings.TestNumbersSum() == 87);

			Debug.Assert(settings.DecimalNumber == 12m);

			settings.DecimalNumber = decimal.MinValue;
			Debug.Assert(settings.DecimalNumber == decimal.MinValue);

			settings.DecimalNumber = decimal.MaxValue;
			Debug.Assert(settings.DecimalNumber == decimal.MaxValue);

			settings.DecimalNumber = 0m;
			Debug.Assert(settings.DecimalNumber == 0m);

			settings.DecimalNumber = 1234.56m;
			Debug.Assert(settings.DecimalNumber == 1234.56m);
		}

		private void settings_PropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			changeAction(args);
		}
	}
}
