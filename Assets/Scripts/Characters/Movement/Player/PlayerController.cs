using UnityEngine;

public class PlayerController : MovementController
{
    // --- STATE MACHINE TRACKING ---

    // Reference to the currently active runtime state execution block.
    public PlayerState CurrentState { get; private set; }


    // --- INSPECTOR SERIALIZED STATES ---

    // Instantiates and exposes the ground state directly within the Inspector layout.
    [field: SerializeField] public PlayerGroundState GroundState { get; private set; } = new PlayerGroundState();

    // Instantiates and exposes the air state directly within the Inspector layout.
    [field: SerializeField] public PlayerAirState AirState { get; private set; } = new PlayerAirState();


    // --- LIFECYCLE HOOKS ---

    protected override void Start()
    {
        // Executes base logic inside MovementController to cleanly cache the CharacterController component.
        base.Start();

        // Sets the baseline starting point for the player execution stack.
        SetState(GroundState);
    }

    private void Update()
    {
        // Safely processes continuous processing ticks only if a valid state is currently loaded.
        if (CurrentState != null)
        {
            // Updates internal frame behaviors like calculating velocities or processing directions.
            CurrentState.UpdateState(this);

            // Constantly checks conditional flags to determine if it needs to transition to a new state.
            CurrentState.ChangeState(this);
        }
    }


    // --- STATE LIFECYCLE MANAGER ---

    /// <summary>
    /// Safely handles exiting the previous state behavior and initializing the next one.
    /// </summary>
    /// <param name="newState">The target incoming player state to execute.</param>
    public void SetState(PlayerState newState)
    {
        // 1. CLEANUP PREVIOUS STATE
        if (CurrentState != null)
        {
            // Fires teardown code blocks like resetting timers or animation tags.
            CurrentState.ExitState(this);
        }

        // 2. ASSIGN NEW ACTIVE REFERENCE
        CurrentState = newState;

        // 3. INITIALIZE NEW STATE
        if (CurrentState != null)
        {
            // Fires initial structural code blocks like playing starting particles or sound elements.
            CurrentState.StartState(this);
        }
    }
}
