using Hashing;
using Instancing;
using JSONSystem;
using ObserverSystem;
using UnityEngine;

/// <summary>
/// An extension of the default MonoBehaviour, the MonoHelper class aids in identifying GameObjects across scenes and between loads.
/// It also provides any helper or utility methods which apply to all MonoBehaviours.
/// </summary>
public class MonoHelper : MonoBehaviour
{
	// Subject
	private MonoSubject subject
	{
		get
		{
			// Lazy instancing.
			if (_subject == null)
			{
				_subject = new MonoSubject(this);
			}
			return _subject;
		}
	}
	private MonoSubject _subject = null;

	// Hash
	private MonoHash hash
	{
		get
		{
			if (_hash == null)
			{
				_hash = new MonoHash(this);
			}
			return _hash;
		}
	}
	private MonoHash _hash = null;
	public string monoID
	{
		get
		{
			return hash.monoID;
		}
	}

	// Instancing
	private ObjectLoader loader
	{
		get
		{
			if (_loader == null)
			{
				_loader = new ObjectLoader();
			}
			return _loader;
		}
	}
	private ObjectLoader _loader = null;

	// JSON
	private MonoJSON json
	{
		get
		{
			if (_json == null)
			{
				_json = new MonoJSON(this);
			}
			return _json;
		}
	}
	private MonoJSON _json = null;

	public MonoObserver thisObserver
	{
		get
		{
			return subject.thisObserver;
		}
	}

	public void SetName(string newName)
	{
		hash.SetName(newName);
	}

	protected virtual void Start()
	{
		SetName(name);
	}

	public static Object Load(Vector3 location, string path)
	{
		return ObjectLoader.Load(location, path);
	}

	public static Object Load(Transform parent, Vector3 localPosition, string path)
	{
		return ObjectLoader.Load(parent, localPosition, path);
	}

	public static Object Load(string path, Vector3 location, MonoHelper instance)
	{
		return ObjectLoader.Load(path, location, instance);
	}

	public static Object Load(string path, Transform parent, Vector3 localPosition, MonoHelper instance)
	{
		return ObjectLoader.Load(path, parent, localPosition, instance);
	}

	public Object Load(string path, Transform parent, Vector3 localPosition)
	{
		return loader.Load(path, parent, localPosition);
	}

	public Object Load(string path, Transform parent, Vector3 localPosition, string newName)
	{
		return loader.Load(path, parent, localPosition, newName);
	}

	public Object Load(string path, Vector3 location)
	{
		return loader.Load(path, location);
	}

	public Object Load(string path, Vector3 location, string newName)
	{
		return loader.Load(path, location, newName);
	}

	public void DontDestroy()
	{
		ObjectLoader.DontDestroy(gameObject);
	}

	protected virtual void OnDestroy()
	{
		hash.CleanUp();
	}

	public void LoadFromCache(CachedPrefab cachedGO)
	{
		json.LoadFromCache(cachedGO);
	}

	public virtual JSONClass ToJSON()
	{
		return json.ToJSON();
	}

	public virtual void FromJSON(JSONClass json)
	{
		this.json.FromJSON(json);
	}

	public void Notify(MonoBehaviour entity, string eventType)
	{
		subject.Notify(entity, eventType);
	}

	public void Notify(MonoBehaviour entity, string eventType, bool notifyAchievements)
	{
		subject.Notify(entity, eventType, notifyAchievements);
	}

	public void AddObserver(Observer[] os)
	{
		subject.AddObserver(os);
	}

	public void AddObserver(Observer o)
	{
		subject.AddObserver(o);
	}

	public void RemoveObserver(Observer o)
	{
		subject.RemoveObserver(o);
	}

	public virtual void OnNotify(MonoBehaviour entity, string eventType)
	{
	}

	public MonoHash GetHash()
	{
		return hash;
	}

	public static JSONClass ToJSON(MonoHelper helper)
	{
		return MonoJSON.ToJSON(helper);
	}

	protected static JSONClass GetFields(MonoHelper source)
	{
		return MonoJSON.GetFields(source);
	}

	protected static JSONClass GetFields(MonoHelper source, JSONClass fieldArray)
	{
		return MonoJSON.GetFields(source, fieldArray);
	}

	public static MonoHelper GetMonoHelper(GameObject go)
	{
		return MonoHash.GetMonoHelper(go);
	}

	public static MonoHelper GetMonoHelper(string hash)
	{
		return MonoHash.GetMonoHelper(hash);
	}

	public static void AddToLoadingQueue(MonoHelper helper)
	{
		ObjectLoader.AddToLoadingQueue(helper);
	}
}
