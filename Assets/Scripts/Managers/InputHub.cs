using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputHub : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Jump { get; private set; }
    public bool Attack { get; private set; }
    public bool Defend { get; private set; }
    public bool Interact { get; private set; }
    public bool Pause { get; private set; }

    public static InputHub instance;

    private PlayerInput input;

    private void Awake()
    {
        instance = this;

        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        input.onActionTriggered -= OnAction;
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                Move = context.ReadValue<Vector2>();
                break;
            case "Look":
                Look = context.ReadValue<Vector2>();
                break;
            case "Jump":
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
