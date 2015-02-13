using System;
using System.Linq;
using Unclassified.Util;

namespace SettingsDemo
{
	/// <summary>
	/// The application start class.
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// Application entry point.
		/// </summary>
		/// <remarks>
		/// The App class is set to the build action "ApplicationDefinition" which also generates a
		/// Main method suitable as application entry point. Therefore, this class must be selected
		/// as start object in the project configuration. If the App class was set up otherwise,
		/// Visual Studio would not find the application-wide resources in the App.xaml file and
		/// mark all such StaticResource occurences in XAML files as an error.
		/// </remarks>
		[STAThread]
		public static void Main()
		{
			// EXPLANATION:
			// The settings system is initialised early at application startup so that settings are
			// available from the start. Settings usually have no dependencies so they can be
			// initialised early, whereas other parts of application initialisation may require
			// settings to be available.
			App.InitializeSettings();

			// Make sure the settings are properly saved in the end
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

			App app = new App();
			app.InitializeComponent();
			app.Run();
		}

		/// <summary>
		/// Called when the current process exits.
		/// </summary>
		/// <param name="sender">Unused.</param>
		/// <param name="args">Unused.</param>
		/// <remarks>
		/// The processing time in this event is limited. All handlers of this event together must
		/// not take more than ca. 3 seconds. The processing will then be terminated.
		/// </remarks>
		private static void CurrentDomain_ProcessExit(object sender, EventArgs args)
		{
			if (App.Settings != null)
			{
				// EXPLANATION:
				// The SettingsStore instance implements IDisposable to make sure no deferred saving
				// is outstanding when the object is really deleted. Just call the Dispose method
				// before giving up the instance so that all changed settings are safely stored.
				App.Settings.SettingsStore.Dispose();
			}
		}
	}
}
