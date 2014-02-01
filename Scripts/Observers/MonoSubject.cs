namespace HelperFunctions
{
	namespace ObserverSystem
	{
		public class MonoSubject : Subject
		{
			public MonoSubject(MonoHelper mHelper)
			{
				helper = mHelper;
				AddConstantObservers();
			}
			private MonoHelper helper = null;
			public MonoObserver thisObserver = null;

			/// <summary>
			/// This adds any Observer classes which are used by ALL MonoSubjects.
			/// </summary>
			private void AddConstantObservers()
			{
				if (thisObserver != null)
				{
					return;
				}
				thisObserver = new MonoObserver(helper);
				AddObserver(thisObserver);
			}

			public MonoObserver GetObserver()
			{
				return thisObserver;
			}
		}
	}
}