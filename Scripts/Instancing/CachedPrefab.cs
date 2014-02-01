using HelperFunctions.JSONSystem;
using UnityEngine;
namespace HelperFunctions
{
	namespace Instancing
	{
		public struct CachedPrefab
		{
			public string source;
			public string hash;
			public Vector3 position;
			public Vector3 scale;
			public Vector3 rotation;
			public JSONClass components;
		}
	}
}