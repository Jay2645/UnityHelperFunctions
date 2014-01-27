using SaveSystem;
using System.Collections.Generic;
using UnityEngine;
namespace Hashing
{
	public class MonoHash
	{
		public MonoHash(MonoHelper bHelper)
		{
			helper = bHelper;
			GenerateMonoID();
		}

		private MonoHelper helper = null;
		private static Dictionary<string, MonoHelper> hashLookup = new Dictionary<string, MonoHelper>();
		private float posHash = -1.0f;
		public string fullPosHash = "";
		public string monoID
		{
			get
			{
				return GenerateMonoID();
			}
			set
			{
				GenerateMonoID();
			}
		}

		private string GenerateMonoID()
		{
			if (fullPosHash != "" || this == null || helper.tag == "DontSave")
			{
				return fullPosHash;
			}
			if (posHash == -1)
			{
				posHash = (1000.0f * helper.transform.position.x) + helper.transform.position.y + (0.001f * helper.transform.position.z);
			}
			fullPosHash = posHash.ToString() + GetType().Name + helper.name + Application.loadedLevelName;
			fullPosHash = fullPosHash.Replace(" ", "");
			AddToHashLookup(this, helper);
			return fullPosHash;
		}

		public static void AddToHashLookup(MonoHash hash, MonoHelper helper)
		{
			string fullPosHash = hash.fullPosHash;
			if (hashLookup.ContainsKey(fullPosHash))
			{
				MonoHelper oldHash = hashLookup[fullPosHash];
				if (oldHash != null && oldHash != helper)
				{
					if (!oldHash.gameObject.activeInHierarchy)
					{
						MonoBehaviour.Destroy(oldHash.gameObject);
						hashLookup[fullPosHash] = helper;
					}
					else if (oldHash.name != helper.name)
					{
						oldHash.SetName(oldHash.name); // This should remove any old key that's already in there
						if (hashLookup.ContainsKey(fullPosHash)) // Should never happen
						{
							if (!SaveGame.IsLoading())
							{
								throw ThrowHashCollision(oldHash, helper);
							}
							MonoBehaviour.Destroy(oldHash.gameObject);
							hashLookup[fullPosHash] = helper;
						}
						else
						{
							hashLookup.Add(fullPosHash, helper);
						}
					}
					else
					{
						if (!SaveGame.IsLoading())
						{
							throw ThrowHashCollision(oldHash, helper);
						}
						MonoBehaviour.Destroy(oldHash.gameObject);
						hashLookup[fullPosHash] = helper;
					}
				}
				else
				{
					hashLookup[fullPosHash] = helper;
				}
			}
			else
			{
				hashLookup.Add(fullPosHash, helper);
			}
			if (!SaveGame.IsLoading())
			{
				Instancing.ObjectLoader.ClearLoadingQueue();
			}
		}

		private static HashCollisionException ThrowHashCollision(MonoHelper first, MonoHelper second)
		{
			string message = "Hash collision detected between " + first + " and " + second + "";
			string oldHash = first.monoID;
			string newHash = second.monoID;
			if (oldHash == newHash)
			{
				message += "! Hash: " + oldHash;
			}
			else
			{
				message += ", but the hashes did not match. First: " + oldHash + ", Second: " + newHash;
			}
			HashCollisionException ex = new HashCollisionException(message);
			ex.hash = newHash;
			return ex;
		}

		public void SetName(string newName)
		{
			hashLookup.Remove(fullPosHash);
			helper.name = newName;
			GenerateMonoID();
		}

		public void CleanUp()
		{
			if (hashLookup.ContainsKey(fullPosHash) && hashLookup[fullPosHash] == helper)
			{
				hashLookup.Remove(fullPosHash);
			}
		}

		public static MonoHelper GetMonoHelper(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MonoHelper helper = null;
			helper = go.GetComponent<MonoHelper>();
			if (helper == null)
			{
				helper = go.AddComponent<MonoHelper>();
			}
			return helper;
		}

		public static MonoHelper GetMonoHelper(string hash)
		{
			if (hashLookup.ContainsKey(hash))
			{
				return hashLookup[hash];
			}
			else
			{
				//Debug.LogError("Cannot find hash " + hash + "!");
				return null;
			}
		}
	}
}