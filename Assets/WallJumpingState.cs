using UnityEngine;

public class WallJumpingState : PlayerStateBase
{
    private float wallJumpTimer = 0f;
    private const float WALL_JUMP_STATE_DURATION = 0.2f;
    
    public WallJumpingState(PlayerController player) : base(player) { }
    
    public override void Enter()
    {
        movement.SetGravityScale(movement.DefaultGravityScale);
        wallJumpTimer = WALL_JUMP_STATE_DURATION;
    }
    
    public override void Update()
    {
        wallJumpTimer -= Time.deltaTime;
        
        HandleInputBuffering();
        
        if (collisionDetector.IsGrounded)
        {
            player.StateMachine.ChangeState(PlayerState.Grounded);
            return;
        }
        
        if (wallJumpTimer <= 0f)
        {
            bool isGliding = movement.Rigidbody.linearVelocity.y < 0f && 
                            playerInput.GlideHeld && 
                            !jumping.IsGlideLocked;
            
            if (isGliding)
            {
                player.StateMachine.ChangeState(PlayerState.Gliding);
            }
            else
            {
                player.StateMachine.ChangeState(PlayerState.Airborne);
            }
            return;
        }
        
        HandleMovementInput();
        movement.EnsureNoRotation();
    }
}