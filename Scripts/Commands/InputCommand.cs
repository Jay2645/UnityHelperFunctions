using UnityEngine;
namespace Commands
{
	/// <summary>
	/// A list of all InputCommand names.
	/// You don't need to keep this up to date if you plan on hardcoding everything.
	/// However, you can use this if you want designers to be able to map keys to InputCommands in the editor.
	/// </summary>
	public enum InputCommandName
	{
		InputCommand,
		MoveLeftCommand,
		MoveRightCommand,
		MoveForwardCommand,
		MoveBackCommand
	}

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

		/// <summary>
		/// Takes an InputCommandName enum and returns the associated InputCommand.
		/// </summary>
		/// <param name="name">The InputCommandName to look up.</param>
		/// <returns>The InputCommand associated with the InputCommandName enum, or null if nothing was found.</returns>
		public static InputCommand FromEnum(InputCommandName name)
		{
			return FromString(typeof(InputCommand).Namespace + "." + name.ToString()) as InputCommand;
		}
	}
}