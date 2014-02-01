namespace HelperFunctions
{
	namespace ObserverSystem
	{
		public class MonoObserver : Observer
		{
			public MonoObserver(MonoHelper monoHelper)
			{
				helper = monoHelper;
			}
			private MonoHelper helper;
			public override void OnNotify(UnityEngine.MonoBehaviour entity, string eventType)
			{
				helper.OnNotify(entity, eventType);
			}

			public override string ToString()
			{
				if (helper == null)
				{
					return base.ToString();
				}
				return "MonoObserver (" + helper.ToString() + ")";
			}
		}
	}
}