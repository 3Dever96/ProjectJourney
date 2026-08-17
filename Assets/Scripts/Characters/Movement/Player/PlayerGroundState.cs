using UnityEngine;

[System.Serializable]
public class PlayerGroundState : PlayerState
{
    [SerializeField] private float maxSpeed;

    public override void StartState(PlayerController player)
    {
        player.VerticalSpeed = player.stickForce;
    }

    public override void UpdateState(PlayerController player)
    {
        Transform cam = Camera.main.transform;

        Vector3 direction = cam.right * InputHub.instance.Move.x + cam.forward * InputHub.instance.Move.y;
        direction.y = 0f;
        player.MoveDirection = direction.normalized;
        player.LookDirection = direction.normalized;

        float moveSpeed = maxSpeed * InputHub.instance.Move.magnitude;

        player.CurrentSpeed = moveSpeed;

        player.FaceDirection(player.LookDirection, player.turnSpeed);

        player.ApplyMovement();
    }

    public override void ChangeState(PlayerController player)
    {
        
    }

    public override void ExitState(PlayerController player)
    {
        
    }
}
