using JSONSystem;
using System.Collections.Generic;
using System.IO;

namespace ModSystem
{
	public static class ModLoader
	{
		private static string dataDirectory;
		private static bool loadedMods = false;
		private static Mod[] mods;

		public static void LoadMods()
		{
			if (loadedMods)
			{
				return;
			}
			dataDirectory = GlobalConsts.GetDataPath() + "/GameData/";
			List<Mod> modList = new List<Mod>();
			string[] modDirectories = Directory.GetDirectories(dataDirectory);
			foreach (string modName in modDirectories)
			{
				if (new DirectoryInfo(modName).Name == "Localization")
				{
					continue;
				}
				Mod m = new Mod(new DirectoryInfo(modName).Name);
				string jsonFilename = modName + "/modinfo.json";
				string rawJSON = JSONSystem.JSON.GetRawJSONFromFile(jsonFilename);
				JSONNode output = JSONSystem.JSON.Parse(rawJSON);

				JSONArray scripts = output["scripts"].AsArray;
				foreach (JSONNode node in scripts)
				{
					m.AddScript(node);
				}

				JSONArray sprites = output["sprites"].AsArray;
				foreach (JSONNode node in sprites)
				{
					m.AddSprite(node);
				}
				modList.Add(m);
			}
			mods = new Mod[modList.Count];
			int count = 1;
			foreach (Mod m in modList)
			{
				// Load base game mod first, then anything else overwrites base game
				if (m.modname == GlobalConsts.GAME_BASE_MOD_NAME)
				{
					mods[0] = m;
				}
				else
				{
					if (count == modList.Count && mods[0] == null)
					{
						count = 0;
					}
					mods[count] = m;
					count++;
				}
			}
			// This forces later-loaded mods to overwrite what is loaded in earlier mods.
			for (int i = 0; i < mods.Length; i++)
			{
				Mod m = mods[i];
				for (int j = i - 1; j >= 0; j--)
				{
					Mod old = mods[j];
					foreach (string s in m.GetScripts())
					{
						if (old.WillLoadScript(s))
						{
							old.DisableScript(s);
						}
					}
					foreach (string s in m.GetSprites())
					{
						if (old.WillLoadSprite(s))
						{
							old.DisableSprite(s);
						}
					}
				}
			}
			loadedMods = true;
		}

		public static Mod[] GetMods()
		{
			if (mods.Length == 0)
			{
				LoadMods();
			}
			return mods;
		}

		public static string[] GetLoadOrder()
		{
			string[] loadOrder = new string[mods.Length];
			for (int i = 0; i < mods.Length; i++)
			{
				loadOrder[i] = mods[i].modname;
			}
			return loadOrder;
		}

		public static void SetLoadOrder(string[] newOrder)
		{
			Mod[] newModOrder = new Mod[mods.Length];
			for (int i = 0; i < newOrder.Length; i++)
			{
				Mod newM = null;
				foreach (Mod m in mods)
				{
					if (m.modname == newOrder[i])
					{
						newM = m;
						break;
					}
				}
				newModOrder[i] = newM;
			}
			mods = newModOrder;
		}

		public static string[] GetScripts()
		{
			List<string> output = new List<string>();
			foreach (Mod m in GetMods())
			{
				output.AddRange(m.GetScripts());
			}
			return output.ToArray();
		}
		public static string[] GetSprites()
		{
			List<string> output = new List<string>();
			foreach (Mod m in GetMods())
			{
				output.AddRange(m.GetSprites());
			}
			return output.ToArray();
		}
	}
}