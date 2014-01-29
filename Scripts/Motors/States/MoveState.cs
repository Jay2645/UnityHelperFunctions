public class MoveState : HorizontalState
{
	public override bool ChangeState(CharacterState newState)
	{
		// Replace with your own logic to govern when you can move and when you cannot.
		return base.ChangeState(newState);
	}
}
