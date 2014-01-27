using ObserverSystem;
using System.Collections.Generic;
namespace AchievementSystem
{
	public class AchievementManager : Observer
	{
		public static AchievementManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AchievementManager();
				}
				return _instance;
			}
		}

		private static AchievementManager _instance = null;
		private static List<Achievement> achievements = new List<Achievement>();

		public override void OnNotify(UnityEngine.MonoBehaviour entity, string eventType)
		{
			foreach (Achievement a in achievements)
			{
				a.OnNotify(entity, eventType);
			}
		}

		public static void UnlockAchievement(Achievement achievement)
		{
			// TODO: Write acheivement unlock code.
		}
	}
}