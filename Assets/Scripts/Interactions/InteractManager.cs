using UnityEngine;

/// <summary>
/// Put this directly on the player to monitor active interactive contexts and manage input debouncing.
/// </summary>
public class InteractManager : MonoBehaviour
{
    // Caches the current button-based interactable script the player is standing next to.
    private InteractButton interaction;

    // An input debounce flag that requires releasing the key before registering subsequent interactions.
    private bool canInteract;

    private void Update()
    {
        // Only evaluates input states if a valid button-based interaction has been assigned.
        if (interaction != null)
        {
            // Verifies the global interaction key is pressed AND the debounce safety is cleared.
            if (InputHub.instance.Interact && canInteract)
            {
                // Executes the customized interaction logic mapped to the world object.
                interaction.OnInteract();

                // Instantly consumes the click to prevent multi-frame script triggers.
                canInteract = false;
            }
        }

        // Resets the safety clearance the exact frame the player releases the interaction key.
        if (!InputHub.instance.Interact && !canInteract)
        {
            canInteract = true;
        }
    }

    /// <summary>
    /// Public access point called by world triggers to register or unassign current interaction targets.
    /// </summary>
    /// <param name="newInteract">The incoming world object target, or null to clear current tracking.</param>
    public void SetInteraction(InteractButton newInteract)
    {
        interaction = newInteract; //
    }
}
