using JSONSystem;
using UnityEngine;

namespace Localization
{
	public enum CurrentLanguage
	{
		English
	}

	public static class LocalizationSystem
	{
		private static JSONClass localizedStrings;
		private static bool loadedLocalization = false;
		private static CurrentLanguage language;

		private static void LoadLocalization()
		{
			string currentLanguage = PlayerPrefs.GetString("language", "English");
			language = (CurrentLanguage)System.Enum.Parse(typeof(CurrentLanguage), currentLanguage, true);
			string fileName = GlobalConsts.GetDataPath() + "/GameData/Localization/" + language.ToString() + ".json";
			if (!System.IO.File.Exists(fileName))
			{
				fileName = GlobalConsts.GetDataPath() + "/GameData/Localization/English.json";
				if (!System.IO.File.Exists(fileName))
				{
					System.IO.File.WriteAllText(fileName, "{}");
				}
			}
			string rawJSON = JSONSystem.JSON.GetRawJSONFromFile(fileName);
			localizedStrings = JSONSystem.JSON.Parse(rawJSON) as JSONClass;
		}

		public static string GetText(string id)
		{
			if (localizedStrings == null && !loadedLocalization)
			{
				LoadLocalization();
				loadedLocalization = true;
			}
			else if (localizedStrings == null)
			{
				return id;
			}
			string output = localizedStrings[id];
			if (output == null || output == "")
			{
				output = id;
			}
			return output;
		}

		public static void SetText(string id, string value)
		{
			if (language == CurrentLanguage.English)
			{
				if (localizedStrings["id"] == null)
				{
					localizedStrings.Add(id, new JSONData(value));
				}
				else
				{
					localizedStrings[id] = value;
				}
				string fileName = GlobalConsts.GetDataPath() + "/GameData/Localization/English.json";
				if (System.IO.File.Exists(fileName))
				{
					System.IO.File.Delete(fileName);
				}
				System.IO.File.WriteAllText(fileName, localizedStrings.ToString("\n"));
			}
		}
	}
}