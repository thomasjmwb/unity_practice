using UnityEngine;

public class PlayerJumping : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public int maxAirJumps = 2;
    public float wallJumpHorizontalImpulse = 4.5f;
    public float wallJumpVerticalImpulse = 7.5f;
    public float glideLockoutAfterWallJump = 0.4f;
    
    private int airJumpsUsed = 0;
    private float glideLockoutTimer = 0f;
    private Rigidbody2D rb;
    
    public int AirJumpsUsed => airJumpsUsed;
    public bool IsGlideLocked => glideLockoutTimer > 0f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (glideLockoutTimer > 0f) 
            glideLockoutTimer -= Time.deltaTime;
    }
    
    public bool CanGroundJump(bool isGrounded)
    {
        return isGrounded;
    }
    
    public bool CanAirJump()
    {
        return airJumpsUsed < maxAirJumps;
    }
    
    public bool CanWallJump(bool touchingWall)
    {
        return touchingWall;
    }
    
    public void PerformGroundJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    public void PerformAirJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        airJumpsUsed++;
    }
    
    public void PerformWallJump(int directionAwayFromWall)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(new Vector2(wallJumpHorizontalImpulse * directionAwayFromWall, wallJumpVerticalImpulse), ForceMode2D.Impulse);
        
        glideLockoutTimer = glideLockoutAfterWallJump;
        
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetGlideBoost();
            movement.StartWallJumpCancelWindow(directionAwayFromWall);
        }
    }
    
    public void ResetAirJumps()
    {
        airJumpsUsed = 0;
    }
    
    public void ClearGlideLockout()
    {
        glideLockoutTimer = 0f;
    }
}