using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyProduct("SettingsDemo")]
[assembly: AssemblyTitle("SettingsDemo")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCopyright("© 2015 Yves Goergen")]
[assembly: AssemblyCompany("")]

// Assembly version, also used for Win32 file version resource.
// Must be a plain numeric version definition:
// 1. Major version number, should be increased with major new versions or rewrites of the application
// 2. Minor version number, should ne increased with minor feature changes or new features
// 3. Bugfix number, should be set or increased for bugfix releases of a previous version
// 4. Unused
[assembly: AssemblyVersion("1.0.0")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]
[assembly: ThemeInfo(
	// Where theme specific resource dictionaries are located
	// (used if a resource is not found in the page, or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly,
	// Where the generic resource dictionary is located
	// (used if a resource is not found in the page, app, or any theme specific resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly)]
