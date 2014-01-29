using UnityEngine;
namespace Commands
{
	/// <summary>
	/// A version of a Command used for processing input.
	/// Has special methods for buttons being pressed down or let go.
	/// </summary>
	public abstract class InputCommand : Command
	{
		/// <summary>
		/// The key which is pressed to activate this command.
		/// </summary>
		public KeyCode keyPress = 0;

		/// <summary>
		/// Called the first frame a button is pressed down. Will not be called any subsequent frames.
		/// </summary>
		/// <param name="actor">The MonoBehaviour to execute the command on.</param>
		public virtual void ExecuteButtonDown(MonoBehaviour actor)
		{
			/* EMPTY */
		}

		/// <summary>
		/// Called the first frame a button is let go. Will not be called any prior or subsequent frames.
		/// </summary>
		/// <param name="actor">The MonoBehaviour to execute the command on.</param>
		public virtual void ExecuteButtonUp(MonoBehaviour actor)
		{
			/* EMPTY */
		}
	}
}