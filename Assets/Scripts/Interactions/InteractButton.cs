using UnityEngine;

/// <summary>
/// Attach this component to world objects that require the player to manually stand nearby and press an interaction key.
/// </summary>
public class InteractButton : Interaction
{
    private void OnTriggerStay(Collider other)
    {
        // Searches upstream from the overlapping collider to find the player's InteractManager component.
        InteractManager interact = other.GetComponentInParent<InteractManager>();

        if (interact != null)
        {
            // Continuously registers this specific object as the player's active interaction target.
            interact.SetInteraction(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Finds the player's InteractManager component when they step away from the zone.
        InteractManager interact = other.GetComponentInParent<InteractManager>();

        if (interact != null)
        {
            // Clears out the player's current interaction target to prevent out-of-range execution.
            interact.SetInteraction(null);
        }
    }
}
