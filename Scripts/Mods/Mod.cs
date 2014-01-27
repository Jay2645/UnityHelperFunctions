using System.Collections.Generic;
namespace ModSystem
{
	public class Mod
	{
		public Mod(string name)
		{
			modname = name;
		}
		public string modname;

		// These are all the "actual" things the mods will load
		// If a player disables something, it is removed from the list
		private List<string> scripts = new List<string>();
		private List<string> sprites = new List<string>();

		// These all hold everything the mod can possibly load
		// Even things that are disabled remain in here
		private List<string> fullScripts = new List<string>();
		private List<string> fullSprites = new List<string>();

		private string MakeFilename(string input)
		{
			return "/GameData/" + modname + "/" + input;
		}

		public string RemoveFilename(string input)
		{
			input = input.Replace(GlobalConsts.GetDataPath(), "");
			return input.Replace("/GameData/" + modname + "/", "");
		}

		private void AddToList(string filename, List<string> primary, List<string> full)
		{
			filename = MakeFilename(filename);
			primary.Add(filename);
			if (!full.Contains(filename))
			{
				full.Add(filename);
			}
		}

		private void RemoveFromList(string filename, List<string> list)
		{
			// Make sure that the given filename matches what we have in the list
			filename = RemoveFilename(filename); // Removes any extra bits before the filename, i.e. C:/Blah/Blah/Blah
			filename = MakeFilename(filename); // Puts the stuff that the list would contain back in
			if (list.Contains(filename))
			{
				list.Remove(filename);
				return;
			}
			// Have yet to find it, so now we isolate the file in question
			filename = System.IO.Path.GetFileNameWithoutExtension(filename);
			for (int i = 0; i < list.Count; i++)
			{
				string listFile = System.IO.Path.GetFileNameWithoutExtension(list[i]);
				if (filename == listFile)
				{
					list.RemoveAt(i);
					return;
				}
			}
			Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_COULD_NOT_FIND_IN_MOD, "Could not find %s in %s!", new object[] { filename, modname }));
		}

		public void AddScript(string filename)
		{
			AddToList(filename, scripts, fullScripts);
		}
		public void AddSprite(string filename)
		{
			AddToList(filename, sprites, fullSprites);
		}

		public List<string> GetScripts()
		{
			return scripts;
		}
		public List<string> GetSprites()
		{
			return sprites;
		}

		public void DisableScript(string filename)
		{
			RemoveFromList(filename, scripts);
		}
		public void DisableSprite(string filename)
		{
			RemoveFromList(filename, sprites);
		}

		private bool WillLoad(string filename, List<string> list)
		{
			filename = System.IO.Path.GetFileNameWithoutExtension(filename);
			for (int i = 0; i < list.Count; i++)
			{
				string listFile = System.IO.Path.GetFileNameWithoutExtension(list[i]);
				if (listFile == filename)
				{
					return true;
				}
			}
			return false;
		}

		public bool WillLoadScript(string filename)
		{
			return WillLoad(filename, scripts);
		}
		public bool WillLoadSprite(string filename)
		{
			return WillLoad(filename, sprites);
		}
	}
}