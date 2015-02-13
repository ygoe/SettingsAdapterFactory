using System;
using System.Linq;
using System.Windows;
using SettingsDemo.Plugins;
using SettingsDemo.ViewModels;
using SettingsDemo.Views;
using Unclassified.Util;

namespace SettingsDemo
{
	/// <inheritdoc/>
	public partial class App : Application
	{
		#region Startup

		/// <inheritdoc/>
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			LoadPlugins();

			// Create main window and view model
			var view = new MainWindow();
			var viewModel = new MainViewModel();
			view.DataContext = viewModel;

			// Show the main window
			view.Show();
		}

		// Load plugins – dummy code
		private void LoadPlugins()
		{
			// EXPLANATION:
			// Plugins can define their own settings. They can share the application's main settings
			// store and store their properties under a certain prefix. This prefix may be defined
			// by the plugin or assigned by the host application. It must be passed as the second
			// parameter to the SettingsAdapterFactory.New method.

			// Assign the plugin settings prefix
			string pluginSettingsPrefix = "DemoPlugin";

			// Create an instance of the plugin class. It will manage its own settings independently.
			var plugin = new DemoPlugin(App.Settings.SettingsStore, pluginSettingsPrefix);
		}

		#endregion Startup

		#region Settings

		// EXPLANATION:
		// This is where the application settings instance is stored. The generated object will be
		// put here and every access to the settings from the application will access this property.
		// It's static so that it can be accessed from everywhere without the need for passing on
		// instances to anything.

		/// <summary>
		/// Provides properties to access the application settings.
		/// </summary>
		public static IAppSettings Settings { get; private set; }

		/// <summary>
		/// Initialises the application settings.
		/// </summary>
		public static void InitializeSettings()
		{
			if (Settings != null) return;   // Already done

			// EXPLANATION:
			// Here is the main work for the SettingsAdapterFactory class. Its New method is called
			// to dynamically create an implementation of the settings interface with all the
			// defined properties and sub-interfaces. The resulting object has a certain runtime
			// type which is not known at compile time and thus not accessible from the code. The
			// concrete type is not relevant, though, because it simply implements the specified
			// interface which can be used by the code.
			//
			// The SettingsAdapterFactory uses a SettingsStore object as its backend. This is where
			// all settings data is actually read from and written to. You can pull out this line
			// and create any ISettingsStore object you like. The FileSettingsStore takes a path to
			// the XML settings file on disk. It also implements deferred saving of the file.
			// Another implementation, RegistrySettingsStore, connects with the Windows registry
			// instead of a file and does not defer write operations.

			Settings = SettingsAdapterFactory.New<IAppSettings>(
				new FileSettingsStore(
					SettingsHelper.GetAppDataPath(@"Unclassified\SettingsDemo", "SettingsDemo.conf")));

			// EXPLANATION:
			// The settings schema can change over time. Since the file format directly represents
			// the interface members, when the code is refactored, the settings values are searched
			// in a different place. In order to convert old names in a new application, we need to
			// track which version last wrote the settings file and detect when a newer schema is
			// used in the current program. The following code checks for older versions of settings
			// and renames old settings keys directly in the settings store (since we don't have
			// interface members to access them right now). This allows keeping the value of
			// previous versions, and also deletes the old names so that they won't gather in the
			// file over the years. Beware that these are mostly unrelated examples to demonstrate
			// the possible conversions.

			// Update settings format from old version
			if (string.IsNullOrEmpty(Settings.LastStartedAppVersion))
			{
				// Changes before the last started version was tracked
				Settings.SettingsStore.Rename("play-sounds", "sounds.enabled");
			}
			if (new Version(Settings.LastStartedAppVersion).CompareTo(new Version("0.3")) < 0)
			{
				// Changes made in version 0.3
				Settings.SettingsStore.Rename("window.left", "MainWindowState.Left");
				Settings.SettingsStore.Rename("window.top", "MainWindowState.Top");
				Settings.SettingsStore.Rename("window.width", "MainWindowState.Width");
				Settings.SettingsStore.Rename("window.height", "MainWindowState.Height");
				Settings.View.MainWindowState.IsMaximized = Settings.SettingsStore.GetInt("window.state") == 2;
				Settings.SettingsStore.Remove("window.state");
			}

			// Remember the version of the application.
			// If we need to react on settings changes from previous application versions, here is
			// the place to check the version currently in the settings, before it's overwritten.
			Settings.LastStartedAppVersion = "1.0";
		}

		#endregion Settings
	}
}
