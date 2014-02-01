using HelperFunctions.CharacterStates;
using HelperFunctions.Motors;
namespace HelperFunctions
{
	namespace Commands
	{
		/// <summary>
		/// An abstract class to make a character move.
		/// </summary>
		public abstract class MoveCommand : MotorCommand
		{
			protected override void OnExecute(UnityEngine.MonoBehaviour actor)
			{
				CharacterMotor motor = GetMotor(actor);
				if (motor == null)
				{
					return;
				}
				if (motor.ChangeState(new MoveState()))
				{
					ExecuteMove(motor);
				}
			}

			/// <summary>
			/// Performs logic to make a character move.
			/// </summary>
			/// <param name="motor">A character's CharacterMotor.</param>
			protected abstract void ExecuteMove(CharacterMotor motor);
		}

		/// <summary>
		/// A command that makes a character move left.
		/// </summary>
		public class MoveLeftCommand : MoveCommand
		{
			protected override void ExecuteMove(CharacterMotor motor)
			{
				// Whatever logic moves your character left
			}
		}

		public class MoveRightCommand : MoveCommand
		{
			protected override void ExecuteMove(CharacterMotor motor)
			{
				// Whatever logic moves your character right
			}
		}

		public class MoveForwardCommand : MoveCommand
		{
			protected override void ExecuteMove(CharacterMotor motor)
			{
				// Whatever logic moves your character forward
			}
		}

		public class MoveBackCommand : MoveCommand
		{
			protected override void ExecuteMove(CharacterMotor motor)
			{
				// Whatever logic moves your character back
			}
		}
	}
}