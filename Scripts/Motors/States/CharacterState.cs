using HelperFunctions.Motors;
using HelperFunctions.ObserverSystem;
namespace HelperFunctions
{
	namespace CharacterStates
	{
		public abstract class CharacterState : Observer
		{
			/// <summary>
			/// Used to mark whether this is an "immobile" state.
			/// If so, the only thing that can change this state is transitioning to null or idle.
			/// </summary>
			public bool isImmobileState = false;

			/// <summary>
			/// Called when the target enters this state.
			/// </summary>
			/// <param name="target">A MonoHelper object referencing the character in question.</param>
			public virtual void OnStateEnter(CharacterMotor target)
			{
				target.AddObserver(this);
			}

			/// <summary>
			/// Checks to see if we can change to the specified state and returns the outcome.
			/// </summary>
			/// <param name="target">The target CharacterMotor.</param>
			/// <param name="newState">The new state we are attempting to change to.</param>
			/// <returns>TRUE if the change is possible, else FALSE.</returns>
			public virtual bool ChangeState(CharacterMotor target, CharacterState newState)
			{
				if (!target.CanMove())
				{

					if (newState != null && !(newState is IdleState))
					{
						return false;
					}
				}
				return true;
			}

			/// <summary>
			/// Update the state, if needed.
			/// An example is an un-cancelable charging state. This function would keep track of every frame since the state began.
			/// After a certain period of time, the charging is "over" and states can be switched.
			/// </summary>
			/// <param name="target">A MonoHelper object referencing the character in question.</param>
			public virtual void Update(CharacterMotor target)
			{
				/* EMPTY */
			}

			/// <summary>
			/// Calls when the target leaves this state.
			/// </summary>
			/// <param name="target">A MonoHelper object referencing the character in question.</param>
			public virtual void OnStateExit(CharacterMotor target)
			{
				target.RemoveObserver(this);
			}

		}
	}
}