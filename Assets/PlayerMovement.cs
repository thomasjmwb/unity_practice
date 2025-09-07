using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float glideGravityScale = 0.3f;
    public float glideHorizontalMultiplier = 1.2f;
    public float initialGlideBoostMultiplier = 2f;
    public float initialGlideBoostDuration = 0.75f;
    
    [Header("Wall Jump Cancel")]
    public float wallJumpCancelDuration = 0.5f;
    
    private Rigidbody2D rb;
    private float defaultGravityScale;
    
    private float glideBoostTimeLeft = 0f;
    private int glideBoostDirection = 0;
    private bool wasGliding = false;
    
    private float wallJumpCancelTimer = 0f;
    private int lastWallJumpDir = 0;
    
    public Rigidbody2D Rigidbody => rb;
    public float DefaultGravityScale => defaultGravityScale;
    public bool WasGliding => wasGliding;
    public float GlideBoostTimeLeft => glideBoostTimeLeft;
    public bool HasWallJumpCancelActive => wallJumpCancelTimer > 0f;
    public int LastWallJumpDirection => lastWallJumpDir;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }
    
    void Update()
    {
        if (wallJumpCancelTimer > 0f) 
            wallJumpCancelTimer -= Time.deltaTime;
    }
    
    public void ApplyHorizontalMovement(float inputDirection, float multiplier = 1f)
    {
        float effectiveSpeed = speed * multiplier;
        float horizontal = inputDirection * effectiveSpeed;
        transform.Translate(Vector3.right * horizontal * Time.deltaTime);
    }
    
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }
    
    public void StartGlideBoost(float inputDirection)
    {
        glideBoostTimeLeft = initialGlideBoostDuration;
        glideBoostDirection = inputDirection != 0
            ? (int)Mathf.Sign(inputDirection)
            : (rb.linearVelocity.x > 0f ? 1 : rb.linearVelocity.x < 0f ? -1 : 0);
    }
    
    public void UpdateGlideBoost(float inputDirection, bool isGliding)
    {
        if (isGliding && glideBoostTimeLeft > 0f && inputDirection != 0 && 
            glideBoostDirection != 0 && Mathf.Sign(inputDirection) != glideBoostDirection)
        {
            glideBoostTimeLeft = 0f;
        }
        
        if (isGliding && glideBoostTimeLeft > 0f)
        {
            glideBoostTimeLeft -= Time.deltaTime;
        }
        
        if (!isGliding)
        {
            glideBoostTimeLeft = 0f;
            glideBoostDirection = 0;
        }
    }
    
    public float CalculateGlideMultiplier()
    {
        return glideHorizontalMultiplier * (glideBoostTimeLeft > 0f ? initialGlideBoostMultiplier : 1f);
    }
    
    public void ResetGlideBoost()
    {
        glideBoostTimeLeft = 0f;
        glideBoostDirection = 0;
        wasGliding = false;
    }
    
    public void SetWasGliding(bool value)
    {
        wasGliding = value;
    }
    
    public void StartWallJumpCancelWindow(int direction)
    {
        lastWallJumpDir = direction;
        wallJumpCancelTimer = wallJumpCancelDuration;
    }
    
    public bool TryCancelWallJump(float inputDirection)
    {
        if (wallJumpCancelTimer > 0f && inputDirection != 0 && lastWallJumpDir != 0 && 
            Mathf.Sign(inputDirection) != lastWallJumpDir)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            wallJumpCancelTimer = 0f;
            lastWallJumpDir = 0;
            return true;
        }
        return false;
    }
    
    public void ClearWallJumpState()
    {
        wallJumpCancelTimer = 0f;
        lastWallJumpDir = 0;
    }
    
    public void EnsureNoRotation()
    {
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0f;
    }
}