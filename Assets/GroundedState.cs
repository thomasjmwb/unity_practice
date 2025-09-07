using UnityEngine;

public class GroundedState : PlayerStateBase
{
    public GroundedState(PlayerController player) : base(player) { }
    
    public override void Enter()
    {
        base.Enter(); // Ensure components are refreshed
        
        if (!AreComponentsValid()) return;
        
        jumping.ResetAirJumps();
        jumping.ClearGlideLockout();
        movement.SetGravityScale(movement.DefaultGravityScale);
        movement.ClearWallJumpState();
        inputBuffer.ClearWallCoyoteTimes();
    }
    
    public override void Update()
    {
        HandleInputBuffering();
        
        if (!collisionDetector.IsGrounded)
        {
            player.StateMachine.ChangeState(PlayerState.Airborne);
            return;
        }
        
        if (TryGroundJump())
        {
            return;
        }
        
        HandleMovementInput();
        movement.EnsureNoRotation();
    }
}