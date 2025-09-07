using UnityEngine;

public class WallSlidingState : PlayerStateBase
{
    public WallSlidingState(PlayerController player) : base(player) { }
    
    public override void Enter()
    {
        movement.SetGravityScale(movement.DefaultGravityScale);
    }
    
    public override void Update()
    {
        HandleInputBuffering();
        
        if (collisionDetector.IsGrounded)
        {
            player.StateMachine.ChangeState(PlayerState.Grounded);
            return;
        }
        
        if (!collisionDetector.TouchingWallLeft && !collisionDetector.TouchingWallRight)
        {
            player.StateMachine.ChangeState(PlayerState.Airborne);
            return;
        }
        
        if (TryWallJump())
        {
            return;
        }
        
        if (TryAirJump())
        {
            return;
        }
        
        bool isGliding = movement.Rigidbody.linearVelocity.y < 0f && 
                        playerInput.GlideHeld && 
                        !jumping.IsGlideLocked;
        
        if (isGliding)
        {
            player.StateMachine.ChangeState(PlayerState.Gliding);
            return;
        }
        
        HandleMovementInput();
        movement.EnsureNoRotation();
    }
}