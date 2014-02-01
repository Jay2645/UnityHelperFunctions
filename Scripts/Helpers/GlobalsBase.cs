using HelperFunctions.CharacterStates;
using HelperFunctions.Commands;
using HelperFunctions.Instancing;
using HelperFunctions.LuaSystem;
using HelperFunctions.Motors;
using HelperFunctions.SaveSystem;
using System.Collections.Generic;
using UnityEngine;

namespace HelperFunctions
{
	public partial class Globals : MonoHelper
	{
		partial void Init();

		public static Globals instance
		{
			get
			{
				if (isClosing)
				{
					return null;
				}
				if (_instance == null)
				{
					// This finds a Globals object in our scene.
					// If it cannot find one, it initalizes one for us and returns it.
					GameObject globalGO = GameObject.FindGameObjectWithTag("Globals");
					if (globalGO == null)
					{
						globalGO = new GameObject("Globals");
						globalGO.tag = "Globals";
					}
					_instance = globalGO.GetComponentInChildren<Globals>();
					if (_instance == null)
					{
						_instance = globalGO.AddComponent<Globals>();
					}
					_instance.Init();
				}
				return _instance;
			}
		}
		private static Globals _instance = null;

		protected static bool hasInit = false;
		private static bool isClosing = false;

		/// <summary>
		/// Returns TRUE if we can access Steam currently. Otherwise returns FALSE.
		/// </summary>
		public static bool steamEnabled
		{
			get
			{
				return CheckSteam();
			}
		}

		/// <summary>
		/// Replace this code with your hook into the Steamworks API.
		/// </summary>
		/// <returns>TRUE if we can successfully access Steam. Otherwise FALSE.</returns>
		private static bool CheckSteam()
		{
			return false;
		}

		#region Init
		protected void BaseInit()
		{
			DontDestroy(this.gameObject);
			inputController = gameObject.GetComponent<InputController>();
			if (inputController == null)
			{
				inputController = gameObject.AddComponent<InputController>();
			}
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
			GameObject roomGlobals = GameObject.FindGameObjectWithTag("Room Settings");
			if (roomGlobals != null)
			{
				roomSettings = roomGlobals.GetComponent<RoomSettings>();
			}
		}
		#endregion

		#region General Variables
		/// <summary>
		/// When a level is loaded, we look for any objects with this tag.
		/// If we find one and we have an object to teleport, we teleport the object to the GameObject with this tag.
		/// </summary>
		private static string teleportTag = "";
		/// <summary>
		/// An object that we're going to teleport on level load.
		/// </summary>
		private static GameObject teleportTarget;
		/// <summary>
		/// A list of GameObjects to cleanup when we start a new game.
		/// </summary>
		protected static List<GameObject> cleanUp;
		/// <summary>
		/// The settings specific to this room.
		/// </summary>
		public static RoomSettings roomSettings;
		/// <summary>
		/// The last loaded level.
		/// </summary>
		public static int lastLoadedLevel = 1;
		/// <summary>
		/// The input controller, which handles the processing of player input and is required for any touch-or-click-based events.
		/// </summary>
		public static InputController inputController;
		/// <summary>
		/// Is the game over?
		/// </summary>
		public static bool gameIsOver = false;
		/// <summary>
		/// A list of GameObjects which should not be destroyed.
		/// </summary>
		public static List<GameObject> dontDestroyObjects = new List<GameObject>();
		/// <summary>
		/// The number of frames since startup.
		/// </summary>
		private static int frameCount = 0;
		/// <summary>
		/// Is the game currently paused?
		/// </summary>
		public static bool gameIsPaused = false;
		/// <summary>
		/// This contains a reference to the player.
		/// </summary>
		public static MonoHelper player = null;
		public KeyCode[] keys;
		public InputCommandName[] actions;
		#endregion

		#region Instance Methods
		#region Monobehaviour Methods

