using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SettingsDemo;
using Unclassified.Util;

namespace UnitTests
{
	[TestClass]
	public class FileSettingsStoreTests
	{
		[TestMethod]
		public void FileSettingsStoreEmptyFallback()
		{
			File.Delete("fallback.conf");
			using (var store = new FileSettingsStore("fallback.conf"))
			{
				CommonMethods.EmptyFallback(store);
			}
		}

		[TestMethod]
		public void FileSettingsStoreSetGet()
		{
			File.Delete("setget.conf");
			using (var store = new FileSettingsStore("setget.conf"))
			{
				CommonMethods.Set1(store);
				CommonMethods.GetTest1(store);
			}
			using (var store = new FileSettingsStore("setget.conf"))
			{
				CommonMethods.GetTest1(store);
			}
			using (var store = new FileSettingsStore("setget.conf"))
			{
				CommonMethods.Set2(store);
				CommonMethods.GetTest2(store);
			}
			using (var store = new FileSettingsStore("setget.conf"))
			{
				CommonMethods.GetTest2(store);
			}
		}

		[TestMethod]
		public void FileSettingsStoreAdapter()
		{
			File.Delete("adapter.conf");
			using (var store = new FileSettingsStore("adapter.conf"))
			{
				IAppSettings settings = SettingsAdapterFactory.New<IAppSettings>(store);
				Assert.IsNotNull(settings);
				CommonMethods.AdapterEmptyDefault(settings);
				CommonMethods.AdapterSet(settings);
				CommonMethods.AdapterGetTest(settings);
			}
			using (var store = new FileSettingsStore("adapter.conf"))
			{
				IAppSettings settings = SettingsAdapterFactory.New<IAppSettings>(store);
				Assert.IsNotNull(settings);
				CommonMethods.AdapterGetTest(settings);
			}
		}
		
		[TestMethod]
		public void FileSettingsStoreRemovePattern()
		{
			File.Delete("pattern.conf");
			using (var store = new FileSettingsStore("pattern.conf"))
			{
				CommonMethods.RemovePattern(store);
			}
		}
	}
}
