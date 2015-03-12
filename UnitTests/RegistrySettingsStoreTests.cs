using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SettingsDemo;
using Unclassified.Util;

namespace UnitTests
{
	[TestClass]
	public class RegistrySettingsStoreTests
	{
		[TestMethod]
		public void RegistrySettingsStoreEmptyFallback()
		{
			Registry.CurrentUser.DeleteSubKeyTree(@"Software\Unclassified\SettingsDemoTest\EmptyFallback", false);
			using (var store = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemoTest\EmptyFallback"))
			{
				CommonMethods.EmptyFallback(store);
			}
		}

		[TestMethod]
		public void RegistrySettingsStoreSetGet()
		{
			Registry.CurrentUser.DeleteSubKeyTree(@"Software\Unclassified\SettingsDemoTest\SetGet", false);
			using (var store = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemoTest\SetGet"))
			{
				CommonMethods.Set1(store);
			}
			using (var store = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemoTest\SetGet"))
			{
				CommonMethods.GetTest1(store);
			}
			using (var store = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemoTest\SetGet"))
			{
				CommonMethods.Set2(store);
			}
			using (var store = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemoTest\SetGet"))
			{
				CommonMethods.GetTest2(store);
			}
		}

		[TestMethod]
		public void FileSettingsStoreAdapter()
		{
			Registry.CurrentUser.DeleteSubKeyTree(@"Software\Unclassified\SettingsDemoTest\Adapter", false);
			using (var store = new RegistrySettingsStore(false, @"Software\Unclassified\SettingsDemoTest\Adapter"))
			{
				IAppSettings settings = SettingsAdapterFactory.New<IAppSettings>(store);
				Assert.IsNotNull(settings);
				CommonMethods.AdapterEmptyDefault(settings);
				CommonMethods.AdapterSet(settings);
				CommonMethods.AdapterGetTest(settings);
			}
		}
	}
}
