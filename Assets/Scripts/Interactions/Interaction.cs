using UnityEngine;

/// <summary>
/// The foundational abstract base component for all interactive elements in the game world.
/// </summary>
public class Interaction : MonoBehaviour
{
    /// <summary>
    /// Virtual lifecycle hook intended to be overridden by unique objects (chests, gates, doors) 
    /// to execute their specific gameplay logic when triggered.
    /// </summary>
    public virtual void OnInteract()
    {
        // Left blank intentionally for child scripts to populate with override code.
    }
}
