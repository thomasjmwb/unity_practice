using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;
    public Transform groundCheck;
    
    private bool isGrounded = false;
    private bool touchingWallLeft = false;
    private bool touchingWallRight = false;
    private bool prevGrounded = false;
    
    public bool IsGrounded => isGrounded;
    public bool TouchingWallLeft => touchingWallLeft;
    public bool TouchingWallRight => touchingWallRight;
    public bool PrevGrounded => prevGrounded;
    public bool JustLanded => isGrounded && !prevGrounded;
    
    void FixedUpdate()
    {
        ResetCollisionState();
    }
    
    void LateUpdate()
    {
        prevGrounded = isGrounded;
    }
    
    private void ResetCollisionState()
    {
        touchingWallLeft = false;
        touchingWallRight = false;
        isGrounded = false;
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var cp in collision.contacts)
        {
            Vector2 toContact = cp.point - (Vector2)transform.position;
            Vector2 n = cp.normal;
            
            if (toContact.y < -0.05f && n.y > 0.5f)
            {
                isGrounded = true;
            }
            
            if (toContact.x < -0.05f && n.x > 0.5f && Mathf.Abs(n.y) < 0.9f)
            {
                touchingWallLeft = true;
            }
            
            if (toContact.x > 0.05f && n.x < -0.5f && Mathf.Abs(n.y) < 0.9f)
            {
                touchingWallRight = true;
            }
        }
    }
}