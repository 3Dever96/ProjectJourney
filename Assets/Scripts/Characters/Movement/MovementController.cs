using UnityEngine;

// Automatically adds a CharacterController component to the GameObject if it's missing.
[RequireComponent(typeof(CharacterController))]
public abstract class MovementController : MonoBehaviour
{
    // --- PROPERTIES & COMPONENTS ---

    // Read-only reference to the required CharacterController component.
    public CharacterController Controller { get; private set; }

    // Calculated horizontal movement speed.
    public float CurrentSpeed { get; set; }

    // Calculated vertical speed (used for gravity, jumping, or falling calculations).
    public float VerticalSpeed { get; set; }

    // Target direction the character should face.
    public Vector3 LookDirection { get; set; }

    // Intended direction of movement, derived from player or AI input (typically normalized).
    public Vector3 MoveDirection { get; set; }

    // The combined structural velocity vector applied directly to the character.
    public Vector3 Velocity { get; set; }


    // --- INSPECTOR CONFIGURATION ---

    [Header("Universal Physics")]

    // Constant downward velocity used to snap characters firmly to slopes or steps.
    public float stickForce = -5f;

    // Custom gravity acceleration value overriding standard Unity physics (9.81f).
    public float gravity = 14.715f;

    // Angular rotation speed determining how fast the character pivots toward its target direction.
    public float turnSpeed = 500f;


    // --- LIFECYCLE METHODS ---

    protected virtual void Start()
    {
        // Safely caches the CharacterController instance when the script initializes.
        Controller = GetComponent<CharacterController>();
    }


    // --- MOVEMENT & UTILITY METHODS ---

    /// <summary>
    /// Smoothly rotates the GameObject toward a target vector direction.
    /// </summary>
    /// <param name="forward">The vector target direction to look toward.</param>
    /// <param name="speed">The angular speed limit of the rotation.</param>
    public void FaceDirection(Vector3 forward, float speed)
    {
        // Safeguards against passing an empty or invalid vector which can crash Quaternion calculations.
        if (forward == Vector3.zero) return;

        // Smoothly interpolates the rotation toward the target look direction, decoupled from framerate.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward), speed * Time.deltaTime);
    }

    /// <summary>
    /// Performs a spherical collision query to check for physical obstructions.
    /// </summary>
    /// <param name="origin">The central coordinates of the character root.</param>
    /// <param name="offset">Vertical offset shift from the origin baseline.</param>
    /// <returns>True if an object on the "Solid" layer overlaps the sphere bounds.</returns>
    public bool CheckCollision(Vector3 origin, float offset)
    {
        // Adjusts the check radius slightly inward (-0.1f) to prevent false positives from scraping walls.
        float checkRadius = Controller.radius - 0.1f;
        Vector3 sphereCenter = origin + (Vector3.up * offset);

        // Uses a non-allocating overlap sphere query constrained to the "Solid" physics layer mask.
        return Physics.CheckSphere(sphereCenter, checkRadius, LayerMask.GetMask("Solid"));
    }

    /// <summary>
    /// Compiles all directional and vertical components and processes final character displacement.
    /// </summary>
    public void ApplyMovement()
    {
        // Combines lateral movement speed and direction.
        Vector3 velocity = CurrentSpeed * MoveDirection;

        // Integrates current vertical falling or jumping movement.
        velocity.y = VerticalSpeed;

        // Caches the absolute velocity result locally.
        Velocity = velocity;

        // Displaces the character through Unity's custom frame-independent CharacterController solver.
        Controller.Move(Velocity * Time.deltaTime);
    }
}
