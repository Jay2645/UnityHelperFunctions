using HelperFunctions.ObserverSystem;
using System.Collections.Generic;
namespace HelperFunctions
{
	namespace AchievementSystem
	{
		/// <summary>
		/// Listens to all events and passes them to any achievements that have been registered with it.
		/// </summary>
		public class AchievementManager : Observer
		{
			/// <summary>
			/// Get a reference to the AchievementManager instance.
			/// </summary>
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
				set
				{
					_instance = value;
				}
			}
			private static AchievementManager _instance = null;

			/// <summary>
			/// A list of all achievements registered with this instance.
			/// </summary>
			private List<Achievement> achievements = new List<Achievement>();

			/// <summary>
			/// Registers an achievement with the AchievmentManager.
			/// </summary>
			/// <param name="a">The achievement to register.</param>
			public static void RegisterAchievement(Achievement a)
			{
				instance.AddAchievement(a);
			}
			private void AddAchievement(Achievement a)
			{
				achievements.Add(a);
			}

			/// <summary>
			/// Notifies all registered achievements that an event has occurred.
			/// </summary>
			/// <param name="entity">An entity related to the event.</param>
			/// <param name="eventType">A unique string identifying the event.</param>
			public override void OnNotify(UnityEngine.MonoBehaviour entity, string eventType)
			{
				foreach (Achievement a in achievements)
				{
					a.OnNotify(entity, eventType);
				}
			}

			/// <summary>
			/// UNIMPLEMENTED: Unlocks an achievement.
			/// </summary>
			/// <param name="achievement">The achievement to unlock.</param>
			public static void UnlockAchievement(Achievement achievement)
			{
				if (achievement.achieved)
				{
					return;
				}
				if (Globals.steamEnabled)
				{
					// string ID = achievement.achievementID;
					// Call SetAchievement in the Steamworks API using the achievement's ID.
				}
				achievement.achieved = true;
				// TODO: Write acheivement unlock code.
			}
		}
	}
}