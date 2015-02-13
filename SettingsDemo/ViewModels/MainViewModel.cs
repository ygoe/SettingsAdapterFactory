using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unclassified.UI;
using Unclassified.Util;

namespace SettingsDemo.ViewModels
{
	/// <summary>
	/// The ViewModel class for the main window.
	/// </summary>
	internal class MainViewModel : ViewModelBase
	{
		#region Private data

		// TODO: Put private fields here

		#endregion Private data

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the <see cref="MainViewModel"/> class.
		/// </summary>
		public MainViewModel()
		{
			DisplayName = "Settings Demo";
		}

		#endregion Constructors

		#region Command definition

		// TODO: Declare commands here

		/// <inheritdoc/>
		protected override void InitializeCommands()
		{
			// TODO: Create commands here
		}

		#endregion Command definition

		#region Data properties

		// EXPLANATION:
		// The view model class, in its position as DataContext for the window, must provide a
		// reference to the IAppSettings instance because a Binding cannot refer to the static
		// App.Settings property. No other properties for settings members are required because the
		// settings object exposes public properties with change notification and automatic saving
		// itself.

		/// <summary>
		/// Gets the application settings instance.
		/// </summary>
		public IAppSettings Settings
		{
			get { return App.Settings; }
		}

		#endregion Data properties
	}
}
