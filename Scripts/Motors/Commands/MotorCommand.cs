using HelperFunctions.CharacterStates;
using HelperFunctions.Motors;
namespace HelperFunctions
{
	namespace Commands
	{
		public abstract class MotorCommand : InputCommand
		{
			protected abstract CharacterState GetCommandState();
			protected CharacterMotor GetMotor(UnityEngine.MonoBehaviour source)
			{
				return source.GetComponentInChildren<CharacterMotor>();
			}
		}
	}
}