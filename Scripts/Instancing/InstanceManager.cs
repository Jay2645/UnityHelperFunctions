using JSONSystem;
using SaveSystem;
using System.Collections.Generic;
using UnityEngine;
namespace Instancing
{
	public class InstanceManager : MonoBehaviour
	{
		private static InstanceManager instance = null;
		private static bool hasLoadedLevel = false;
		private static Dictionary<string, Object> loadedResources = new Dictionary<string, Object>();
		/// <summary>
		/// A list of all current GameObjects which have been instantiated in this room and the path used to load them.
		/// </summary>
		private static List<KeyValuePair<string, GameObject>> loadedGOs = new List<KeyValuePair<string, GameObject>>();
		private static Dictionary<string, List<CachedPrefab>> roomGOs = new Dictionary<string, List<CachedPrefab>>();

		public static Object Load(string path)
		{
			if (loadedResources.ContainsKey(path) && loadedResources[path] != null)
			{
				return InstantiateObject(path, loadedResources[path]);
			}
			// Not loaded yet
			Object o = Resources.Load(path);
			if (o == null)
			{
				Console.LogWarning("Object cannot be found at " + path + "!");
				return null;
			}
			loadedResources.Add(path, o);
			return InstantiateObject(path, o);
		}

		private static Object InstantiateObject(string path, Object source)
		{
			Object instantiated = Instantiate(source);
			CheckInstance();
			if (instantiated is GameObject || instantiated is Component)
			{
				GameObject instGO = null;
				if (instantiated is GameObject)
				{
					instGO = (GameObject)instantiated;
					KeyValuePair<string, GameObject> kvp = new KeyValuePair<string, GameObject>(path, instGO);
					loadedGOs.Add(kvp);

				}
				else
				{
					Component c = (Component)instantiated;
					instGO = c.gameObject;
					KeyValuePair<string, GameObject> kvp = new KeyValuePair<string, GameObject>(path, c.gameObject);
					loadedGOs.Add(kvp);
				}
				MonoHelper[] helpers = instGO.transform.root.GetComponentsInChildren<MonoHelper>();
				foreach (MonoHelper helper in helpers)
				{
					helper.SetName(helper.name); // This will generate a new hash.
				}
			}
			CacheGOs(false);
			return instantiated;
		}

		private static void CheckInstance()
		{
			if (instance == null)
			{
				instance = Globals.instance.gameObject.GetComponentInChildren<InstanceManager>();
				if (instance == null)
				{
					instance = Globals.instance.gameObject.AddComponent<InstanceManager>();
				}
			}
		}

		public static void SaveRoom()
		{
			CacheGOs(false);
		}

		public static void CacheGOs(bool includePlayer)
		{
			List<CachedPrefab> loadedRoomGOs = new List<CachedPrefab>();
			foreach (KeyValuePair<string, GameObject> kvp in loadedGOs)
			{
				CachedPrefab cachedGO = new CachedPrefab();
				cachedGO.source = kvp.Key;
				GameObject go = kvp.Value;
				if (go == null || go.transform.parent != null || go.tag == "DontSave" || !includePlayer && go.tag == "Player")
				{
					continue;
				}
				MonoHelper goHelper = MonoHelper.GetMonoHelper(go);
				cachedGO.hash = goHelper.monoID;
				cachedGO.position = go.transform.position;
				cachedGO.scale = go.transform.localScale;
				cachedGO.rotation = go.transform.rotation.eulerAngles;
				JSONClass components = new JSONClass();
				MonoHelper[] helpers = go.GetComponentsInChildren<MonoHelper>();
				foreach (MonoHelper helper in helpers)
				{
					JSONClass helperJSON = helper.ToJSON();
					if (helperJSON == null)
					{
						continue;
					}
					string helperName = helper.GetType().Name;
					components[helperName] = helperJSON;
				}
				cachedGO.components = components;
				loadedRoomGOs.Add(cachedGO);
			}
			string levelName = Application.loadedLevelName;
			if (roomGOs.ContainsKey(levelName))
			{
				roomGOs[levelName] = loadedRoomGOs;
			}
			else
			{
				roomGOs.Add(Application.loadedLevelName, loadedRoomGOs);
			}
		}

