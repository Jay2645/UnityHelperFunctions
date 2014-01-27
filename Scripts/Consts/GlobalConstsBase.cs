public static partial class GlobalConsts
{
	/// <summary>
	/// How long it takes to go in and out of a scene (plus loading times)
	/// </summary>
	public const float SCENE_TRANSITION_TIME = 2.0f;
	/// <summary>
	/// How much spacing between each response.
	/// </summary>
	public const float RESPONSE_SPACING = 0.25f;
	public const float PLAYER_SPAWN_DEPRESSION = 1.8f;

	#region Loading
	public const string GAME_BASE_MOD_NAME = "Syrup Studios";
	public static string GetDataPath()
	{
		return UnityEngine.Application.dataPath;
	}
	#endregion

	#region Localized IDs
	// Collected here in case any IDs change.
	/// <summary>
	/// Tells the localization system to skip this file (i.e. use the supplied string rather than any localized variant).
	/// </summary>
	public const string SKIP_LOCALIZATION = "";

	/// <summary>
	/// "Not currently playing a track."
	/// </summary>
	public const string ID_NOT_PLAYING_TRACK = "NOT_PLAYING_TRACK";
	/// <summary>
	/// "Playing track %s, %d seconds long."
	/// </summary>
	public const string ID_CURRENTLY_PLAYING_TRACK = "CURRENTLY_PLAYING_TRACK";

	/// <summary>
	/// "Press any key."
	/// </summary>
	public const string ID_PRESS_ANY_KEY = "PRESS_ANY_KEY";
	/// <summary>
	/// "Saved screenshot as %s"
	/// </summary>
	public const string ID_SCREENSHOT = "SAVED_SCREENSHOT";
	/// <summary>
	/// "Loading savegame %s."
	/// </summary>
	public const string ID_LOADING_GAME = "LOADING_GAME";
	/// <summary>
	/// "Loading level %s."
	/// </summary>
	public const string ID_LOADING_LEVEL = "LOADING_LEVEL";
	/// <summary>
	/// "Okay"
	/// </summary>
	public const string ID_OKAY = "OK";

	/// <summary>
	/// "%s: %d"
	/// </summary>
	public const string ID_COUNTRY_DEBUG_VALUES = "COUNTRY_DEBUG_VALUES";
	/// <summary>
	/// "Saved Games"
	/// </summary>
	public const string ID_SAVED_GAME_FOLDER_NAME = "SAVED_GAME_FOLDER_NAME";
	/// <summary>
	/// "Successfully saved game as %s."
	/// </summary>
	public const string ID_SAVED_GAME_DEBUG = "SAVED_GAME_DEBUG";

	/// <summary>
	/// "Executed %s."
	/// </summary>
	public const string ID_EXECUTED_LUA = "EXECUTED_LUA";
	/// <summary>
	/// "LuaException in %s!\n%s"
	/// </summary>
	public const string ID_ERROR_LUA_EXCEPTION = "LUA_EXCEPTION";
	/// <summary>
	/// "Exception: %s"
	/// </summary>
	public const string ID_ERROR_GENERIC_EXCEPTION = "GENERIC_EXCEPTION";
	/// <summary>
	/// "Cannot load %s as it is not an Lua file."
	/// </summary>
	public const string ID_ERROR_NOT_LUA_FILE = "NON_LUA_FILE";
	/// <summary>
	/// "Could not load %s!\n%s"
	/// </summary>
	public const string ID_ERROR_LOADING_FAILED = "LOADING_FAILED";
	/// <summary>
	/// "Could not find %s in %s!"
	/// </summary>
	public const string ID_ERROR_COULD_NOT_FIND_IN_MOD = "COULD_NOT_FIND_IN_MOD";
	#endregion
}
