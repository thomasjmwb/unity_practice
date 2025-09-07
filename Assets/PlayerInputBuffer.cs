using UnityEngine;

public class PlayerInputBuffer : MonoBehaviour
{
    [Header("Buffer Settings")]
    public float jumpBufferTime = 0.12f;
    public float wallCoyoteTime = 0.12f;
    public float awayInputBufferTime = 0.12f;
    
    private float jumpBufferTimer = 0f;
    private float wallLeftCoyoteTimer = 0f;
    private float wallRightCoyoteTimer = 0f;
    private float leftAwayTimer = 0f;
    private float rightAwayTimer = 0f;
    
    public bool HasBufferedJump => jumpBufferTimer > 0f;
    public bool HasWallLeftCoyote => wallLeftCoyoteTimer > 0f;
    public bool HasWallRightCoyote => wallRightCoyoteTimer > 0f;
    public bool HasLeftAwayBuffer => leftAwayTimer > 0f;
    public bool HasRightAwayBuffer => rightAwayTimer > 0f;
    
    void Update()
    {
        UpdateTimers();
    }
    
    private void UpdateTimers()
    {
        jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);
        wallLeftCoyoteTimer = Mathf.Max(0f, wallLeftCoyoteTimer - Time.deltaTime);
        wallRightCoyoteTimer = Mathf.Max(0f, wallRightCoyoteTimer - Time.deltaTime);
        leftAwayTimer = Mathf.Max(0f, leftAwayTimer - Time.deltaTime);
        rightAwayTimer = Mathf.Max(0f, rightAwayTimer - Time.deltaTime);
    }
    
    public void BufferJump()
    {
        jumpBufferTimer = jumpBufferTime;
    }
    
    public void ConsumeJumpBuffer()
    {
        jumpBufferTimer = 0f;
    }
    
    public void RefreshWallLeftCoyote()
    {
        wallLeftCoyoteTimer = wallCoyoteTime;
    }
    
    public void RefreshWallRightCoyote()
    {
        wallRightCoyoteTimer = wallCoyoteTime;
    }
    
    public void BufferLeftAway()
    {
        leftAwayTimer = awayInputBufferTime;
    }
    
    public void BufferRightAway()
    {
        rightAwayTimer = awayInputBufferTime;
    }
    
    public void ConsumeAwayBuffers()
    {
        leftAwayTimer = 0f;
        rightAwayTimer = 0f;
    }
    
    public void ClearWallCoyoteTimes()
    {
        wallLeftCoyoteTimer = 0f;
        wallRightCoyoteTimer = 0f;
    }
    
    public bool IsLeftAwayActive()
    {
        return HasLeftAwayBuffer || Input.GetKey(KeyCode.A);
    }
    
    public bool IsRightAwayActive()
    {
        return HasRightAwayBuffer || Input.GetKey(KeyCode.D);
    }
}