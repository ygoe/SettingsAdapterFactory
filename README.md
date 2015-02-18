# SettingsAdapterFactory

Generates a dynamic implementation of an interface with properties that binds to a settings store and implements INotifyPropertyChanged.

See http://unclassified.software/source/settingsadapterfactory for further information.

## These are the interesting files here

* SettingsDemo/Unclassified/Util/**SettingsAdapterFactory.cs**<br>
  Interface implemmentation factory. The dynamic code is produced here.
* SettingsDemo/Unclassified/Util/**SettingsHelper.cs**<br>
  Provides methods for specialized settings situations.
* SettingsDemo/Unclassified/Util/**FileSettingsStore.cs**<br>
  A data store that contains all setting keys and values of a specified setting file.
* SettingsDemo/Unclassified/Util/**RegistrySettingsStore.cs**<br>
  A data store that contains all setting keys and values of a specified registry key.

Copy the first two files and one of the last two into your project to use this code.

## Usage of the factory

Define your settings structure:

```csharp
public interface IAppSettings : ISettings
{
  string LastStartedAppVersion { get; set; }
  string[] RecentlyLoadedFiles { get; set; }
  bool IsSoundEnabled { get; set; }
}
```

Then create an instance of its implementation:

```csharp
IAppSettings settings = SettingsAdapterFactory.New<IAppSettings>(
  new FileSettingsStore(
    SettingsHelper.GetAppDataPath(@"Company\Product", "Product.conf")));
```

The settings instance is created in SettingsDemo/App.xaml.cs, `InitializeSettings()`.
