using HelperFunctions.Localization;
using UnityEngine;

namespace HelperFunctions
{

	[System.Serializable]
	public class LocalizedString
	{
		public LocalizedString(string id)
		{
			_ID = id;
		}
		public LocalizedString(string id, string defaultValue)
		{
			LocalizeText(id, defaultValue);
		}

		public LocalizedString(string id, string defaultValue, object[] replace)
		{
			LocalizeText(id, defaultValue);
			ReplaceText(replace);
		}

		public LocalizedString(System.Collections.Generic.List<LocalizedString> components)
		{
			foreach (LocalizedString lString in components)
			{
				_Text += lString.Text;
			}
		}

		public LocalizedString(LocalizedString[] components)
		{
			foreach (LocalizedString lString in components)
			{
				_Text += lString.Text;
			}
		}
		private LocalizedString()
		{
		}
		[SerializeField]
		private string _ID = "";

		[SerializeField]
		private string _Text = "";

		public string Text
		{
			get
			{
				if (_Text != "" || _ID == GlobalConsts.SKIP_LOCALIZATION)
				{
					return _Text;
				}
				return LocalizationSystem.GetText(_ID);
			}
			set
			{
				if (LocalizationSystem.GetText(_ID) != value)
				{
					LocalizationSystem.SetText(_ID, value);
				}
			}
		}

		public string ID
		{
			get
			{
				return _ID;
			}
		}

		private void LocalizeText(string id, string defaultValue)
		{
			if (id == GlobalConsts.SKIP_LOCALIZATION)
			{
				if (!defaultValue.Contains("_"))
				{
					_Text = defaultValue;
					return;
				}
			}
			_ID = id;
			Text = defaultValue;
		}

		private void ReplaceText(object[] replace)
		{
			if (replace == null)
			{
				return;
			}
			System.Collections.Generic.List<string> replaceStrings = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<float> replaceFloats = new System.Collections.Generic.List<float>();
			foreach (object o in replace)
			{
				if (o is string)
				{
					replaceStrings.Add((string)o);
				}
				else if (o is System.Exception)
				{
					replaceStrings.Add(((System.Exception)o).ToString());
				}
				else if (o is int || o is float)
				{
					float value = 0.0f;
					if (o is int)
					{
						value = (int)o;
					}
					else if (o is float)
					{
						value = (float)o;
					}
					replaceFloats.Add(value);
				}
			}
			string output = Text;
			string[] floatSplit = output.Split(new string[] { "%d" }, System.StringSplitOptions.None);
			if (floatSplit.Length > 0)
			{
				output = "";
				for (int i = 0; i < floatSplit.Length; i++)
				{
					if (i >= replaceFloats.Count)
					{
						output += floatSplit[i];
						continue;
					}
					output += floatSplit[i] + replaceFloats[i];
				}
			}
			string[] stringSplit = output.Split(new string[] { "%s" }, System.StringSplitOptions.None);
			if (stringSplit.Length > 0)
			{
				output = "";
				for (int i = 0; i < stringSplit.Length; i++)
				{
					if (i >= replaceStrings.Count)
					{
						output += stringSplit[i];
						continue;
					}
					output += stringSplit[i] + replaceStrings[i];
				}
			}
			_Text = output;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}