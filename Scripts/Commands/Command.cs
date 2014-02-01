using UnityEngine;
namespace HelperFunctions
{
namespace Commands
{
	public abstract class Command
	{
		private System.Collections.Generic.List<Command> commandList = new System.Collections.Generic.List<Command>();
		private int index = -1;

		/// <summary>
		/// Executes the current command.
		/// </summary>
		/// <param name="actor">The MonoBehaviour to execute the command on.</param>
		public void Execute(MonoBehaviour actor)
		{
			int numberOfUndos = commandList.Count - (index + 1);
			if (numberOfUndos > 0)
			{
				// The user has hit the undo button, so we need to clean all commands past the current one off the stack.
				commandList.RemoveRange(index + 1, numberOfUndos);
			}
			commandList.Add(this);
			index++;
		}
		/// <summary>
		/// Executes the current command.
		/// </summary>
		/// <param name="actor">The MonoBehaviour to execute the command on.</param>
		protected abstract void OnExecute(MonoBehaviour actor);

		public virtual Command Undo()
		{
			if (index <= 0 || commandList.Count == 0)
			{
				index = -1;
				return null;
			}
			index--;
			return commandList[index];
		}
		public virtual Command Redo()
		{
			if (index >= commandList.Count - 1)
			{
				index = commandList.Count - 1;
				return null;
			}
			index++;
			return commandList[index];
		}

		/// <summary>
		/// Returns a Command based on a string name.
		/// Passing the default ToString() output to this method will produce a new instance of the Command being passed.
		/// </summary>
		/// <param name="name">The name of the Command type to return.</param>
		/// <returns>A Command of the given name. Null if it could not be found or the name was not a Command.</returns>
		public static Command FromString(string name)
		{
			System.Type command = System.Type.GetType(name, true, true);
			return System.Activator.CreateInstance(command) as Command;
		}
	}
}
}