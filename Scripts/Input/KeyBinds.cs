using Commands;
using JSONSystem;
using System.Collections.Generic;
using UnityEngine;

public static class KeyBinds
{
	private static Dictionary<KeyCode, InputCommand> keysPressed
	{
		get
		{
			if (_keysPressed == null)
			{
				InitializeDictionaries();
			}
			return _keysPressed;
		}
		set
		{
			_keysPressed = value;
		}
	}
	private static Dictionary<KeyCode, InputCommand> _keysPressed;

	private static void InitializeDictionaries()
	{
		string rawJSON = PlayerPrefs.GetString("bindings", "");
		if (rawJSON == "")
		{
			rawJSON = GetDefaultBindings();
		}
		JSONClass bindings = JSONSystem.JSON.Parse(rawJSON).AsObject;
		keysPressed = new Dictionary<KeyCode, InputCommand>();
		for (int i = 0; i < bindings.Count; i++)
		{
			string keyStr = bindings.GetKey(i);
			string actionStr = bindings[i];
			KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyStr, true);
			InputCommand action = Command.FromString(actionStr) as InputCommand;
			BindKey(action, key);
		}
	}

	private static string GetDefaultBindings()
	{
		JSONClass bindings = new JSONClass();
		KeyCode[] keys = Globals.instance.keys;
		Command[] actions = Globals.instance.actions;
		for (int i = 0; i < keys.Length; i++)
		{
			bindings.Add(keys[i].ToString().ToLower(), new JSONData(actions[i].ToString()));
		}
		return bindings.ToString();
	}

	/// <summary>
	/// Binds a key to active an InputCommand and saves it to PlayerPrefs.
	/// Note that it will not save to PlayerPrefs if the build is a debug build.
	/// </summary>
	/// <param name="a">The command to bind.</param>
	/// <param name="k">The key to bind the command to.</param>
	public static void BindKey(InputCommand a, KeyCode k)
	{
		Console.Log("Binding " + k + " to " + a);
		a.keyPress = k;
		if (keysPressed.ContainsKey(k))
		{
			keysPressed[k] = a;
		}
		else
		{
			keysPressed.Add(k, a);
		}
		JSONClass bindings = new JSONClass();
		foreach (KeyValuePair<KeyCode, InputCommand> kvp in keysPressed)
		{
			bindings.Add(kvp.Key.ToString(), new JSONData(kvp.Value.ToString()));
		}
		if (!Debug.isDebugBuild)
		{
			PlayerPrefs.SetString("bindings", bindings.ToString());
			PlayerPrefs.Save();
		}
	}

	public static void CheckKeysDown(MonoBehaviour actor)
	{
		foreach (KeyValuePair<KeyCode, InputCommand> kvp in keysPressed)
		{
			KeyCode key = kvp.Key;
			InputCommand value = kvp.Value;
			if (Input.GetKeyDown(key))
			{
				value.ExecuteButtonDown(actor);
			}
			else if (Input.GetKeyUp(key))
			{
				value.ExecuteButtonUp(actor);
			}
			if (Input.GetKey(key))
			{
				value.Execute(actor);
			}
		}
	}
}