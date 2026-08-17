using UnityEngine;

// Allows instances of this plain C# class to be serialized and displayed natively inside Unity's Inspector layout.
[System.Serializable]
public class PlayerAirState : PlayerState
{
    // --- STATE CONFIGURATION ---

    // The maximum terminal velocity value allowed when falling down under the influence of gravity.
    [SerializeField] private float fallSpeed;


    // --- STATE LIFECYCLE LOOPS ---

    public override void StartState(PlayerController player)
    {
        // Structural hook left blank; ideal place to trigger floating animations or air trail particles.
    }

    public override void UpdateState(PlayerController player)
    {
        // 1. VARIABLE JUMP HEIGHT & ROOF COLLISION CHECK
        // If the player releases the jump button early, OR hits their head on an object above them...
        if (!InputHub.instance.Jump || player.CheckCollision(player.transform.position + Vector3.up * 1.5f, 0.15f))
        {
            // Instantly cancels upward momentum to create sharp variable jump heights or cleanly arrest ceiling impacts.
            player.VerticalSpeed = Mathf.Min(player.VerticalSpeed, 0f);
        }

        // 2. GRAVITY ENGINE ACCELERATION
        // Continues pushing the player downwards as long as they haven't reached their maximum terminal fall speed.
        if (player.VerticalSpeed > fallSpeed)
        {
            // Accumulates custom gravity values linearly over time, frame-rate independent.
            player.VerticalSpeed += player.gravity * Time.deltaTime;
        }

        // 3. MID-AIR ORIENTATION
        // Maintains character rotation alignment toward the last recorded look direction while airborne.
        player.FaceDirection(player.LookDirection, player.turnSpeed);

        // 4. PHYSICS TRANSLATION CONTEXTS
        // Processes accumulated velocity arrays down to Unity's CharacterController system displacement solver.
        player.ApplyMovement();
    }

    public override void ChangeState(PlayerController player)
    {
        // LANDING DETECTION RULES
        // If the player is falling or stable (velocity <= 0) AND the collision sphere detects solid ground underneath...
        if (player.VerticalSpeed <= 0f && player.CheckCollision(player.transform.position + Vector3.up * 0.5f, -0.15f))
        {
            // Safely returns the player state tracking loops back to grounded movement models.
            player.SetState(player.GroundState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Structural hook left blank; ideal place to clear landing animation keys or trigger hard landing impacts.
    }
}
