using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SettingsDemo;
using Unclassified.Util;

namespace UnitTests
{
	internal static class CommonMethods
	{
		public static void EmptyFallback(ISettingsStore store)
		{
			Assert.AreEqual(false, store.GetBool("bool_key"));
			Assert.AreEqual(false, store.GetBool("bool_key", false));
			Assert.AreEqual(true, store.GetBool("bool_key", true));

			bool[] boolArr = store.GetBoolArray("bool_arr_key");
			Assert.AreEqual(0, boolArr.Length);

			Assert.AreEqual(0, store.GetInt("int_key"));
			Assert.AreEqual(0, store.GetInt("int_key", 0));
			Assert.AreEqual(9, store.GetInt("int_key", 9));

			int[] intArr = store.GetIntArray("int_arr_key");
			Assert.AreEqual(0, intArr.Length);

			Assert.AreEqual(0, store.GetLong("long_key"));
			Assert.AreEqual(0, store.GetLong("long_key", 0));
			Assert.AreEqual(9000, store.GetLong("long_key", 9000));

			long[] longArr = store.GetLongArray("long_arr_key");
			Assert.AreEqual(0, longArr.Length);

			Assert.AreEqual(double.NaN, store.GetDouble("double_key"));
			Assert.AreEqual(0, store.GetDouble("double_key", 0));
			Assert.AreEqual(9.25, store.GetDouble("double_key", 9.25));

			double[] doubleArr = store.GetDoubleArray("double_arr_key");
			Assert.AreEqual(0, doubleArr.Length);

			Assert.AreEqual("", store.GetString("string_key"));
			Assert.AreEqual(null, store.GetString("string_key", null));
			Assert.AreEqual("Abc", store.GetString("string_key", "Abc"));

			string[] stringArr = store.GetStringArray("string_arr_key");
			Assert.AreEqual(0, stringArr.Length);

			Assert.AreEqual(DateTime.MinValue, store.GetDateTime("datetime_key"));
			Assert.AreEqual(new DateTime(2000, 1, 1), store.GetDateTime("datetime_key", new DateTime(2000, 1, 1)));

			DateTime[] dateTimeArr = store.GetDateTimeArray("datetime_arr_key");
			Assert.AreEqual(0, dateTimeArr.Length);

			Assert.AreEqual(TimeSpan.Zero, store.GetTimeSpan("timespan_key"));
			Assert.AreEqual(new TimeSpan(1, 2, 3), store.GetTimeSpan("timespan_key", new TimeSpan(1, 2, 3)));

			TimeSpan[] timeSpanArr = store.GetTimeSpanArray("timespan_arr_key");
			Assert.AreEqual(0, timeSpanArr.Length);

			NameValueCollection collection = store.GetNameValueCollection("namevaluecollection_key");
			Assert.AreEqual(0, collection.Count);
		}

		public static void Set1(ISettingsStore store)
		{
			store.Set("bool_key", true);
			store.Set("bool_arr_key", new bool[] { false, true, true });
			store.Set("int_key", 8);
			store.Set("int_arr_key", new int[] { 2, 4, 9, 9 });
			store.Set("long_key", 8L);
			store.Set("long_arr_key", new long[] { 3, 9 });
			store.Set("double_key", 8.25);
			store.Set("double_arr_key", new double[] { 4.5, double.NaN, 6 });
			store.Set("string_key", "Xyz");
			store.Set("string_arr_key", new string[] { "Abc", "", "0" });
			store.Set("datetime_key", new DateTime(2001, 2, 3));
			store.Set("datetime_arr_key", new DateTime[] { new DateTime(2010, 2, 4) });
			store.Set("timespan_key", new TimeSpan(2, 3, 4));
			store.Set("timespan_arr_key", new TimeSpan[] { new TimeSpan(123456) });

			NameValueCollection collection = new NameValueCollection();
			collection["a"] = "va";
			collection["aa"] = "vaa";
			collection["B"] = "vB";
			store.Set("namevaluecollection_key", collection);
		}

