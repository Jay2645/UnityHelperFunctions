using AudioSystem;
using Instancing;
using JSONSystem;
using LuaSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SaveSystem
{
	public class DontSave : Attribute
	{

	}

	public class SaveGameException : Exception
	{
		public SaveGameException()
		{
		}

		public SaveGameException(string message)
			: base(message)
		{
		}

		public SaveGameException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	public static class SaveGame
	{
		public const string OBJECT_ID_JSON_STRING = "hash";
		public const string COMPONENT_NAME_JSON_STRING = "componentname";
		public static int autosaveCount = 3;
		public static bool saving = false;
		private static bool loading = false;
		private static Camera loadingCamera = null;

		#region Convert Vectors
		public static JSONClass ConvertVector2(Vector2 convert)
		{
			JSONClass vector = new JSONClass();
			vector.Add("x", new JSONData(convert.x));
			vector.Add("y", new JSONData(convert.y));
			return vector;
		}

		public static Vector2 ConvertVector2(JSONClass convert)
		{
			Vector2 vector = Vector2.zero;
			vector.x = convert["x"].AsFloat;
			vector.y = convert["y"].AsFloat;
			return vector;
		}

		public static JSONClass ConvertVector3(Vector3 convert)
		{
			JSONClass vector = new JSONClass();
			vector.Add("x", new JSONData(convert.x));
			vector.Add("y", new JSONData(convert.y));
			vector.Add("z", new JSONData(convert.z));
			return vector;
		}

		public static Vector3 ConvertVector3(JSONClass convert)
		{
			Vector3 vector = Vector3.zero;
			vector.x = convert["x"].AsFloat;
			vector.y = convert["y"].AsFloat;
			vector.z = convert["z"].AsFloat;
			return vector;
		}

		public static JSONClass ConvertVector4(Vector4 convert)
		{
			JSONClass vector = new JSONClass();
			vector.Add("w", new JSONData(convert.w));
			vector.Add("x", new JSONData(convert.x));
			vector.Add("y", new JSONData(convert.y));
			vector.Add("z", new JSONData(convert.z));
			return vector;
		}

		public static Vector4 ConvertVector4(JSONClass convert)
		{
			Vector4 vector = Vector4.zero;
			vector.w = convert["w"].AsFloat;
			vector.x = convert["x"].AsFloat;
			vector.y = convert["y"].AsFloat;
			vector.z = convert["z"].AsFloat;
			return vector;
		}

		public static JSONClass ConvertColor(Color convert)
		{
			JSONClass vector = new JSONClass();
			vector.Add("r", new JSONData(convert.r));
			vector.Add("g", new JSONData(convert.g));
			vector.Add("b", new JSONData(convert.b));
			vector.Add("a", new JSONData(convert.a));
			return vector;
		}

		public static Color ConvertColor(JSONClass convert)
		{
			Color vector = Color.clear;
			vector.r = convert["r"].AsFloat;
			vector.g = convert["g"].AsFloat;
			vector.b = convert["b"].AsFloat;
			vector.a = convert["a"].AsFloat;
			return vector;
		}
		#endregion

		#region Save Fields
		public static JSONClass SaveField(object fieldObj, string fieldName)
		{
			JSONClass fieldData = null;
			if (fieldObj == null || fieldObj is LuaBinding)
			{
				return null;
			}
			else if (fieldObj is GameObject)
			{
				if ((GameObject)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper((GameObject)fieldObj).monoID));
			}
			else if (fieldObj is MonoHelper)
			{
				if ((MonoHelper)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = ((MonoHelper)fieldObj).GetType().Name; // For some reason, doing this in one step screws up serialization.
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(((MonoHelper)fieldObj).monoID));
			}
			else if (fieldObj is MonoBehaviour)
			{
				if ((MonoBehaviour)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = ((MonoBehaviour)fieldObj).GetType().Name;
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper(((MonoBehaviour)fieldObj).gameObject).monoID));
			}
			else if (fieldObj is Camera)
			{
				if ((Camera)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = "Camera";
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper(((Camera)fieldObj).gameObject).monoID));
			}
			else if (fieldObj is TextMesh)
			{
				if ((TextMesh)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = "TextMesh";
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper(((TextMesh)fieldObj).gameObject).monoID));
			}
			else if (fieldObj is string) // String is technically an IEnumerable so we need to check for it first
			{
				fieldData = new JSONClass();
				fieldData.Add(fieldName, new JSONData((string)fieldObj));
			}
			else if (fieldObj is IEnumerable)
			{
				fieldData = SerializeIEnumerable((IEnumerable)fieldObj, fieldName);
			}
			else if (fieldObj is Vector2)
			{
				fieldData = ConvertVector2((Vector2)fieldObj);
			}
			else if (fieldObj is Vector3)
			{
				fieldData = ConvertVector3((Vector3)fieldObj);
			}
			else if (fieldObj is Vector4)
			{
				fieldData = ConvertVector4((Vector4)fieldObj);
			}
			else if (fieldObj is Color)
			{
				fieldData = ConvertColor((Color)fieldObj);
			}
			else if (fieldObj is bool)
			{
				fieldData = new JSONClass();
				fieldData.Add(fieldName, new JSONData((bool)fieldObj));
			}
			else if (fieldObj is int)
			{
				fieldData = new JSONClass();
				fieldData.Add(fieldName, new JSONData((int)fieldObj));
			}
			else if (fieldObj is float)
			{
				fieldData = new JSONClass();
				fieldData.Add(fieldName, new JSONData((float)fieldObj));
			}
			else if (fieldObj is double)
			{
				fieldData = new JSONClass();
				fieldData.Add(fieldName, new JSONData((double)fieldObj));
			}
			else if (fieldObj is Enum)
			{
				fieldData = new JSONClass();
				fieldData.Add(fieldName, SerializeEnum((Enum)fieldObj));
			}
			else if (fieldObj is AudioClip)
			{
				if ((AudioClip)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				fieldData.Add(fieldName, SerializeAudioClip((AudioClip)fieldObj));
			}
			else if (fieldObj is Renderer)
			{
				if ((Renderer)fieldObj == null || fieldObj is LineRenderer)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = "Renderer";
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper(((Renderer)fieldObj).gameObject).monoID));
			}
			else if (fieldObj is Collider)
			{
				if ((Collider)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = "Collider";
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper(((Collider)fieldObj).gameObject).monoID));
			}
			else if (fieldObj is Collider2D)
			{
				if ((Collider2D)fieldObj == null)
				{
					return null;
				}
				fieldData = new JSONClass();
				string typeName = "Collider2D";
				fieldData.Add(COMPONENT_NAME_JSON_STRING, new JSONData(typeName));
				fieldData.Add(OBJECT_ID_JSON_STRING, new JSONData(MonoHelper.GetMonoHelper(((Collider2D)fieldObj).gameObject).monoID));
			}
			else
			{
				// Etc
				Type t = fieldObj.GetType();
				System.Reflection.MethodInfo componentMethod = t.GetMethod("ToJSON"); // Let Component handle serialization if it can
				if (componentMethod != null)
				{
					fieldData = componentMethod.Invoke(fieldObj, null) as JSONClass;
				}
				else if (t.IsValueType)
				{
					fieldData = new JSONClass();
					JSONClass fieldClass = new JSONClass();
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
						object childObj = field.GetValue(fieldObj);
						if (childObj == null)
						{
							continue;
						}
						string childName = field.Name;
						if (badFields.Contains(childName))
						{
							continue;
						}
						JSONClass childData = SaveField(childObj, childName);
						if (childData == null)
						{
							continue;
						}
						fieldClass.Add(childName, childData);
					}
					fieldData.Add(fieldName, fieldClass);
				}
				if (fieldData == null && t != typeof(Action))
				{
					Debug.LogWarning(fieldName + " (" + t + "): " + fieldObj);
				}
			}
			return fieldData;
		}

		private static JSONClass SerializeTextMesh(TextMesh mesh)
		{
			JSONClass fieldData = new JSONClass();
			fieldData.Add("alignment", new JSONData(mesh.alignment.ToString()));
			fieldData.Add("anchor", new JSONData(mesh.anchor.ToString()));
			fieldData.Add("size", new JSONData(mesh.characterSize));
			JSONClass textColor = ConvertColor(mesh.color);
			fieldData.Add("color", textColor);
			fieldData.Add("text", new JSONData(mesh.text));
			return fieldData;
		}

		private static JSONClass SerializeIEnumerable(IEnumerable fieldObj, string fieldName)
		{
			JSONClass fieldData = new JSONClass();
			JSONArray fieldArray = new JSONArray();
			if (fieldObj is IDictionary)
			{
				foreach (DictionaryEntry item in (IDictionary)fieldObj)
				{
					JSONClass keyVal = SaveField(item.Key, "key");
					JSONClass itemVal = SaveField(item.Value, "value");
					if (itemVal == null || keyVal == null)
					{
						continue;
					}
					JSONClass dictArray = new JSONClass();
					dictArray.Add("dictionarykey", keyVal);
					dictArray.Add("dictionaryvalue", itemVal);
					fieldArray.Add(dictArray);
				}
			}
			else
			{
				try
				{
					foreach (var item in (IEnumerable)fieldObj)
					{
						JSONClass itemVal = SaveField(item, "value");
						if (itemVal == null)
						{
							continue;
						}
						fieldArray.Add(itemVal);
					}
				}
				catch (UnassignedReferenceException)
				{
					return null;
				}
			}
			fieldData.Add(fieldName, fieldArray);
			return fieldData;
		}

		private static JSONClass SerializeEnum(Enum enumObj)
		{
			JSONClass fieldData = new JSONClass();
			fieldData.Add("type", new JSONData(enumObj.GetType().Name));
			fieldData.Add("value", new JSONData(enumObj.ToString()));
			return fieldData;
		}

		private static JSONClass SerializeAudioClip(AudioClip clip)
		{
			if (clip == null)
			{
				return null;
			}
			JSONClass fieldData = new JSONClass();
			fieldData.Add("name", new JSONData(clip.name));
			return fieldData;
		}
		#endregion

		public static void AutoSave()
		{
			for (int i = autosaveCount - 1; i >= 0; i--)
			{
				string filePath = GlobalConsts.GetDataPath() + "/Saves/";
				string fileName = filePath + AutoSaveNamer(i);
				fileName += "autosave.json";
				if (!File.Exists(fileName))
				{
					continue;
				}
				string newFileName = filePath + AutoSaveNamer(i + 1) + "autosave.json";
				if (File.Exists(newFileName))
				{
					File.Delete(newFileName);
				}
				File.Move(fileName, newFileName);
			}
			Save("autosave");
		}

		private static string AutoSaveNamer(int i)
		{
			string output = "";
			if (i == 1)
			{
				output += "old";
			}
			else if (i == 2)
			{
				output += "older";
			}
			else if (i >= 3)
			{
				output += "old";
				// 3 becomes "oldest", 4 becomes "oldestest", 5 becomes "oldestestest", etc.
				for (int j = i - 3; j >= 0; j--)
				{
					output += "est";
				}
			}
			return output;
		}

		public static JSONClass Save(string safeSaveName)
		{
			saving = true;
			// Filename
			foreach (char c in Path.GetInvalidFileNameChars())
			{
				safeSaveName = safeSaveName.Replace(c, '_');
			}
			safeSaveName = safeSaveName.Replace(' ', '_');
			if (safeSaveName.LastIndexOf('.') == -1)
			{
				safeSaveName += ".json";
			}

			// Collect data
			JSONClass saveData = InstanceManager.ToJSON();
			saveData.Add("loadedlevel", new JSONData(Application.loadedLevelName));

			// Writing to disk
			string rawJSON = saveData.ToString("\n");
			string safePath = Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), new LocalizedString(GlobalConsts.ID_SAVED_GAME_FOLDER_NAME, "Saved Games").Text), "Papal Simulator");
			Directory.CreateDirectory(safePath);
			safePath += "/" + safeSaveName;
			File.WriteAllText(safePath, rawJSON);
			Console.Log(new LocalizedString(GlobalConsts.ID_SAVED_GAME_DEBUG, "Successfully saved game as %s.", new object[] { safeSaveName }), true);
			saving = false;
			return saveData;
		}

		private static void MakeCamera()
		{
			if (loadingCamera != null)
			{
				return;
			}
			GameObject go = GameObject.FindGameObjectWithTag("Loading Screen");
			if (go == null)
			{
				go = new GameObject("Loading Camera");
				go.tag = "Loading Screen";
			}
			Globals.DontDestroy(go);
			loadingCamera = go.GetComponent<Camera>();
			if (loadingCamera == null)
			{
				loadingCamera = go.AddComponent<Camera>();
			}
			loadingCamera.clearFlags = CameraClearFlags.Color;
			loadingCamera.backgroundColor = Color.black;
			loadingCamera.depth = 100;
			loadingCamera.cullingMask = LayerMask.NameToLayer("Loading Screen");
			go.transform.position = Vector3.zero;
			loadingCamera.enabled = false;
		}

		public static void Load(string loadName)
		{
			IsLoading(true);
			Console.Log(new LocalizedString(GlobalConsts.ID_LOADING_GAME, "Loading savegame %s.", new object[] { loadName }), true);
			string safePath = Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), new LocalizedString(GlobalConsts.ID_SAVED_GAME_FOLDER_NAME, "Saved Games").Text), "Papal Simulator");
			Directory.CreateDirectory(safePath);
			if (loadName.LastIndexOf('.') == -1)
			{
				loadName += ".json";
			}
			safePath += "/" + loadName;
			string rawJSON = JSONSystem.JSON.GetRawJSONFromFile(safePath);
			Load(JSONSystem.JSON.Parse(rawJSON).AsObject);
		}

		public static void Load(JSONClass savedGame)
		{
			IsLoading(true);
			Application.LoadLevel(savedGame["loadedlevel"]);
			AudioManager.globalVolume = 0.0f;
			InstanceManager.FromJSON(savedGame);
		}

		public static void LoadField(JSONClass source, System.Reflection.FieldInfo field, MonoHelper dest)
		{
			if (source == null)
			{
				Console.LogWarning("Cannot find source JSON for " + field.Name + " in " + dest + "!");
				throw new SaveGameException();
			}
			string fieldName = field.Name;
			object value = field.GetValue(dest);
			if (value == null || value is LuaBinding || source[fieldName] == "")
			{
				return;
			}
			try
			{
				if (value is string)
				{
					string result = source[fieldName];
					field.SetValue(dest, result);
				}
				else if (value is double)
				{
					double result = source[fieldName].AsDouble;
					field.SetValue(dest, result);
				}
				else if (value is float)
				{
					float result = source[fieldName].AsFloat;
					field.SetValue(dest, result);
				}
				else if (value is bool)
				{
					bool result = source[fieldName].AsBool;
					field.SetValue(dest, result);
				}
				else if (value is int)
				{
					int result = source[fieldName].AsInt;
					field.SetValue(dest, result);
				}
				else if (value is Color)
				{
					Color result = ConvertColor(source[fieldName].AsObject);
					field.SetValue(dest, result);
				}
				else if (value is Vector4)
				{
					Vector4 result = ConvertVector4(source[fieldName].AsObject);
					field.SetValue(dest, result);
				}
				else if (value is Vector3)
				{
					Vector3 result = ConvertVector3(source[fieldName].AsObject);
					field.SetValue(dest, result);
				}
				else if (value is Vector2)
				{
					Vector2 result = ConvertVector2(source[fieldName].AsObject);
					field.SetValue(dest, result);
				}
				else if (value is AudioClip)
				{
					JSONClass jsonObject = source[fieldName].AsObject;
					AudioClip result = AudioManager.GetClip(jsonObject["name"]);
					field.SetValue(dest, result);
				}
				else if (value is Enum)
				{
					Enum result = ConvertEnum(source[fieldName].AsObject);
					field.SetValue(dest, result);
				}
				else if (value is GameObject)
				{
					string hash = source[OBJECT_ID_JSON_STRING];
					if (hash == null)
					{
						return;
					}
					MonoHelper helper = MonoHelper.GetMonoHelper(hash);
					if (helper == null || helper.gameObject == null)
					{
						return;
					}
					field.SetValue(dest, helper.gameObject);
				}
				else if (value is MonoHelper)
				{
					string hash = source[OBJECT_ID_JSON_STRING];
					if (hash == null)
					{
						return;
					}
					MonoHelper helper = MonoHelper.GetMonoHelper(hash);
					if (helper == null || helper.gameObject == null)
					{
						return;
					}
					field.SetValue(dest, helper);
				}
				else
				{
					Debug.LogWarning(fieldName + " (" + value.GetType() + "): " + value + ", " + source.ToString());
				}
			}
			catch (Exception ex)
			{
				if (!(ex is InvalidOperationException || ex is ArgumentException))
				{
					throw ex;
				}
			}
		}

		private static Enum ConvertEnum(JSONClass input)
		{
			string typeName = input["type"];
			string valueName = input["value"];
			Type type = Type.GetType(typeName);
			return (Enum)Enum.Parse(type, valueName);
		}

		public static bool IsLoading()
		{
			return loading;
		}

		public static void IsLoading(bool isLoading)
		{
			loading = isLoading;
			if (loadingCamera == null)
			{
				MakeCamera();
			}
			CameraFade.StartAlphaFade(Color.black, !loading, loading, GlobalConsts.SCENE_TRANSITION_TIME, 0.0f, () =>
			{
				loadingCamera.enabled = loading;
			});
		}
	}
}