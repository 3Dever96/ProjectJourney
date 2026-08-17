using UnityEngine;

/// <summary>
/// Abstract foundational base class defining the mandatory lifecycle methods 
/// that every player state (e.g., Grounded, Airborne) must implement.
/// </summary>
public abstract class PlayerState
{
    /// <summary>
    /// Triggered exactly once when the PlayerController transitions into this state.
    /// Ideal for initializing variables, playing audio, or triggering entry animations.
    /// </summary>
    /// <param name="player">The running instance of the PlayerController driving this state.</param>
    public abstract void StartState(PlayerController player);

    /// <summary>
    /// Executed on every frame inside the PlayerController's Update loop.
    /// Ideal for continuous operations like parsing input, updating movement data, or counting timers.
    /// </summary>
    /// <param name="player">The running instance of the PlayerController driving this state.</param>
    public abstract void UpdateState(PlayerController player);

    /// <summary>
    /// Executed on every frame directly after UpdateState.
    /// Reserved specifically for checking condition flags and handling transition thresholds to other states.
    /// </summary>
    /// <param name="player">The running instance of the PlayerController driving this state.</param>
    public abstract void ChangeState(PlayerController player);

    /// <summary>
    /// Triggered exactly once immediately before the PlayerController switches away to a new state.
    /// Ideal for cleaning up active flags, stopping looping effects, or resetting timers.
    /// </summary>
    /// <param name="player">The running instance of the PlayerController driving this state.</param>
    public abstract void ExitState(PlayerController player);
}
