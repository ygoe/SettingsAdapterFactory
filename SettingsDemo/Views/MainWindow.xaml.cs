using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unclassified.UI;
using Unclassified.Util;

namespace SettingsDemo.Views
{
	/// <summary>
	/// The main window.
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Private data

		// TODO: Put private fields here

		#endregion Private data

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			// EXPLANATION:
			// Window state persistence is only used when it's enabled in the settings. This is a
			// somewhat clunky way to do it, because it doesn't apply at runtime, but it's only an
			// example. Most of the time, you won't offer an option for this behaviour and either do
			// it or not.

			if (App.Settings.View.RememberLocation)
			{
				// EXPLANATION:
				// The SettingsHelper class contains some methods that appear in every application
				// again. This one restores a window's placement from the IWindowStateSettings
				// provided from somewhere in the settings. There can be different such properties
				// for different windows. Then, it adds all the relevant event handlers to keep the
				// settings up-to-date when the window is moved or resized. It also correctly
				// handles minimising and maximising the window and won't start your application
				// with an invisible window next time.

				SettingsHelper.BindWindowState(this, App.Settings.MainWindowState);
			}
		}

		#endregion Constructors

		#region Window event handlers

		// TODO: Put window event handler methods here

		#endregion Window event handlers

		#region Control event handlers

		private void FileButton_Click(object sender, RoutedEventArgs e)
		{
#if DEBUG
			new Test().RunFile();
			MessageBox.Show("Test passed.");
#else
			MessageBox.Show("Test can only be run in Debug build.");
#endif
		}

		private void RegistryButton_Click(object sender, RoutedEventArgs e)
		{
#if DEBUG
			new Test().RunRegistry();
			MessageBox.Show("Test passed.");
#else
			MessageBox.Show("Test can only be run in Debug build.");
#endif
		}

		#endregion Control event handlers
	}
}
