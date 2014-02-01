using HelperFunctions.Hashing;
using HelperFunctions.SaveSystem;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HelperFunctions
{
	namespace JSONSystem
	{
		public class MonoJSON
		{
			public MonoJSON(MonoHelper mHelper)
			{
				helper = mHelper;
			}
			private MonoHelper helper = null;

			public void LoadFromCache(Instancing.CachedPrefab cachedGO)
			{
				MonoHash hash = helper.GetHash();
				hash.fullPosHash = cachedGO.hash;
				MonoHash.AddToHashLookup(hash, helper);
				helper.transform.position = cachedGO.position;
				helper.transform.localScale = cachedGO.scale;
				helper.transform.rotation = Quaternion.Euler(cachedGO.rotation);
				JSONClass components = cachedGO.components;
				for (int i = 0; i < components.Count; i++)
				{
					System.Type key = System.Type.GetType(components.GetKey(i));
					if (key == null)
					{
						Console.LogWarning("Could not find key " + components.GetKey(i) + " in " + helper.name + ".");
						continue;
					}
					MonoHelper component = helper.GetComponentInChildren(key) as MonoHelper;
					if (component == null)
					{
						Console.LogWarning("Could not find component " + components.GetKey(i) + " in " + helper.name + ".");
						continue;
					}
					component.FromJSON(components[i].AsObject);
				}
			}

			public virtual JSONClass ToJSON()
			{
				// Checks to see if this is a base MonoHelper class
				if (!helper.GetType().IsSubclassOf(typeof(MonoHelper)))
				{
					return null;
				}
				return ToJSON(helper);
			}

			public static JSONClass ToJSON(MonoHelper helper)
			{
				JSONClass componentData = new JSONClass();
				componentData.Add("hash", new JSONData(helper.monoID));
				componentData.Add("fields", GetFields(helper));
				return componentData;
			}

			public virtual void FromJSON(JSONClass json)
			{
				MonoHash hash = helper.GetHash();
				hash.fullPosHash = json["hash"];
				System.Type t = GetType();
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
										BindingFlags.Static | BindingFlags.Instance |
										BindingFlags.FlattenHierarchy;
				FieldInfo[] fields = t.GetFields(flags);
				FieldInfo[] badFieldArray = typeof(MonoHelper).GetFields(flags);
				List<string> badFields = new List<string>();
				foreach (FieldInfo field in badFieldArray)
				{
					badFields.Add(field.Name);
				}
				JSONClass fieldJSON = json["fields"].AsObject;
				foreach (FieldInfo field in fields)
				{
					string fieldName = field.Name;
					if (badFields.Contains(fieldName) || System.Attribute.IsDefined(field, typeof(DontSave)))
					{
						continue;
					}
					SaveGame.LoadField(fieldJSON[fieldName].AsObject, field, helper);
				}
			}

			public static JSONClass GetFields(MonoHelper source)
			{
				return GetFields(source, new JSONClass());
			}

			public static JSONClass GetFields(MonoHelper source, JSONClass fieldArray)
			{
				System.Type t = source.GetType();
				BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
										BindingFlags.Static | BindingFlags.Instance |
										BindingFlags.FlattenHierarchy;
				FieldInfo[] fields = t.GetFields(flags);
				FieldInfo[] badFieldArray = typeof(MonoHelper).GetFields(flags);
				List<string> badFields = new List<string>();
				foreach (FieldInfo field in badFieldArray)
				{
					badFields.Add(field.Name);
				}
				foreach (FieldInfo field in fields)
				{
					object fieldObj = field.GetValue(source);
					if (fieldObj == null)
					{
						continue;
					}
					string fieldName = field.Name;
					if (badFields.Contains(fieldName) || System.Attribute.IsDefined(field, typeof(DontSave)))
					{
						continue;
					}
					JSONClass fieldData = SaveGame.SaveField(fieldObj, fieldName);
					if (fieldData == null)
					{
						continue;
					}
					fieldArray.Add(fieldName, fieldData);
				}
				return fieldArray;
			}
		}
	}
}