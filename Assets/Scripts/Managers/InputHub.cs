using UnityEngine;
using UnityEngine.InputSystem; // Required for utilizing the New Input System architecture.

// Automatically adds and enforces the PlayerInput component dependency on this GameObject.
[RequireComponent(typeof(PlayerInput))]
public class InputHub : MonoBehaviour
{
    // --- INPUT DATA PROPERTIES (Read-Only Externally) ---

    // 2D directional vector representing standard movement input (WASD / Left Thumbstick).
    public Vector2 Move { get; private set; }

    // 2D directional vector representing camera pointer look input (Mouse / Right Thumbstick).
    public Vector2 Look { get; private set; }

    // State tracks whether the Jump action button is actively pressed.
    public bool Jump { get; private set; }

    // State tracks whether the Attack action button is actively pressed.
    public bool Attack { get; private set; }

    // State tracks whether the Defend action button is actively pressed.
    public bool Defend { get; private set; }

    // State tracks whether the Interact action button is actively pressed.
    public bool Interact { get; private set; }

    // State tracks whether the Pause action button is actively pressed.
    public bool Pause { get; private set; }


    // --- MANAGERS & SINGLETON ---

    // Global static reference to easily pull input state data from other scripts.
    public static InputHub instance;

    // Internal reference to the local PlayerInput component handling asset bindings.
    private PlayerInput input;

    private void Awake()
    {
        // Allocates the Singleton instance access point.
        instance = this;

        // Caches the required PlayerInput component reference.
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Subscribes our local parsing method to Unity's global input action callback system.
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        // Unsubscribes to cleanly prevent memory leaks or errors when the object is destroyed/disabled.
        input.onActionTriggered -= OnAction;
    }

    /// <summary>
    /// Master callback receiver called by PlayerInput whenever any bound action state changes.
    /// </summary>
    /// <param name="context">Contains structural frame info regarding the triggered action phase and values.</param>
    public void OnAction(InputAction.CallbackContext context)
    {
        // Filters incoming calls by matching the exact action map name string specified in your Input Action Asset.
        switch (context.action.name)
        {
            case "Move":
                // Continually reads continuous Vector2 positions (e.g., Vector2.up, Vector2.zero).
                Move = context.ReadValue<Vector2>();
                break;
            case "Look":
                // Continually reads pointer delta or positional thumbstick Vector2 values.
                Look = context.ReadValue<Vector2>();
                break;
            case "Jump":
                // Reads true during the performed/started phases, and false when the key is released.
                Jump = context.ReadValue<bool>();
                break;
            case "Attack":
                Attack = context.ReadValue<bool>();
                break;
            case "Defend":
                Defend = context.ReadValue<bool>();
                break;
            case "Interact":
                Interact = context.ReadValue<bool>();
                break;
            case "Pause":
                Pause = context.ReadValue<bool>();
                break;
        }
    }
}
