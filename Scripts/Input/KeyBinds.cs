using JSONSystem;
using System.Collections.Generic;
using UnityEngine;

public enum InputAction
{
	Left,
	Right,
	Jump,
	Console,
	Pause
}

public static class KeyBinds
{
	private static Dictionary<KeyCode, InputAction> keysPressed;
	private static Dictionary<InputAction, bool> actionsToPerform;
	private static Dictionary<InputAction, bool> actionsPressedDown;

	private static void InitializeDictionaries()
	{
		string rawJSON = PlayerPrefs.GetString("bindings", "");
		if (rawJSON == "")
		{
			rawJSON = GetDefaultBindings();
		}
		JSONClass bindings = JSONSystem.JSON.Parse(rawJSON).AsObject;
		keysPressed = new Dictionary<KeyCode, InputAction>();
		actionsToPerform = new Dictionary<InputAction, bool>();
		actionsPressedDown = new Dictionary<InputAction, bool>();
		for (int i = 0; i < bindings.Count; i++)
		{
			string keyStr = bindings.GetKey(i);
			string actionStr = bindings[i];
			KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyStr, true);
			InputAction action = (InputAction)System.Enum.Parse(typeof(InputAction), actionStr, true);
			keysPressed.Add(key, action);
			if (!actionsToPerform.ContainsKey(action))
			{
				actionsToPerform.Add(action, false);
				actionsPressedDown.Add(action, false);
			}
		}
	}

	private static string GetDefaultBindings()
	{
		JSONClass bindings = new JSONClass();
		bindings.Add("d", new JSONData("Right"));
		bindings.Add("a", new JSONData("Left"));
		bindings.Add("rightarrow", new JSONData("Right"));
		bindings.Add("leftarrow", new JSONData("Left"));
		bindings.Add("uparrow", new JSONData("Jump"));
		bindings.Add("space", new JSONData("Jump"));
		bindings.Add("backquote", new JSONData("Console"));
		bindings.Add("escape", new JSONData("Pause"));
		return bindings.ToString();
	}

	public static void BindKey(InputAction a, KeyCode k)
	{
		if (keysPressed == null)
		{
			InitializeDictionaries();
		}
		if (keysPressed.ContainsKey(k))
		{
			keysPressed[k] = a;
		}
		else
		{
			keysPressed.Add(k, a);
		}
		JSONClass bindings = new JSONClass();
		foreach (KeyValuePair<KeyCode, InputAction> kvp in keysPressed)
		{
			bindings.Add(kvp.Key.ToString(), new JSONData(kvp.Value.ToString()));
		}
		PlayerPrefs.SetString("bindings", bindings.ToString());
		PlayerPrefs.Save();
	}

	public static void CheckKeysDown()
	{
		if (keysPressed == null)
		{
			InitializeDictionaries();
		}
		Dictionary<InputAction, bool> newActions = actionsToPerform;
		Dictionary<InputAction, bool> newActionsDown = actionsPressedDown;
		foreach (KeyValuePair<KeyCode, InputAction> kvp in keysPressed)
		{
			KeyCode key = kvp.Key;
			if (Input.GetKey(key))
			{
				newActions[kvp.Value] = true;
			}
			if (Input.GetKeyDown(key))
			{
				newActionsDown[kvp.Value] = true;
			}
		}
		actionsToPerform = newActions;
		actionsPressedDown = newActionsDown;
	}

	public static bool GetAction(InputAction action)
	{
		if (actionsToPerform == null)
		{
			InitializeDictionaries();
		}
		return actionsToPerform[action];
	}

	public static bool GetActionDown(InputAction action)
	{
		if (actionsPressedDown == null)
		{
			InitializeDictionaries();
		}
		return actionsPressedDown[action];
	}

	public static void ResetKeyPresses()
	{
		if (keysPressed == null)
		{
			InitializeDictionaries();
		}
		Dictionary<InputAction, bool> newActions = new Dictionary<InputAction, bool>();
		Dictionary<InputAction, bool> newActionsDown = new Dictionary<InputAction, bool>();
		foreach (KeyValuePair<InputAction, bool> kvp in actionsToPerform)
		{
			newActions.Add(kvp.Key, false);
			newActionsDown.Add(kvp.Key, false);
		}
		actionsToPerform = newActions;
		actionsPressedDown = newActionsDown;
	}
}