		protected void BaseOnLevelLoad()
		{
			if (gameIsOver)
			{
				return;
			}
			CameraFade.StartAlphaFade(Color.black, true, true, GlobalConsts.SCENE_TRANSITION_TIME);
			if (teleportTag == "" || teleportTarget == null)
				return;
			GameObject teleportGO = GameObject.FindGameObjectWithTag(teleportTag);
			GameObject[] targetGOs = GameObject.FindGameObjectsWithTag(teleportTarget.tag);
			if (targetGOs != null)
			{
				if (targetGOs.Length == 1)
				{
					GameObject go = targetGOs[0];
					if (go != teleportTarget)
					{
						Destroy(teleportTarget);
						teleportTarget = go;
					}
				}
				else if (targetGOs.Length > 0)
				{
					foreach (GameObject go in targetGOs)
					{
						if (go.transform.position != teleportTarget.transform.position)
						{
							Destroy(go);
						}
					}
				}
			}
			if (teleportGO == null)
				return;
			float z = teleportTarget.transform.position.z;
			Vector3 position = teleportGO.transform.position;
			position.z = z;
			position.y -= GlobalConsts.PLAYER_SPAWN_DEPRESSION;
			if (Physics2D.Raycast(position + Vector3.left, Vector3.down))
			{
				position = position + Vector3.left;
			}
			else if (Physics2D.Raycast(position + Vector3.right, Vector3.down))
			{
				position = position + Vector3.right;
			}
			teleportTarget.transform.position = position;
			roomSettings = null;
			GameObject roomGlobals = GameObject.FindGameObjectWithTag("Room Settings");
			if (roomGlobals != null)
			{
				roomSettings = roomGlobals.GetComponent<RoomSettings>();
			}
			AutoSave();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F12))
			{
				string screenshotName = string.Format("screenshot-{0:yyyy-MM-dd_hh-mm-ss-tt}.png", System.DateTime.Now);
				Application.CaptureScreenshot(screenshotName, 4);
				//Console.Log(new LocalizedString(GlobalConsts.ID_SCREENSHOT, "Saved screenshot as %s", new object[] { screenshotName }), true);
			}
			if (frameCount > 2)
			{
				return;
			}
			frameCount++;
			if (frameCount == 2) // This ensures that all possible bindings have been loaded.
			{
				LuaManager.DoAllFiles();
			}
		}

		void OnApplicationQuit()
		{
			isClosing = true;
		}
		#endregion
		#endregion

		#region Static Methods
		#region Loading Methods
		public static void AutoSave()
		{
			SaveGame.AutoSave();
		}

		public static void LoadLastLevel()
		{
			LoadLevel(lastLoadedLevel);
		}

		public static void LoadLastLevel(GameObject teleportObject, string teleportTo)
		{
			LoadLevel(lastLoadedLevel, teleportObject, teleportTo);
		}

		public static void LoadLevel(int id)
		{
			InstanceManager.SaveRoom();
			lastLoadedLevel = Application.loadedLevel;
			CameraFade.StartAlphaFade(Color.black, false, true, GlobalConsts.SCENE_TRANSITION_TIME, 0.0f, () =>
			{
				Application.LoadLevel(id);
			});
		}

		public static void LoadLevel(string name)
		{
			InstanceManager.SaveRoom();
			lastLoadedLevel = Application.loadedLevel;
			CameraFade.StartAlphaFade(Color.black, false, false, GlobalConsts.SCENE_TRANSITION_TIME, 0.0f, () =>
			{
				Application.LoadLevel(name);
			});
		}

		public static void LoadLevel(int id, GameObject teleportObject, string teleportTo)
		{
			ProtectFromLoad(teleportObject, teleportTo);
			LoadLevel(id);
		}

		public static void LoadLevel(string name, GameObject teleportObject, string teleportTo)
		{
			ProtectFromLoad(teleportObject, teleportTo);
			LoadLevel(name);
		}

		private static void ProtectFromLoad(GameObject teleportObject, string teleportTo)
		{
			teleportTag = teleportTo;
			DontDestroy(teleportObject);
			foreach (Transform child in teleportObject.transform)
			{
				DontDestroy(child.gameObject);
			}
			teleportTarget = teleportObject;
		}
		#endregion

		#region Game Over Methods
		public void CleanUp()
		{
			if (cleanUp == null)
				return;
			GameObject[] cleanUpArray = cleanUp.ToArray();
			foreach (GameObject go in cleanUpArray)
			{
				Destroy(go);
			}
			cleanUp = null;
		}

		public static void DontDestroy(GameObject go)
		{
			DontDestroyOnLoad(go);
			if (!dontDestroyObjects.Contains(go))
			{
				dontDestroyObjects.Add(go);
			}
		}

		public static void CleanUpObjects()
		{
			GameObject[] cleanUp = dontDestroyObjects.ToArray();
			for (int i = 0; i < cleanUp.Length; i++)
			{
				Destroy(cleanUp[i]);
			}
			dontDestroyObjects.Clear();
			if (instance != null)
			{
				Destroy(instance.gameObject);
			}
			gameIsOver = false;
		}
		#endregion

		#region State Machines
		public static bool SetPlayerVerticalState(VerticalState nextState)
		{
			if (player == null)
			{
				return false;
			}
			CharacterMotor motor = player.GetComponentInChildren<CharacterMotor>();
			if (motor == null)
			{
				return false;
			}
			return motor.ChangeState(nextState);
		}
		public static bool SetPlayerHorizontalState(HorizontalState nextState)
		{
			if (player == null)
			{
				return false;
			}
			CharacterMotor motor = player.GetComponentInChildren<CharacterMotor>();
			if (motor == null)
			{
				return false;
			}
			return motor.ChangeState(nextState);
		}
		public static bool SetPlayerEquipmentState(EquipmentState nextState)
		{
			if (player == null)
			{
				return false;
			}
			CharacterMotor motor = player.GetComponentInChildren<CharacterMotor>();
			if (motor == null)
			{
				return false;
			}
			return motor.ChangeState(nextState);
		}

		public static VerticalState GetPlayerVerticalState()
		{
			if (player == null)
			{
				return null;
			}
			CharacterMotor motor = player.GetComponentInChildren<CharacterMotor>();
			if (motor == null)
			{
				return null;
			}
			return motor.currentVerticalState;
		}
		public static HorizontalState GetPlayerHorizontalState()
		{
			if (player == null)
			{
				return null;
			}
			CharacterMotor motor = player.GetComponentInChildren<CharacterMotor>();
			if (motor == null)
			{
				return null;
			}
			return motor.currentHorizontalState;
		}
		public static EquipmentState GetPlayerEquipmentState()
		{
			if (player == null)
			{
				return null;
			}
			CharacterMotor motor = player.GetComponentInChildren<CharacterMotor>();
			if (motor == null)
			{
				return null;
			}
			return motor.currentEquipmentState;
		}
		#endregion

		public static bool GameIsPaused()
		{
			return gameIsPaused;
		}

		public static void OpenPauseMenu()
		{
			// TODO
		}

		public static void ClosePauseMenu()
		{
			// TODO
		}
		#endregion
	}
}