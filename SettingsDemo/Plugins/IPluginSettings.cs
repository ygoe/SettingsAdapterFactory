using System;
using System.ComponentModel;
using Unclassified.Util;

namespace SettingsDemo.Plugins
{
	// EXPLANATION:
	// This is the interface to define what settings to keep in the example plugin. See IAppSettings
	// for a complete description.
	//
	// Plugins can define their own settings. They can share the application's main settings store
	// and store their properties under a certain prefix. This prefix may be defined by the plugin
	// or assigned by the host application. It must be passed as the second parameter to the
	// SettingsAdapterFactory.New method.

	/// <summary>
	/// Defines the settings of the demo plugin.
	/// </summary>
	public interface IDemoPluginSettings : ISettings
	{
		/// <summary>
		/// Gets or sets the last started version of the plugin.
		/// </summary>
		string LastStartedPluginVersion { get; set; }

		/// <summary>
		/// Gets or sets the custom value for the plugin. This is an example property only.
		/// </summary>
		int CustomValue { get; set; }
	}
}
