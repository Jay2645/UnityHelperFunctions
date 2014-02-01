using System.Collections.Generic;
namespace HelperFunctions
{
	namespace ObserverSystem
	{
		public class Subject
		{
			/// <summary>
			/// This is a list of Observer classes which will recieve Notify events from us.
			/// </summary>
			protected List<Observer> observerList = new List<Observer>();

			public void Notify(UnityEngine.MonoBehaviour entity, string eventType)
			{
				Notify(entity, eventType, true);
			}

			public void Notify(UnityEngine.MonoBehaviour entity, string eventType, bool notifyAchievements)
			{
				List<Observer> allObservers = new List<Observer>();
				foreach (Observer o in observerList)
				{
					if (o == null)
					{
						continue;
					}
					allObservers.Add(o);
				}
				observerList = allObservers;
				foreach (Observer o in observerList)
				{
					o.OnNotify(entity, eventType);
				}
				if (notifyAchievements)
				{
					AchievementSystem.AchievementManager.instance.OnNotify(entity, eventType);
				}
			}

			public void AddObserver(Observer[] os)
			{
				foreach (Observer o in os)
				{
					AddObserver(o);
				}
			}

			public void AddObserver(Observer o)
			{
				if (!observerList.Contains(o))
				{
					observerList.Add(o);
				}
			}

			public void RemoveObserver(Observer o)
			{
				if (observerList.Contains(o))
				{
					observerList.Remove(o);
				}
			}
		}
	}
}