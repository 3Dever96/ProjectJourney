using UnityEngine;

/// <summary>
/// Attach this component to world objects that should execute their code automatically upon physical entry (like area zones or gateways).
/// </summary>
public class InteractCollider : Interaction
{
    private void OnTriggerEnter(Collider other)
    {
        // Immediately looks for the InteractManager on the entering object to confirm it is the player.
        InteractManager interact = other.GetComponent<InteractManager>();

        if (interact != null)
        {
            // Bypasses the key press check completely and instantly fires the gameplay event.
            OnInteract();
        }
    }
}
