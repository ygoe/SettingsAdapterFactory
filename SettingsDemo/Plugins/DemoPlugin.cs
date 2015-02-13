using System;
using System.Linq;
using Unclassified.Util;

namespace SettingsDemo.Plugins
{
	/// <summary>
	/// Implements the demo plugin that shows how to use settings for loosely coupled plugins.
	/// </summary>
	public class DemoPlugin
	{
		/// <summary>
		/// Provides properties to access the plugin settings.
		/// </summary>
		public IDemoPluginSettings Settings { get; private set; }

		/// <summary>
		/// Initialises a new instance of the DemoPlugin class that implements the demo plugin.
		/// </summary>
		/// <param name="settingsStore">The application's settings store to share by this plugin.</param>
		/// <param name="pluginSettingsPrefix">The settings key prefix for this plugin in the application's settings store.</param>
		public DemoPlugin(ISettingsStore settingsStore, string pluginSettingsPrefix)
		{
			// EXPLANATION:
			// The prefix is here assigned by the host application, but it could also be defined by
			// the plugin itself, provided there are no collisions between different plugins or
			// multiple instances of them, or at least the properties in the same prefix.

			// Create an implementation of the plugin's settings interface
			Settings = SettingsAdapterFactory.New<IDemoPluginSettings>(settingsStore, pluginSettingsPrefix);

			// Set the demo value
			Settings.CustomValue = 5;
		}
	}
}
