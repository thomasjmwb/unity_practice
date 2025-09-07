using UnityEngine;

public abstract class PlayerStateBase
{
    protected PlayerController player;
    protected PlayerMovement movement;
    protected PlayerJumping jumping;
    protected PlayerCollisionDetector collisionDetector;
    protected PlayerInputBuffer inputBuffer;
    protected PlayerInput playerInput;
    
    public PlayerStateBase(PlayerController player)
    {
        this.player = player;
        RefreshComponentReferences();
    }
    
    protected void RefreshComponentReferences()
    {
        if (player != null)
        {
            this.movement = player.movement;
            this.jumping = player.jumping;
            this.collisionDetector = player.collisionDetector;
            this.inputBuffer = player.inputBuffer;
            this.playerInput = player.playerInput;
        }
    }
    
    protected bool AreComponentsValid()
    {
        return player != null && movement != null && jumping != null && 
               collisionDetector != null && inputBuffer != null && playerInput != null;
    }
    
    public virtual void Enter() 
    {
        if (!AreComponentsValid())
        {
            RefreshComponentReferences();
        }
    }
    
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    
    protected bool TryWallJump()
    {
        if (!AreComponentsValid()) return false;
        
        if (!inputBuffer.HasBufferedJump || collisionDetector.IsGrounded) 
            return false;
        
        bool canUseLeft = collisionDetector.TouchingWallLeft || inputBuffer.HasWallLeftCoyote;
        bool canUseRight = collisionDetector.TouchingWallRight || inputBuffer.HasWallRightCoyote;
        
        if (canUseLeft && inputBuffer.IsRightAwayActive())
        {
            jumping.PerformWallJump(1);
            inputBuffer.ConsumeJumpBuffer();
            inputBuffer.ConsumeAwayBuffers();
            player.StateMachine.ChangeState(PlayerState.WallJumping);
            return true;
        }
        else if (canUseRight && inputBuffer.IsLeftAwayActive())
        {
            jumping.PerformWallJump(-1);
            inputBuffer.ConsumeJumpBuffer();
            inputBuffer.ConsumeAwayBuffers();
            player.StateMachine.ChangeState(PlayerState.WallJumping);
            return true;
        }
        
        return false;
    }
    
    protected bool TryGroundJump()
    {
        if (!AreComponentsValid()) return false;
        
        if (inputBuffer.HasBufferedJump && jumping.CanGroundJump(collisionDetector.IsGrounded))
        {
            jumping.PerformGroundJump();
            inputBuffer.ConsumeJumpBuffer();
            player.StateMachine.ChangeState(PlayerState.Airborne);
            return true;
        }
        return false;
    }
    
    protected bool TryAirJump()
    {
        if (!AreComponentsValid()) return false;
        
        if (inputBuffer.HasBufferedJump && jumping.CanAirJump())
        {
            jumping.PerformAirJump();
            inputBuffer.ConsumeJumpBuffer();
            player.StateMachine.ChangeState(PlayerState.Airborne);
            return true;
        }
        return false;
    }
    
    protected void HandleMovementInput()
    {
        if (!AreComponentsValid()) return;
        
        float inputDirection = playerInput.HorizontalInput;
        
        if (movement.TryCancelWallJump(inputDirection))
        {
            return;
        }
        
        movement.ApplyHorizontalMovement(inputDirection);
    }
    
    protected void HandleInputBuffering()
    {
        if (!AreComponentsValid()) return;
        
        if (playerInput.JumpPressed)
        {
            inputBuffer.BufferJump();
        }
        
        if (playerInput.LeftPressed)
        {
            inputBuffer.BufferLeftAway();
        }
        
        if (playerInput.RightPressed)
        {
            inputBuffer.BufferRightAway();
        }
        
        if (collisionDetector.TouchingWallLeft)
        {
            inputBuffer.RefreshWallLeftCoyote();
        }
        
        if (collisionDetector.TouchingWallRight)
        {
            inputBuffer.RefreshWallRightCoyote();
        }
    }
}