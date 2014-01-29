
public class CharacterMotor : MonoHelper
{
	/// <summary>
	/// The current state of this character on the HORIZONTAL axis.
	/// i.e. Running, walking, etc. Null if there is no horizontal movement.
	/// </summary>
	public HorizontalState currentHorizontalState
	{
		get
		{
			return _currentHorizontalState;
		}
		set
		{
			_currentHorizontalState = ChangeState(_currentHorizontalState, value) as HorizontalState;
		}
	}
	private HorizontalState _currentHorizontalState = null;

	/// <summary>
	/// The current state of this character on the VERTICAL axis.
	/// i.e. Jumping, ducking, etc. Null if there is no vertical movement.
	/// </summary>
	public VerticalState currentVerticalState
	{
		get
		{
			return _currentVerticalState;
		}
		set
		{
			_currentVerticalState = ChangeState(_currentVerticalState, value) as VerticalState;
		}
	}
	private VerticalState _currentVerticalState = null;

	/// <summary>
	/// The current state of this character's EQUIPMENT.
	/// For example, different weaponry should be different states.
	/// </summary>
	public EquipmentState currentEquipmentState
	{
		get
		{
			return _currentEquipmentState;
		}
		set
		{
			_currentEquipmentState = ChangeState(_currentEquipmentState, value) as EquipmentState;
		}
	}
	private EquipmentState _currentEquipmentState = null;

	/// <summary>
	/// The "default" state. Used when both Horizontal and Vertical states are null.
	/// Should normally be "standing," "idle," or some equivalent.
	/// </summary>
	private IdleState defaultState = new IdleState();

	/// <summary>
	/// Will attempt to change this character's present HorizontalState to the specified state.
	/// Will not do anything if the state could not be changed.
	/// </summary>
	/// <param name="nextState">The state to try and change to. </param>
	/// <returns>TRUE if it was successful, else false.</returns>
	public bool ChangeState(HorizontalState nextState)
	{
		currentHorizontalState = nextState;
		return nextState == currentHorizontalState;
	}

	/// <summary>
	/// Will attempt to change this character's present VerticalState to the specified state.
	/// Will not do anything if the state could not be changed.
	/// </summary>
	/// <param name="nextState">The state to try and change to. </param>
	/// <returns>TRUE if it was successful, else false.</returns>
	public bool ChangeState(VerticalState nextState)
	{
		currentVerticalState = nextState;
		return nextState == currentVerticalState;
	}

	/// <summary>
	/// Will attempt to change this character's present EquipmentState to the specified state.
	/// Will not do anything if the state could not be changed.
	/// </summary>
	/// <param name="nextState">The state to try and change to. </param>
	/// <returns>TRUE if it was successful, else false.</returns>
	public bool ChangeState(EquipmentState nextState)
	{
		currentEquipmentState = nextState;
		return nextState == currentEquipmentState;
	}

	/// <summary>
	/// Attempts to change the state of the Character. Will not do anything if the state cannot be changed. 
	/// </summary>
	/// <param name="thisState">The current state of the character.</param>
	/// <param name="nextState">The state to attempt to change to.</param>
	/// <returns>The state the character should be in.</returns>
	protected CharacterState ChangeState(CharacterState thisState, CharacterState nextState)
	{
		if (thisState != null)
		{
			if (!thisState.ChangeState(nextState))
			{
				return thisState;
			}
			thisState.OnStateExit(this);
		}
		thisState = nextState;
		if (thisState == null)
		{
			return null;
		}
		thisState.OnStateEnter(this);
		return thisState;
	}

	/// <summary>
	/// Is this the player? If not, AI hooks in to determine states.
	/// </summary>
	public bool isPlayer = false;

	protected virtual void FixedUpdate()
	{
		if (isPlayer)
		{
			KeyBinds.CheckKeysDown(this);
		}
		else
		{
			// AI hook goes here!
		}
		if (currentHorizontalState != null)
		{
			currentHorizontalState.Update(this);
		}
		if (currentVerticalState != null)
		{
			currentVerticalState.Update(this);
		}
		else if (currentHorizontalState == null)
		{
			// Both Horizontal and Vertical states are null -- move to default state.
			defaultState = (IdleState)ChangeState(null, defaultState); // This is merely a formality to make sure the OnStateEnter gets called.
			defaultState.Update(this);
		}
		if (currentEquipmentState != null)
		{
			currentEquipmentState.Update(this);
		}
	}
}
