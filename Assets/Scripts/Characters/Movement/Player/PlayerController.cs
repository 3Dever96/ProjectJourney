using UnityEngine;

public class PlayerController : MovementController
{
    public PlayerState CurrentState {  get; private set; }

    [field: SerializeField] public PlayerGroundState GroundState { get; private set; } = new PlayerGroundState();

    protected override void Start()
    {
        base.Start();

        SetState(GroundState);
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState(this);
            CurrentState.ChangeState(this);
        }
    }

    public void SetState(PlayerState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            CurrentState.StartState(this);
        }
    }
}
