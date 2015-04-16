// Copyright (c) 2015, Yves Goergen, http://unclassified.software/source/settingsadapterfactory
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Unclassified.Util
{
	/// <summary>
	/// Provides methods for specialized settings situations.
	/// </summary>
	public static partial class SettingsHelper
	{
		#region Settings file path methods

		/// <summary>
		/// Returns a settings file path in the user's AppData directory.
		/// </summary>
		/// <param name="directory">The directory in the AppData directory. May include backslashes or slashes for subdirectories.</param>
		/// <param name="fileName">The settings file name.</param>
		/// <returns></returns>
		public static string GetAppDataPath(string directory, string fileName)
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				directory
					.Replace('\\', Path.DirectorySeparatorChar)
					.Replace('/', Path.DirectorySeparatorChar),
				fileName);
		}

		#endregion Settings file path methods

		#region ISettingsStore extension methods

		/// <summary>
		/// Gets the display name of the storage location for an <see cref="ISettingsStore"/>
		/// instance. This includes optional flags.
		/// </summary>
		/// <param name="settingsStore">The settings store instance of which to return the location.</param>
		/// <returns></returns>
		public static string GetLocationDisplayName(this ISettingsStore settingsStore)
		{
			if (settingsStore == null) throw new ArgumentNullException("settingsStore");

			FileSettingsStore fileStore = settingsStore as FileSettingsStore;
			if (fileStore != null)
			{
				return fileStore.FileName +
					(fileStore.IsReadOnly ? " [RO]" : "") +
					(fileStore.IsEncrypted ? " [ENC]" : "");
			}
			RegistrySettingsStore registryStore = settingsStore as RegistrySettingsStore;
			if (registryStore != null)
			{
				return (registryStore.IsGlobal ? "HKEY_LOCAL_MACHINE" : "HKEY_CURRENT_USER") +
					"\\" + registryStore.BaseKey +
					(registryStore.IsReadOnly ? " [RO]" : "");
			}
			return settingsStore.ToString();
		}

		/// <summary>
		/// Removes multiple setting keys from the settings store, matching a regular expression
		/// pattern.
		/// </summary>
		/// <param name="settingsStore">The settings store instance in which to remove keys.</param>
		/// <param name="keyRegex">The Regex pattern of the setting keys to remove.</param>
		/// <returns>true if any key was removed, false if none existed or matched.</returns>
		public static bool RemovePattern(this ISettingsStore settingsStore, string keyRegex)
		{
			if (settingsStore == null) throw new ArgumentNullException("settingsStore");

			bool anyRemoved = false;
			foreach (string key in settingsStore.GetKeys())
			{
				if (Regex.IsMatch(key, keyRegex))
				{
					anyRemoved |= settingsStore.Remove(key);
				}
			}
			return anyRemoved;
		}

		#endregion ISettingsStore extension methods
	}

	#region Window state settings interface

	/// <summary>
	/// Defines a settings structure that represents a window location, size and state.
	/// </summary>
	public interface IWindowStateSettings : ISettings
	{
		/// <summary>Gets or sets the left edge of the window.</summary>
		[Obfuscation(Exclude = true)]
		[DefaultValue(int.MinValue)]
		int Left { get; set; }

		/// <summary>Gets or sets the top edge of the window.</summary>
		[Obfuscation(Exclude = true)]
		[DefaultValue(int.MinValue)]
		int Top { get; set; }

		/// <summary>Gets or sets the width of the window.</summary>
		[Obfuscation(Exclude = true)]
		[DefaultValue(int.MinValue)]
		int Width { get; set; }

		/// <summary>Gets or sets the height of the window.</summary>
		[Obfuscation(Exclude = true)]
		[DefaultValue(int.MinValue)]
		int Height { get; set; }

		/// <summary>Gets or sets a value indicating whether the window is maximized.</summary>
		[Obfuscation(Exclude = true)]
		[DefaultValue(false)]
		bool IsMaximized { get; set; }
	}

	#endregion Window state settings interface
}
