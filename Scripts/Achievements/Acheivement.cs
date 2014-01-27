using ObserverSystem;
namespace AchievementSystem
{
	/// <summary>
	/// A simple class referencing an achievement.
	/// Inherit from this class to specify things like award conditions.
	/// </summary>
	public class Achievement : Observer
	{
		/// <summary>
		/// Default constructor. Will register the achievement in the AchievementManager.
		/// </summary>
		public Achievement()
		{
			AchievementManager.RegisterAchievement(this);
		}
		/// <summary>
		/// The name of the achievement.
		/// </summary>
		public string name = "";

		/// <summary>
		/// Override this to add your achievement logic.
		/// </summary>
		/// <param name="entity">An entity related to the event (i.e. SpawnPlayer will pass the Player).</param>
		/// <param name="eventType">A string specifying the content of the event.</param>
		public override void OnNotify(UnityEngine.MonoBehaviour entity, string eventType)
		{
			/* EMPTY */
		}
	}
}