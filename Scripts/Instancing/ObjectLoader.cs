using System.Collections.Generic;
using UnityEngine;
namespace HelperFunctions
{
	namespace Instancing
	{
		public class ObjectLoader
		{
			protected static Dictionary<string, MonoHelper> loadingQueue = new Dictionary<string, MonoHelper>();
			private static List<GameObject> manuallyCleanup = new List<GameObject>();

			public static Object Load(Vector3 location, string path)
			{
				return Load(path, location, Globals.instance);
			}

			public static Object Load(Transform parent, Vector3 localPosition, string path)
			{
				MonoHelper instance = parent.root.GetComponentInChildren<MonoHelper>();
				if (instance == null)
				{
					instance = Globals.instance;
				}
				return Load(path, parent, localPosition, instance);
			}

			public static Object Load(string path, Vector3 location, MonoHelper instance)
			{
				return instance.Load(path, location);
			}

			public static Object Load(string path, Transform parent, Vector3 localPosition, MonoHelper instance)
			{
				return instance.Load(path, parent, localPosition);
			}

			public Object Load(string path, Transform parent, Vector3 localPosition)
			{
				return Load(path, parent, localPosition, "");
			}

			public Object Load(string path, Transform parent, Vector3 localPosition, string newName)
			{
				if (newName == "")
				{
					newName = path.Substring(path.LastIndexOf("/") + 1);
					newName += "(Clone from " + parent.name + ")";
				}
				Object o = Load(path, parent.position + localPosition, newName);
				if (o is MonoBehaviour)
				{
					((MonoBehaviour)o).transform.parent = parent;
				}
				else if (o is GameObject)
				{
					((GameObject)o).transform.parent = parent;
				}
				return o;
			}

			public Object Load(string path, Vector3 location)
			{
				return Load(path, location, "");
			}

			public Object Load(string path, Vector3 location, string newName)
			{
				MonoHelper mono = null;
				Object o = null;
				try
				{
					o = InstanceManager.Load(path);
					if (o is MonoHelper)
					{
						mono = (MonoHelper)o;
					}
					else if (o is GameObject)
					{
						mono = MonoHelper.GetMonoHelper((GameObject)o);
					}
					if (mono != null)
					{
						// Move the MonoHelper to its final position and name in order to generate an accurate hash
						mono.transform.position = location;
						if (newName != "")
						{
							mono.SetName(newName);
						}
						if (SaveSystem.SaveGame.IsLoading())
						{
							// We need to generate the hash to check if we should replace it with something that's being loaded
							string oID = mono.monoID;
							if (loadingQueue.ContainsKey(oID)) // Use the object we need to load instead
							{
								mono = loadingQueue[oID];
								if (o is GameObject)
								{
									// Destroy the physical representation of the object in the game world
									MonoBehaviour.Destroy(o);
									o = mono.gameObject;
								}
								else
								{
									MonoBehaviour.Destroy(((MonoHelper)o).gameObject);
									o = mono;
								}
								manuallyCleanup.Add(mono.gameObject);
								loadingQueue.Remove(oID);
							}
						}
					}
					return o;
				}
				catch (Hashing.HashCollisionException ex)
				{
					if (mono != null)
					{
						MonoBehaviour.Destroy(mono.gameObject);
					}
					Console.LogWarning("Loading " + path + " from hash.");
					return MonoHelper.GetMonoHelper(ex.hash);
				}
			}

			public static void AddToLoadingQueue(MonoHelper helper)
			{
				string hash = helper.monoID;
				if (loadingQueue.ContainsKey(hash))
				{
					Console.LogWarning("Attempting to load a duplicate hash! " + hash + " in MonoHelper " + helper + " is a duplicate of " + loadingQueue[hash] + "!");
				}
				else
				{
					loadingQueue.Add(hash, helper);
				}
			}

			public static void ClearLoadingQueue()
			{
				loadingQueue.Clear();
			}

			public static void DontDestroy(GameObject gameObject)
			{
				if (manuallyCleanup.Contains(gameObject))
				{
					return;
				}
				Globals.DontDestroy(gameObject);
				manuallyCleanup.Add(gameObject);
			}

			public void CleanUp()
			{
				foreach (GameObject go in manuallyCleanup)
				{
					MonoBehaviour.Destroy(go);
				}
			}
		}
	}
}