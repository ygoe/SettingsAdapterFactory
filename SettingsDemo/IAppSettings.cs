using System;
using System.ComponentModel;
using Unclassified.Util;

namespace SettingsDemo
{
	// EXPLANATION:
	// This is the main interface to define what settings to keep in the application. It must
	// inherit from the ISettings interface in order to provide the SettingsStore property. This can
	// be the only way to access the back-end settings store later on, and is used for old versions
	// upgrading, and closing the file on app shutdown.
	//
	// Each setting is represented by a property. Most properties should have a public getter and
	// setter so that the application can read and write its value. Groups of settings can be
	// defined with a separate interface which is referenced with a property with only a getter.
	// (You wouldn't want to replace a subtree of the settings adapter implementation with something
	// else.) These sub-interfaces can also be reused in multiple places. A pre-defined interface
	// is IWindowStateSettings, which is also supported by methods in the SettingsHelper class.
	//
	// Properties can be documented as usual. They can also have attributes to further specify their
	// behaviour. The DefaultValueAttribute specifies what value a setting should have if it doesn't
	// appear in the settings store. This is not supported for array types.
	//
	// Technically, the SettingsAdapterFactory will implement the entire interface you define. What
	// else can it do? The .NET CLR wouldn't let pass anything else. Should you forget a getter or
	// setter, it won't be available. Should you define a method, it will do nothing and return a
	// default value. If you use property types that are not supported, because they cannot be
	// mapped to the settings store, a runtime exception will be raised directly when starting the
	// application.

	/// <summary>
	/// Defines the application settings.
	/// </summary>
	public interface IAppSettings : ISettings
	{
		/// <summary>
		/// Provides settings for the main window state.
		/// </summary>
		IWindowStateSettings MainWindowState { get; }

		/// <summary>
		/// Gets or sets the last started version of the application.
		/// </summary>
		string LastStartedAppVersion { get; set; }

		/// <summary>
		/// Gets or sets the UI culture code.
		/// </summary>
		[DefaultValue("de-DE")]
		string Culture { get; set; }

		/// <summary>
		/// Gets or sets an array containing the recently loaded files. The most recently loaded
		/// entry is first.
		/// </summary>
		string[] RecentlyLoadedFiles { get; set; }

		/// <summary>
		/// Gets or sets some numbers.
		/// </summary>
		int[] TestNumbers { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a sound is played on new items.
		/// </summary>
		bool IsSoundEnabled { get; set; }

		/// <summary>
		/// Gets the view-related settings.
		/// </summary>
		IViewSettings View { get; }
	}

	/// <summary>
	/// Defines view-related application settings.
	/// </summary>
	public interface IViewSettings : ISettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether a monospace font is used.
		/// </summary>
		bool MonospaceFont { get; set; }

		/// <summary>
		/// Gets or sets the time zone to use for displaying times.
		/// </summary>
		[DefaultValue(TimeType.Remote)]
		TimeType TimeType { get; set; }

		/// <summary>
		/// Gets or sets the level indent size.
		/// </summary>
		[DefaultValue(15)]
		int IndentSize { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the main window location is restored.
		/// </summary>
		bool RememberLocation { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the main window is always on top.
		/// </summary>
		bool IsWindowOnTop { get; set; }

		/// <summary>
		/// Gets the state settings of the main window.
		/// </summary>
		IWindowStateSettings MainWindowState { get; }
	}

	// EXPLANATION:
	// You can use enum types just like regular integer number types. They will be stored to the
	// backend as their numeric value. So you can rename them but you cannot reorder them or change
	// their assigned values without causing a breaking change. (See App.InitializeSettings on how
	// to update settings from previous versions.)

	/// <summary>
	/// Defines time zone types.
	/// </summary>
	public enum TimeType
	{
		/// <summary>
		/// UTC time.
		/// </summary>
		[Description("UTC")]
		Utc,

		/// <summary>
		/// Local time as on the local system.
		/// </summary>
		[Description("Local system")]
		Local,
		
		/// <summary>
		/// Local time as on the remote system.
		/// </summary>
		[Description("Remote system")]
		Remote
	}
}
