// Copyright (c) 2015, Yves Goergen, http://unclassified.software/source/settingsadapterfactory
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Unclassified.Util
{
	/// <summary>
	/// Provides methods for specialized settings situations.
	/// </summary>
	public static partial class SettingsHelper
	{
		#region Window state handling

		/// <summary>
		/// Binds the window location, size and state to the settings. This method should be called
		/// in the Form constructor after InitializeComponent.
		/// </summary>
		/// <param name="form">The window to update and monitor.</param>
		/// <param name="settings">The settings to use for the window.</param>
		public static void BindWindowState(Form form, IWindowStateSettings settings)
		{
			// Apply the current settings to the window, if available
			if (settings.Left != int.MinValue &&
				settings.Top != int.MinValue &&
				settings.Width != int.MinValue &&
				settings.Height != int.MinValue)
			{
				form.StartPosition = FormStartPosition.Manual;
				form.Left = settings.Left;
				form.Top = settings.Top;
				form.Width = settings.Width;
				form.Height = settings.Height;
				form.WindowState = settings.IsMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
			}

			// Write back any changes to the settings.
			// The event signatures are different, so it's easier to just copy the handler code
			// and let the compiler figure out the inferred types.
			form.LocationChanged += (sender, args) =>
			{
				if (form.WindowState == FormWindowState.Normal)
				{
					settings.Left = (int) form.Bounds.Left;
					settings.Top = (int) form.Bounds.Top;
					settings.Width = (int) form.Bounds.Width;
					settings.Height = (int) form.Bounds.Height;
				}
				settings.IsMaximized = form.WindowState == FormWindowState.Maximized;
			};
			form.SizeChanged += (sender, args) =>
			{
				if (form.WindowState == FormWindowState.Normal)
				{
					settings.Left = (int) form.Bounds.Left;
					settings.Top = (int) form.Bounds.Top;
					settings.Width = (int) form.Bounds.Width;
					settings.Height = (int) form.Bounds.Height;
				}
				settings.IsMaximized = form.WindowState == FormWindowState.Maximized;
			};
		}

		#endregion Window state handling

		#region Color string conversion

		/// <summary>
		/// Converts a <see cref="Color"/> value to a string for a settings string property.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>A hexadecimal color representation.</returns>
		public static string ColorToString(Color color)
		{
			return "#" + (color.A != 255 ? color.A.ToString("x2") : "") +
				color.R.ToString("x2") +
				color.G.ToString("x2") +
				color.B.ToString("x2");
		}

		/// <summary>
		/// Converts a hexadecimal or decimal color string to a <see cref="Color"/> value.
		/// </summary>
		/// <param name="str">The string to convert.</param>
		/// <returns>A <see cref="Color"/> value.</returns>
		public static Color StringToColor(string str)
		{
			if (str.StartsWith("#"))
				str = str.Substring(1);
			if (str.Length != 6 && str.Length != 8)
				throw new FormatException("Invalid 6- or 8-digit hexadecimal color value.");
			long value = long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
			if (str.Length == 6)
				value |= 0xff000000;
			return Color.FromArgb(
				(byte) ((value >> 24) & 0xff),
				(byte) ((value >> 16) & 0xff),
				(byte) ((value >> 8) & 0xff),
				(byte) (value & 0xff));
		}

		#endregion Color string conversion
	}
}
