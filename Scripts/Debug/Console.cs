using UnityEngine;

public class Console
{
	public static void Log(string message)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(message);
		}
	}

	public static void Log(string message, Object context)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(message, context);
		}
	}

	public static void Log(LocalizedString message, bool showPlayer)
	{
		if (Debug.isDebugBuild || showPlayer)
		{
			DebugConsole.Log(message);
			Debug.Log(message);
		}
	}

	public static void Log(LocalizedString message, Object context, bool showPlayer)
	{
		if (Debug.isDebugBuild || showPlayer)
		{
			Debug.Log(message, context);
		}
	}

	public static void LogWarning(string message)
	{
		if (Debug.isDebugBuild)
		{
			Debug.LogWarning(message);
		}
	}

	public static void LogWarning(string message, Object context)
	{
		if (Debug.isDebugBuild)
		{
			Debug.LogWarning(message, context);
		}
	}

	public static void LogWarning(LocalizedString message, bool showPlayer)
	{
		if (Debug.isDebugBuild || showPlayer)
		{
			Debug.LogWarning(message);
			DebugConsole.LogWarning(message);
		}
	}

	public static void LogWarning(LocalizedString message, Object context, bool showPlayer)
	{
		if (Debug.isDebugBuild || showPlayer)
		{
			Debug.LogWarning(message, context);
		}
	}

	public static void LogError(LocalizedString message)
	{
		Debug.LogError(message);
		DebugConsole.LogError(message);
		DebugConsole.IsOpen = true;
	}

	public static void LogError(LocalizedString message, Object context)
	{
		Debug.LogError(message, context);
		DebugConsole.IsOpen = true;
	}

	public static void AddDebugCommands()
	{
		DebugConsole.RegisterCommand("Lua", LuaRunner);
	}

	#region Debug Commands
	public static object LuaRunner(params string[] args)
	{
		string realString = args[0];
		for (int i = 1; i < args.Length; i++)
		{
			realString += " " + args[i];
		}
		LuaSystem.LuaManager.DoString(realString);
		return "Done.";
	}
	#endregion
}