		public static JSONClass ToJSON()
		{
			CacheGOs(true);
			JSONClass roomData = new JSONClass();
			JSONClass cachedRooms = new JSONClass();
			foreach (KeyValuePair<string, List<CachedPrefab>> kvp in roomGOs)
			{
				JSONArray roomObjects = new JSONArray();
				List<CachedPrefab> cachedGOs = kvp.Value;
				foreach (CachedPrefab cachedGO in cachedGOs)
				{
					JSONClass jsonObject = new JSONClass();
					jsonObject.Add("source", new JSONData(cachedGO.source));
					jsonObject.Add("hash", new JSONData(cachedGO.hash));
					jsonObject.Add("position", SaveGame.ConvertVector3(cachedGO.position));
					jsonObject.Add("scale", SaveGame.ConvertVector3(cachedGO.scale));
					jsonObject.Add("rotation", SaveGame.ConvertVector3(cachedGO.rotation));
					jsonObject.Add("components", cachedGO.components);
					roomObjects.Add(jsonObject);
				}
				cachedRooms.Add(kvp.Key, roomObjects);
			}
			roomData.Add("roomdata", cachedRooms);
			return roomData;
		}

		public static void FromJSON(JSONClass json)
		{
			FromJSON(json, Application.loadedLevelName);
		}

		public static void FromJSON(JSONClass json, string roomName)
		{
			roomGOs.Clear();
			json = json["roomdata"].AsObject;
			for (int i = 0; i < json.Count; i++)
			{
				string thisRoom = json.GetKey(i);
				JSONArray roomObjects = json[i].AsArray;
				List<CachedPrefab> cachedRoomGOs = new List<CachedPrefab>();
				for (int j = 0; j < roomObjects.Count; j++)
				{
					JSONClass roomObject = roomObjects[j].AsObject;
					CachedPrefab cachedGO = new CachedPrefab();
					cachedGO.source = roomObject["source"];
					cachedGO.hash = roomObject["hash"];
					cachedGO.position = SaveGame.ConvertVector3(roomObject["position"].AsObject);
					cachedGO.scale = SaveGame.ConvertVector3(roomObject["scale"].AsObject);
					cachedGO.rotation = SaveGame.ConvertVector3(roomObject["rotation"].AsObject);
					cachedGO.components = roomObject["components"].AsObject;
					cachedRoomGOs.Add(cachedGO);
				}
				roomGOs.Add(thisRoom, cachedRoomGOs);
			}
			ReloadLevel(roomName);
		}

		public static void LoadFromCache(string levelName)
		{
			hasLoadedLevel = true;
			if (!roomGOs.ContainsKey(levelName))
			{
				return;
			}
			List<CachedPrefab> cacheList = roomGOs[levelName];
			foreach (CachedPrefab cachedGO in cacheList)
			{
				MonoHelper monoGO = LoadFromCache(cachedGO);
				Globals.DontDestroy(monoGO.gameObject);
				MonoHelper.AddToLoadingQueue(monoGO);
			}
		}

		private static MonoHelper LoadFromCache(CachedPrefab cachedGO)
		{
			GameObject go = Load(cachedGO.source) as GameObject;
			MonoHelper goMonoHelper = MonoHelper.GetMonoHelper(go);
			goMonoHelper.LoadFromCache(cachedGO);
			return goMonoHelper;
		}

		private static void ReloadLevel()
		{
			ReloadLevel(Application.loadedLevelName);
		}

		private static void ReloadLevel(string levelName)
		{
			if (hasLoadedLevel)
			{
				SaveGame.IsLoading(false);
				hasLoadedLevel = false;
				AudioSystem.AudioManager.globalVolume = 1.0f;
				return;
			}
			foreach (KeyValuePair<string, GameObject> kvp in loadedGOs)
			{
				if (kvp.Value == null)
				{
					continue;
				}
				kvp.Value.SetActive(false);
			}
			if (Globals.player != null)
			{
				Destroy(Globals.player.gameObject);
			}
			Console.Log(new LocalizedString(GlobalConsts.ID_LOADING_LEVEL, "Loading level %s.", new object[] { levelName }), true);
			loadedGOs.Clear();
			LoadFromCache(levelName);
			CheckInstance();
			Application.LoadLevel(levelName);
		}

		void OnLevelWasLoaded(int level)
		{
			ReloadLevel();
		}
	}
}