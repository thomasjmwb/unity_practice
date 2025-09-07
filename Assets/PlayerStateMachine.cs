using UnityEngine;
using System.Collections.Generic;

public enum PlayerState
{
    Grounded,
    Airborne,
    Gliding,
    WallSliding,
    WallJumping
}

public class PlayerStateMachine
{
    private PlayerController player;
    private PlayerStateBase currentState;
    private Dictionary<PlayerState, PlayerStateBase> states;
    
    public PlayerState CurrentStateType { get; private set; }
    
    public PlayerStateMachine(PlayerController player)
    {
        this.player = player;
        InitializeStates();
    }
    
    private void InitializeStates()
    {
        states = new Dictionary<PlayerState, PlayerStateBase>
        {
            { PlayerState.Grounded, new GroundedState(player) },
            { PlayerState.Airborne, new AirborneState(player) },
            { PlayerState.Gliding, new GlidingState(player) },
            { PlayerState.WallSliding, new WallSlidingState(player) },
            { PlayerState.WallJumping, new WallJumpingState(player) }
        };
    }
    
    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        
        CurrentStateType = newState;
        currentState = states[newState];
        currentState.Enter();
    }
    
    public void Update()
    {
        currentState?.Update();
    }
    
    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
}