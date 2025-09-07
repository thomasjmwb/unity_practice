using UnityEngine;

public class AirborneState : PlayerStateBase
{
    public AirborneState(PlayerController player) : base(player) { }
    
    public override void Enter()
    {
        base.Enter();
        if (!AreComponentsValid()) return;
        movement.SetGravityScale(movement.DefaultGravityScale);
    }
    
    public override void Update()
    {
        if (!AreComponentsValid()) return;
        
        HandleInputBuffering();
        
        if (collisionDetector.IsGrounded)
        {
            player.StateMachine.ChangeState(PlayerState.Grounded);
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