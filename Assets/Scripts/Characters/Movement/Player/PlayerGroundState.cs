using UnityEngine;

// Allows instances of this plain C# class to be serialized and displayed natively inside Unity's Inspector layout.
[System.Serializable]
public class PlayerGroundState : PlayerState
{
    // --- STATE CONFIGURATION ---

    // Maximum horizontal velocity threshold allowed while moving on solid terrain surfaces.
    [SerializeField] private float maxSpeed;

    // Upward instantaneous velocity applied to the character when triggering a jump execution.
    [SerializeField] private float jumpSpeed;

    // Input debounce flag requiring the jump button to be fully released before triggering another jump.
    private bool canJump;


    // --- STATE LIFECYCLE LOOPS ---

    public override void StartState(PlayerController player)
    {
        // Applies a constant downward bias force to safely lock the character controller cleanly onto slopes or steps.
        player.VerticalSpeed = player.stickForce;

        // Locks jumping capabilities instantly on entry until structural input registration cycles update.
        canJump = false;
    }

    public override void UpdateState(PlayerController player)
    {
        // 1. CAMERA-RELATIVE DIRECTION CALCULATIONS
        // References the main scene view camera transform path for alignment.
        Transform cam = Camera.main.transform;

        // Displaces intended vector direction relative to the local camera horizon grid paths.
        Vector3 direction = cam.right * InputHub.instance.Move.x + cam.forward * InputHub.instance.Move.y;

        // flattens vertical elevation shifts to eliminate unintended flying or grounding tendencies.
        direction.y = 0f;

        // Standardizes coordinate output arrays before caching them across global movement references.
        player.MoveDirection = direction.normalized;
        player.LookDirection = direction.normalized;

        // 2. SPEED PROFILE COMPILING
        // Multiplies default maximum caps relative to absolute analog stick actuation weights.
        player.CurrentSpeed = maxSpeed * InputHub.instance.Move.magnitude;

        // Smoothly spins character meshes toward intended movement angles based on configured turn speed rules.
        player.FaceDirection(player.LookDirection, player.turnSpeed);

        // 3. DEBOUNCED JUMP LOGIC ENGINE
        // Validates explicit physical action clicks against current internal safety allowance markers.
        if (InputHub.instance.Jump && canJump)
        {
            // Direct injection of upward velocity vector forces.
            player.VerticalSpeed = jumpSpeed;

            // Consumes structural availability immediately to prevent automated multi-frame jump looping.
            canJump = false;
        }

        // Refreshes structural jumping clearance options safely the exact moment button context clicks drop.
        if (!InputHub.instance.Jump && !canJump)
        {
            canJump = true;
        }

        // 4. PHYSICS TRANSLATION CONTEXTS
        // Applies structural coordinates directly down towards Unity's character control system solvers.
        player.ApplyMovement();
    }

    public override void ChangeState(PlayerController player)
    {
        // Evaluates if the upward velocity vector threshold breaks grounding baseline bounds...
        // ...or drops a custom offset checking sphere below the character root footprint grid to detect edges.
        if (player.VerticalSpeed > 0f || !player.CheckCollision(player.transform.position + Vector3.up * 0.5f, -0.15f))
        {
            // Safely transitions tracking loops straight over towards airborne state handling models.
            player.SetState(player.AirState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Structural hook left blank; ideal place to later reset custom landing cooldown parameters.
    }
}
