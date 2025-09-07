using UnityEngine;

public class GlidingState : PlayerStateBase
{
    public GlidingState(PlayerController player) : base(player) { }
    
    public override void Enter()
    {
        movement.SetGravityScale(movement.glideGravityScale);
        
        if (!movement.WasGliding)
        {
            movement.StartGlideBoost(playerInput.HorizontalInput);
        }
    }
    
    public override void Update()
    {
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
        
        bool shouldGlide = movement.Rigidbody.linearVelocity.y < 0f && 
                          playerInput.GlideHeld && 
                          !jumping.IsGlideLocked;
        
        if (!shouldGlide)
        {
            player.StateMachine.ChangeState(PlayerState.Airborne);
            return;
        }
        
        movement.UpdateGlideBoost(playerInput.HorizontalInput, true);
        
        float glideMultiplier = movement.CalculateGlideMultiplier();
        movement.ApplyHorizontalMovement(playerInput.HorizontalInput, glideMultiplier);
        
        movement.EnsureNoRotation();
    }
    
    public override void Exit()
    {
        movement.SetWasGliding(true);
    }
}