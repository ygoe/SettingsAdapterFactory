using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SettingsDemo
{
	/// <summary>
	/// Provides extension methods that extend the application settings interface by additional
	/// analysis or actions.
	/// </summary>
	public static class AppSettingsExtensions
	{
		// EXPLANATION:
		// The implemented interface only provides properties with the defined behaviour. No methods
		// or additional aggregating or analysing properties are provided. Extension methods can be
		// used to extend the functionality of these settings interfaces.
		// NOTE: Be aware that no "property" change notification can be provided for these methods.
		
		/// <summary>
		/// Gets the native culture name.
		/// </summary>
		/// <param name="settings">The settings instance.</param>
		/// <returns>The native culture name.</returns>
		public static string GetCultureName(this IAppSettings settings)
		{
			var ci = new CultureInfo(settings.Culture);
			return ci.NativeName;
		}

		public static int TestNumbersSum(this IAppSettings settings)
		{
			return settings.TestNumbers.Sum();
		}
	}

	public static class ViewSettingsExtensions
	{
		public static string FontName(this IViewSettings settings)
		{
			if (settings.MonospaceFont)
			{
				return "Consolas";
			}
			else
			{
				return "Segoe UI";
			}
		}
	}
}