		public static void GetTest1(ISettingsStore store)
		{
			Assert.AreEqual(true, store.GetBool("bool_key"));
			Assert.AreEqual(true, store.GetBool("bool_key", false));

			bool[] boolArr = store.GetBoolArray("bool_arr_key");
			Assert.AreEqual(3, boolArr.Length);
			Assert.AreEqual(false, boolArr[0]);
			Assert.AreEqual(true, boolArr[1]);
			Assert.AreEqual(true, boolArr[2]);

			Assert.AreEqual(8, store.GetInt("int_key"));
			Assert.AreEqual(8, store.GetInt("int_key", 0));

			int[] intArr = store.GetIntArray("int_arr_key");
			Assert.AreEqual(4, intArr.Length);
			Assert.AreEqual(2, intArr[0]);
			Assert.AreEqual(4, intArr[1]);
			Assert.AreEqual(9, intArr[2]);
			Assert.AreEqual(9, intArr[3]);

			Assert.AreEqual(8L, store.GetLong("long_key"));
			Assert.AreEqual(8L, store.GetLong("long_key", 0L));

			long[] longArr = store.GetLongArray("long_arr_key");
			Assert.AreEqual(2, longArr.Length);
			Assert.AreEqual(3, longArr[0]);
			Assert.AreEqual(9, longArr[1]);

			Assert.AreEqual(8.25, store.GetDouble("double_key"));
			Assert.AreEqual(8.25, store.GetDouble("double_key", 0.0));

			double[] doubleArr = store.GetDoubleArray("double_arr_key");
			Assert.AreEqual(3, doubleArr.Length);
			Assert.AreEqual(4.5, doubleArr[0]);
			Assert.AreEqual(double.NaN, doubleArr[1]);
			Assert.AreEqual(6, doubleArr[2]);

			Assert.AreEqual("Xyz", store.GetString("string_key"));
			Assert.AreEqual("Xyz", store.GetString("string_key", ""));

			string[] stringArr = store.GetStringArray("string_arr_key");
			Assert.AreEqual(3, stringArr.Length);
			Assert.AreEqual("Abc", stringArr[0]);
			Assert.AreEqual("", stringArr[1]);
			Assert.AreEqual("0", stringArr[2]);

			Assert.AreEqual(new DateTime(2001, 2, 3), store.GetDateTime("datetime_key"));
			Assert.AreEqual(new DateTime(2001, 2, 3), store.GetDateTime("datetime_key", DateTime.MinValue));

			DateTime[] dateTimeArr = store.GetDateTimeArray("datetime_arr_key");
			Assert.AreEqual(1, dateTimeArr.Length);
			Assert.AreEqual(new DateTime(2010, 2, 4), dateTimeArr[0]);

			Assert.AreEqual(new TimeSpan(2, 3, 4), store.GetTimeSpan("timespan_key"));
			Assert.AreEqual(new TimeSpan(2, 3, 4), store.GetTimeSpan("timespan_key", TimeSpan.Zero));

			TimeSpan[] timeSpanArr = store.GetTimeSpanArray("timespan_arr_key");
			Assert.AreEqual(1, timeSpanArr.Length);
			Assert.AreEqual(new TimeSpan(123456), timeSpanArr[0]);

			NameValueCollection collection = store.GetNameValueCollection("namevaluecollection_key");
			Assert.AreEqual(3, collection.Count);
			Assert.AreEqual("a", collection.GetKey(0));
			Assert.AreEqual("aa", collection.GetKey(1));
			Assert.AreEqual("B", collection.GetKey(2));
			Assert.AreEqual("va", collection["a"]);
			Assert.AreEqual("vaa", collection["aa"]);
			Assert.AreEqual("vB", collection["B"]);
		}

		public static void Set2(ISettingsStore store)
		{
			store.Set("bool_key", false);
			store.Set("int_key", 0);
			store.Set("long_key", 0L);
			store.Set("double_key", 0.0);
			store.Set("string_key", "");
			store.Set("datetime_key", DateTime.MinValue);
			store.Set("timespan_key", TimeSpan.Zero);

			store.Set("bool_arr_key", null);
		}

