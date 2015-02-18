// Copyright (c) 2015, Yves Goergen, http://unclassified.software/source/settingsadapterfactory
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;

namespace Unclassified.Util
{
	/// <summary>
	/// Represents a data store that contains all setting keys and values of a specified registry
	/// key.
	/// </summary>
	public class RegistrySettingsStore : ISettingsStore
	{
		#region Private data

		/// <summary>
		/// Internal synchronisation object.
		/// </summary>
		private object syncLock = new object();

		private bool isGlobal;
		private string baseKey;
		private RegistryKey hive;

		/// <summary>
		/// Indicates whether the settings file was opened in read-only mode. This prevents any
		/// write access to the settings and will never save the file back.
		/// </summary>
		private bool readOnly;

		/// <summary>
		/// Indicates whether the instance has already been disposed.
		/// </summary>
		private bool isDisposed;

		#endregion Private data

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the <see cref="RegistrySettingsStore"/> class.
		/// </summary>
		/// <param name="isGlobal">true to access the HKLM registry hive, false for HKCU.</param>
		/// <param name="baseKey">The base registry key to store the settings in.</param>
		public RegistrySettingsStore(bool isGlobal, string baseKey)
			: this(isGlobal, baseKey, false)
		{
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="RegistrySettingsStore"/> class.
		/// </summary>
		/// <param name="isGlobal">true to access the HKLM registry hive, false for HKCU.</param>
		/// <param name="baseKey">The base registry key to store the settings in.</param>
		/// <param name="readOnly">true to open the settings in read-only mode. This prevents any
		/// write access to the settings and will never write to the registry.</param>
		public RegistrySettingsStore(bool isGlobal, string baseKey, bool readOnly)
		{
			this.isGlobal = isGlobal;
			this.baseKey = baseKey;
			this.readOnly = readOnly;

			if (isGlobal)
			{
				hive = Registry.LocalMachine;
			}
			else
			{
				hive = Registry.CurrentUser;
			}
		}

		#endregion Constructors

		#region Write access

		/// <summary>
		/// Checks whether the passed object is of a data type that can be stored in the settings
		/// file. Throws an ArgumentException if the data type is unsupported.
		/// </summary>
		/// <param name="newValue">The value to check.</param>
		private void CheckType(object newValue)
		{
			// Unpack enum value
			// NOTE: This doesn't handle arrays of enums
			if (newValue.GetType().IsEnum)
			{
				newValue = Convert.ChangeType(newValue, newValue.GetType().GetEnumUnderlyingType());
			}
			// Check for supported type
			if (newValue is string ||
				newValue is string[] ||
				newValue is int ||
				newValue is int[] ||
				newValue is long ||
				newValue is long[] ||
				newValue is double ||
				newValue is double[] ||
				newValue is bool ||
				newValue is bool[] ||
				newValue is DateTime ||
				newValue is DateTime[] ||
				newValue is TimeSpan ||
				newValue is TimeSpan[])
			{
				return;
			}
			throw new ArgumentException("The data type is not supported: " + newValue.GetType().Name);
		}

		/// <summary>
		/// Sets a setting key to a new value.
		/// </summary>
		/// <param name="key">The setting key to update.</param>
		/// <param name="newValue">The new value for that key. Set null to remove the key.</param>
		public void Set(string key, object newValue)
		{
			lock (syncLock)
			{
				if (isDisposed) throw new ObjectDisposedException("");
				if (readOnly) throw new InvalidOperationException("This SettingsStore instance is created in read-only mode.");

				if (newValue == null)
				{
					Remove(key);
				}
				else
				{
					CheckType(newValue);

					// Unpack enum value
					if (newValue.GetType().IsEnum)
					{
						newValue = Convert.ChangeType(newValue, newValue.GetType().GetEnumUnderlyingType());
					}

					string regKey, regValue;
					SplitPath(key, baseKey, out regKey, out regValue);

					// Write to registry
					if (isGlobal)
					{
						regKey = @"HKEY_LOCAL_MACHINE\" + regKey;
					}
					else
					{
						regKey = @"HKEY_CURRENT_USER\" + regKey;
					}

					RegistryValueKind kind = RegistryValueKind.Unknown;
					if (newValue is string)
						kind = RegistryValueKind.String;
					else if (newValue is string[])
						kind = RegistryValueKind.MultiString;
					else if (newValue is int)
						kind = RegistryValueKind.DWord;
					else if (newValue is int[])
					{
						kind = RegistryValueKind.String;
						newValue = ((int[]) newValue)
							.Select(i => i.ToString(CultureInfo.InvariantCulture))
							.Aggregate((a, b) => a + "," + b);
					}
					else if (newValue is long)
						kind = RegistryValueKind.QWord;
					else if (newValue is long[])
					{
						kind = RegistryValueKind.String;
						newValue = ((long[]) newValue)
							.Select(i => i.ToString(CultureInfo.InvariantCulture))
							.Aggregate((a, b) => a + "," + b);
					}
					else if (newValue is double)
					{
						kind = RegistryValueKind.String;
						newValue = ((double) newValue).ToString(CultureInfo.InvariantCulture);
					}
					else if (newValue is double[])
					{
						kind = RegistryValueKind.String;
						newValue = ((double[]) newValue)
							.Select(i => i.ToString(CultureInfo.InvariantCulture))
							.Aggregate((a, b) => a + "," + b);
					}
					else if (newValue is bool)
						kind = RegistryValueKind.DWord;
					else if (newValue is bool[])
					{
						kind = RegistryValueKind.String;
						newValue = ((bool[]) newValue)
							.Select(i => i ? "1" : "0")
							.Aggregate((a, b) => a + "," + b);
					}
					else if (newValue is DateTime)
					{
						kind = RegistryValueKind.String;
						newValue = ((DateTime) newValue).ToString("o");
					}
					else if (newValue is DateTime[])
					{
						kind = RegistryValueKind.String;
						newValue = ((DateTime[]) newValue)
							.Select(i => i.ToString("o"))
							.Aggregate((a, b) => a + "," + b);
					}
					else if (newValue is TimeSpan)
					{
						kind = RegistryValueKind.QWord;
						newValue = ((TimeSpan) newValue).Ticks;
					}
					else if (newValue is TimeSpan[])
					{
						kind = RegistryValueKind.String;
						newValue = ((TimeSpan[]) newValue)
							.Select(i => i.Ticks.ToString(CultureInfo.InvariantCulture))
							.Aggregate((a, b) => a + "," + b);
					}

					Registry.SetValue(regKey, regValue, newValue, kind);
					OnPropertyChanged(key);
				}
			}
		}

		/// <summary>
		/// Removes a setting key from the settings store.
		/// </summary>
		/// <param name="key">The setting key to remove.</param>
		/// <returns>true if the value was renamed, false if it did not exist.</returns>
		public bool Remove(string key)
		{
			lock (syncLock)
			{
				if (isDisposed) throw new ObjectDisposedException("");
				if (readOnly) throw new InvalidOperationException("This SettingsStore instance is created in read-only mode.");

				string regKey, regValue;
				SplitPath(key, baseKey, out regKey, out regValue);

				// Delete in registry
				RegistryKey rk = hive.OpenSubKey(regKey, true);
				try
				{
					if (rk == null) return false;   // Registry key does not exist
					if (rk.GetValue(regValue) == null) return false;   // Value does not exist

					rk.DeleteValue(regValue);
					OnPropertyChanged(key);

					// Delete all keys that contain no values or subkeys anymore
					while (rk.GetValueNames().Length == 0 && rk.GetSubKeyNames().Length == 0)
					{
						rk.Close();
						rk = null;
						if (regKey.Equals(baseKey, StringComparison.OrdinalIgnoreCase))
							break;   // Don't traverse the base key!
						string childKey;
						regKey = GetParentRegKey(regKey, out childKey);
						if (regKey == null)
							break;   // At root element (oops…)
						rk = hive.OpenSubKey(regKey, true);
						rk.DeleteSubKey(childKey);
					}

					return true;
				}
				finally
				{
					if (rk != null)
						rk.Close();
				}
			}
		}

		/// <summary>
		/// Renames a setting key in the settings store.
		/// </summary>
		/// <param name="oldKey">The old setting key to rename.</param>
		/// <param name="newKey">The new setting key.</param>
		/// <returns>true if the value was renamed, false if it did not exist.</returns>
		public bool Rename(string oldKey, string newKey)
		{
			lock (syncLock)
			{
				if (isDisposed) throw new ObjectDisposedException("");
				if (readOnly) throw new InvalidOperationException("This SettingsStore instance is created in read-only mode.");

				object data = Get(oldKey);
				if (data != null)
				{
					Remove(oldKey);
					OnPropertyChanged(oldKey);

					Set(newKey, data);
					OnPropertyChanged(newKey);
					return true;
				}
				return false;
			}
		}

		#endregion Write access

		#region Read access

		/// <summary>
		/// Gets all setting keys that are currently set in this settings store.
		/// </summary>
		/// <returns></returns>
		public string[] GetKeys()
		{
			lock (syncLock)
			{
				if (isDisposed) throw new ObjectDisposedException("");

				// Read from registry (recursively)
				List<string> keys = new List<string>();
				using (var rk = hive.OpenSubKey(baseKey))
				{
					if (rk != null)
					{
						GetValuesRecursive(rk, keys);
					}
				}
				keys.Sort((a, b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase));
				return keys.ToArray();
			}
		}

		/// <summary>
		/// Gets the current value of a setting key, or null if the key is unset.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		/// <remarks>
		/// Since the original data type is not stored in the Windows registry, the raw value
		/// returned by this method may be in an unexpected format. Use another Get method to have
		/// the value converted to the requested type.
		/// </remarks>
		public object Get(string key)
		{
			lock (syncLock)
			{
				if (isDisposed) throw new ObjectDisposedException("");

				string regKey, regValue;
				SplitPath(key, baseKey, out regKey, out regValue);

				// Read from registry
				using (RegistryKey rk = hive.OpenSubKey(regKey))
				{
					if (rk == null) return null;   // Registry key does not exist
					object data = rk.GetValue(regValue);
					return data;
				}
			}
		}

		/// <summary>
		/// Gets the current bool value of a setting key, or false if the key is unset or has an
		/// incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public bool GetBool(string key)
		{
			return GetBool(key, false);
		}

		/// <summary>
		/// Gets the current bool value of a setting key, or a fallback value if the key is unset or
		/// has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public bool GetBool(string key, bool fallbackValue)
		{
			object data = Get(key);

			if (data == null) return fallbackValue;
			if (data.ToString().Trim() == "1" ||
				data.ToString().Trim().ToLower() == "true") return true;
			if (data.ToString().Trim() == "0" ||
				data.ToString().Trim().ToLower() == "false") return false;
			return fallbackValue;
		}

		/// <summary>
		/// Gets the current bool[] value of a setting key, or an empty array if the key is unset or
		/// has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public bool[] GetBoolArray(string key)
		{
			object data = Get(key);

			if (data == null) return new bool[0];
			return data.ToString()
				.Split(',')
				.Select(_ => _ != "0")
				.ToArray();
		}

		/// <summary>
		/// Gets the current int value of a setting key, or 0 if the key is unset or has an
		/// incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public int GetInt(string key)
		{
			return GetInt(key, 0);
		}

		/// <summary>
		/// Gets the current int value of a setting key, or a fallback value if the key is unset or
		/// has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public int GetInt(string key, int fallbackValue)
		{
			object data = Get(key);

			if (data == null) return fallbackValue;
			try
			{
				return Convert.ToInt32(data, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				return fallbackValue;
			}
		}

		/// <summary>
		/// Gets the current int[] value of a setting key, or an empty array if the key is unset or
		/// has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public int[] GetIntArray(string key)
		{
			object data = Get(key);

			if (data == null) return new int[0];
			return data.ToString()
				.Split(',')
				.Select(_ => Convert.ToInt32(_, CultureInfo.InvariantCulture))
				.ToArray();
		}

		/// <summary>
		/// Gets the current long value of a setting key, or 0 if the key is unset or has an
		/// incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public long GetLong(string key)
		{
			return GetLong(key, 0);
		}

		/// <summary>
		/// Gets the current long value of a setting key, or a fallback value if the key is unset or
		/// has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public long GetLong(string key, long fallbackValue)
		{
			object data = Get(key);

			if (data == null) return fallbackValue;
			try
			{
				return Convert.ToInt64(data, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				return fallbackValue;
			}
		}

		/// <summary>
		/// Gets the current long[] value of a setting key, or an empty array if the key is unset or
		/// has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public long[] GetLongArray(string key)
		{
			object data = Get(key);

			if (data == null) return new long[0];
			return data.ToString()
				.Split(',')
				.Select(_ => Convert.ToInt64(_, CultureInfo.InvariantCulture))
				.ToArray();
		}

		/// <summary>
		/// Gets the current double value of a setting key, or NaN if the key is unset or has an
		/// incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public double GetDouble(string key)
		{
			return GetDouble(key, double.NaN);
		}

		/// <summary>
		/// Gets the current double value of a setting key, or a fallback value if the key is unset
		/// or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public double GetDouble(string key, double fallbackValue)
		{
			object data = Get(key);

			if (data == null) return fallbackValue;
			try
			{
				return Convert.ToDouble(data, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				return fallbackValue;
			}
		}

		/// <summary>
		/// Gets the current double[] value of a setting key, or an empty array if the key is unset
		/// or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public double[] GetDoubleArray(string key)
		{
			object data = Get(key);

			if (data == null) return new double[0];
			return data.ToString()
				.Split(',')
				.Select(_ => Convert.ToDouble(_, CultureInfo.InvariantCulture))
				.ToArray();
		}

		/// <summary>
		/// Gets the current string value of a setting key, or "" if the key is unset.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public string GetString(string key)
		{
			return GetString(key, "");
		}

		/// <summary>
		/// Gets the current string value of a setting key, or a fallback value if the key is unset.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public string GetString(string key, string fallbackValue)
		{
			object data = Get(key);

			if (data == null) return fallbackValue;
			return Convert.ToString(data, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets the current string[] value of a setting key, or an empty array if the key is unset
		/// or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public string[] GetStringArray(string key)
		{
			object data = Get(key);

			if (data is string[]) return data as string[];
			return new string[0];
		}

		/// <summary>
		/// Gets the current DateTime value of a setting key, or DateTime.MinValue if the key is
		/// unset or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public DateTime GetDateTime(string key)
		{
			return GetDateTime(key, DateTime.MinValue);
		}

		/// <summary>
		/// Gets the current DateTime value of a setting key, or a fallback value if the key is
		/// unset or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public DateTime GetDateTime(string key, DateTime fallbackValue)
		{
			object data = Get(key);

			if (data == null) return fallbackValue;
			try
			{
				return DateTime.Parse((string) data, null, DateTimeStyles.RoundtripKind);
			}
			catch (FormatException)
			{
				return fallbackValue;
			}
		}

		/// <summary>
		/// Gets the current DateTime[] value of a setting key, or an empty array if the key is
		/// unset or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public DateTime[] GetDateTimeArray(string key)
		{
			object data = Get(key);

			if (data == null) return new DateTime[0];
			return data.ToString()
				.Split(',')
				.Select(_ => DateTime.Parse(_, null, DateTimeStyles.RoundtripKind))
				.ToArray();
		}

		/// <summary>
		/// Gets the current TimeSpan value of a setting key, or TimeSpan.Zero if the key is unset
		/// or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public TimeSpan GetTimeSpan(string key)
		{
			return GetTimeSpan(key, TimeSpan.Zero);
		}

		/// <summary>
		/// Gets the current TimeSpan value of a setting key, or a fallback value if the key is
		/// unset or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <param name="fallbackValue">The fallback value to return if the key is unset.</param>
		/// <returns></returns>
		public TimeSpan GetTimeSpan(string key, TimeSpan fallbackValue)
		{
			return new TimeSpan(GetLong(key, fallbackValue.Ticks));
		}

		/// <summary>
		/// Gets the current TimeSpan[] value of a setting key, or an empty array if the key is
		/// unset or has an incompatible data type.
		/// </summary>
		/// <param name="key">The setting key.</param>
		/// <returns></returns>
		public TimeSpan[] GetTimeSpanArray(string key)
		{
			object data = Get(key);

			if (data == null) return new TimeSpan[0];
			return data.ToString()
				.Split(',')
				.Select(_ => new TimeSpan(Convert.ToInt64(_, CultureInfo.InvariantCulture)))
				.ToArray();
		}

		#endregion Read access

		#region Helper methods

		private static void SplitPath(string key, string regBaseKey, out string regKey, out string regValue)
		{
			int pointIndex = key.LastIndexOf('.');
			regKey = regBaseKey;
			regValue = key;
			if (pointIndex != -1)
			{
				regKey += "\\" + key.Substring(0, pointIndex).Replace('.', '\\');
				regValue = key.Substring(pointIndex + 1);
			}
		}

		private static string GetParentRegKey(string regKey, out string childKey)
		{
			childKey = regKey;
			int index = regKey.LastIndexOf('\\');
			if (index == -1)
				return null;   // Root element
			childKey = regKey.Substring(index + 1);
			regKey = regKey.Substring(0, index);
			if (regKey.StartsWith("HKEY_", StringComparison.OrdinalIgnoreCase) && regKey.IndexOf('\\') == -1)
				return null;   // Root element is no path
			return regKey;
		}

		private static void GetValuesRecursive(RegistryKey rk, List<string> keys)
		{
			keys.AddRange(rk.GetValueNames());
			foreach (var subKey in rk.GetSubKeyNames())
			{
				using (var subRk = rk.OpenSubKey(subKey))
				{
					if (subRk != null)
					{
						GetValuesRecursive(subRk, keys);
					}
				}
			}
		}

		#endregion Helper methods

		#region IDisposable members

		/// <summary>
		/// Frees all resources.
		/// </summary>
		public void Dispose()
		{
			lock (syncLock)
			{
				if (!isDisposed)
				{
					isDisposed = true;
					// Nothing to do here, actually.
				}
			}
		}

		#endregion IDisposable members

		#region INotifyPropertyChanged members

		/// <summary>
		/// Occurs when a setting key value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="key">Name of the setting key that has changed.</param>
		protected void OnPropertyChanged(string key)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(key));
			}
		}

		#endregion INotifyPropertyChanged members
	}
}
