using UnityEngine;
namespace Commands
{
	public abstract class Command
	{
		protected Command lastCommand = null;
		protected Command nextCommand = null;

		public abstract void Execute(MonoBehaviour actor);

		private void SetRedo(Command next)
		{
			nextCommand = next;
		}

		public virtual Command Undo()
		{
			return lastCommand;
		}
		public virtual Command Redo()
		{
			return nextCommand;
		}
	}
}