		public static void GetTest2(ISettingsStore store)
		{
			Assert.AreEqual(false, store.GetBool("bool_key"));
			Assert.AreEqual(false, store.GetBool("bool_key", true));

			Assert.AreEqual(0, store.GetInt("int_key"));
			Assert.AreEqual(0, store.GetInt("int_key", 5));

			Assert.AreEqual(0L, store.GetLong("long_key"));
			Assert.AreEqual(0L, store.GetLong("long_key", 5));

			Assert.AreEqual(0.0, store.GetDouble("double_key"));
			Assert.AreEqual(0.0, store.GetDouble("double_key", 5));

			Assert.AreEqual("", store.GetString("string_key"));
			Assert.AreEqual("", store.GetString("string_key", "Test"));

			Assert.AreEqual(DateTime.MinValue, store.GetDateTime("datetime_key"));
			Assert.AreEqual(DateTime.MinValue, store.GetDateTime("datetime_key", DateTime.MaxValue));

			Assert.AreEqual(TimeSpan.Zero, store.GetTimeSpan("timespan_key"));
			Assert.AreEqual(TimeSpan.Zero, store.GetTimeSpan("timespan_key", new TimeSpan(5)));

			bool[] boolArr = store.GetBoolArray("bool_arr_key");
			Assert.AreEqual(0, boolArr.Length);
		}

		public static void AdapterEmptyDefault(IAppSettings settings)
		{
			Assert.AreEqual("", settings.LastStartedAppVersion);
			Assert.AreEqual("de-DE", settings.Culture);
			Assert.AreEqual(0, settings.RecentlyLoadedFiles.Count);
			Assert.AreEqual(0, settings.TestMap.Count);
			Assert.AreEqual(0, settings.TestNumbers.Length);
			Assert.AreEqual(false, settings.IsSoundEnabled);

			Assert.AreEqual(false, settings.View.MonospaceFont);
			Assert.AreEqual(TimeType.Remote, settings.View.TimeType);
			Assert.AreEqual(15, settings.View.IndentSize);
		}

		public static void AdapterSet(IAppSettings settings)
		{
			settings.LastStartedAppVersion = "1.2.3";
			settings.Culture = "en-GB";
			settings.RecentlyLoadedFiles.Add("Abc");
			settings.RecentlyLoadedFiles.Add("def");
			settings.TestMap["Aaa"] = 3;
			settings.TestMap["bBB"] = 4;
			settings.TestMap["Xyz"] = 20;
			settings.TestNumbers = new int[] { 4, 32, 0 };
			settings.IsSoundEnabled = true;

			settings.View.MonospaceFont = false;
			settings.View.TimeType = TimeType.Utc;
			settings.View.IndentSize = 20;
		}

		public static void AdapterGetTest(IAppSettings settings)
		{
			Assert.AreEqual("1.2.3", settings.LastStartedAppVersion);
			Assert.AreEqual("en-GB", settings.Culture);
			Assert.AreEqual(2, settings.RecentlyLoadedFiles.Count);
			Assert.AreEqual("Abc", settings.RecentlyLoadedFiles[0]);
			Assert.AreEqual("def", settings.RecentlyLoadedFiles[1]);
			Assert.AreEqual(3, settings.TestMap.Count);
			Assert.AreEqual(3, settings.TestMap["Aaa"]);
			Assert.AreEqual(4, settings.TestMap["bBB"]);
			Assert.AreEqual(20, settings.TestMap["Xyz"]);
			Assert.AreEqual(3, settings.TestNumbers.Length);
			Assert.AreEqual(4, settings.TestNumbers[0]);
			Assert.AreEqual(32, settings.TestNumbers[1]);
			Assert.AreEqual(0, settings.TestNumbers[2]);
			Assert.AreEqual(true, settings.IsSoundEnabled);

			Assert.AreEqual(false, settings.View.MonospaceFont);
			Assert.AreEqual(TimeType.Utc, settings.View.TimeType);
			Assert.AreEqual(20, settings.View.IndentSize);
		}

		public static void RemovePattern(ISettingsStore store)
		{
			int count = store.GetKeys().Length;
			store.Set("pattern1", 1);
			store.Set("pattern2", 2);
			store.Set("pattern3", 3);
			store.Set("pattern4", 4);
			store.Set("pattern5", 5);
			store.RemovePattern(@"^pattern[1-3]");
			int count2 = store.GetKeys().Length;

			Assert.AreEqual(count + 2, count2, "Unexpected number of keys after removing with pattern");
			Assert.IsFalse(store.GetKeys().Contains("pattern1"));
			Assert.IsFalse(store.GetKeys().Contains("pattern2"));
			Assert.IsFalse(store.GetKeys().Contains("pattern3"));
			Assert.IsTrue(store.GetKeys().Contains("pattern4"));
			Assert.IsTrue(store.GetKeys().Contains("pattern5"));
		}
	}
}
