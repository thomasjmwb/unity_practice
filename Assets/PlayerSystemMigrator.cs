using UnityEngine;

[System.Serializable]
public class OldPlayerSettings
{
    public float speed = 5f;
    public float glideGravityScale = 0.3f;
    public float glideHorizontalMultiplier = 1.2f;
    public int maxAirJumps = 2;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public float initialGlideBoostDuration = 0.75f;
    public float initialGlideBoostMultiplier = 2f;
    public float wallJumpHorizontalImpulse = 4.5f;
    public float wallJumpVerticalImpulse = 7.5f;
    public float glideLockoutAfterWallJump = 0.4f;
    public float wallJumpCancelDuration = 0.5f;
    public float wallCoyoteTime = 0.12f;
    public float jumpInputBufferTime = 0.12f;
    public float awayInputBufferTime = 0.12f;
}

public class PlayerSystemMigrator : MonoBehaviour
{
    [Header("Migration Settings")]
    public OldPlayerSettings oldSettings = new OldPlayerSettings();
    
    [Header("Auto-Migrate")]
    [SerializeField] private bool autoMigrateOnStart = true;
    
    void Start()
    {
        if (autoMigrateOnStart)
        {
            MigratePlayerSystem();
        }
    }
    
    [ContextMenu("Migrate Player System")]
    public void MigratePlayerSystem()
    {
        Debug.Log("Starting player system migration...");
        
        // Remove old player component if it exists
        var oldPlayerComponent = GetComponent<player>();
        if (oldPlayerComponent != null)
        {
            Debug.Log("Removing old player component...");
            DestroyImmediate(oldPlayerComponent);
        }
        
        // Add PlayerController if it doesn't exist
        var playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.Log("Adding PlayerController...");
            playerController = gameObject.AddComponent<PlayerController>();
        }
        
        // Add and configure PlayerMovement
        var movement = GetComponent<PlayerMovement>();
        if (movement == null)
        {
            Debug.Log("Adding PlayerMovement...");
            movement = gameObject.AddComponent<PlayerMovement>();
        }
        ConfigureMovement(movement);
        
        // Add and configure PlayerJumping
        var jumping = GetComponent<PlayerJumping>();
        if (jumping == null)
        {
            Debug.Log("Adding PlayerJumping...");
            jumping = gameObject.AddComponent<PlayerJumping>();
        }
        ConfigureJumping(jumping);
        
        // Add and configure PlayerCollisionDetector
        var collisionDetector = GetComponent<PlayerCollisionDetector>();
        if (collisionDetector == null)
        {
            Debug.Log("Adding PlayerCollisionDetector...");
            collisionDetector = gameObject.AddComponent<PlayerCollisionDetector>();
        }
        ConfigureCollisionDetector(collisionDetector);
        
        // Add and configure PlayerInputBuffer
        var inputBuffer = GetComponent<PlayerInputBuffer>();
        if (inputBuffer == null)
        {
            Debug.Log("Adding PlayerInputBuffer...");
            inputBuffer = gameObject.AddComponent<PlayerInputBuffer>();
        }
        ConfigureInputBuffer(inputBuffer);
        
        // Add and configure PlayerInput
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.Log("Adding PlayerInput...");
            playerInput = gameObject.AddComponent<PlayerInput>();
        }
        
        // Refresh PlayerController component references after all components are added
        if (playerController != null)
        {
            playerController.RefreshComponentReferences();
            Debug.Log("Refreshed PlayerController component references.");
        }
        
        Debug.Log("Player system migration completed successfully!");
        
        // Destroy this migrator component as it's no longer needed
        DestroyImmediate(this);
    }
    
    private void ConfigureMovement(PlayerMovement movement)
    {
        movement.speed = oldSettings.speed;
        movement.glideGravityScale = oldSettings.glideGravityScale;
        movement.glideHorizontalMultiplier = oldSettings.glideHorizontalMultiplier;
        movement.initialGlideBoostMultiplier = oldSettings.initialGlideBoostMultiplier;
        movement.initialGlideBoostDuration = oldSettings.initialGlideBoostDuration;
        movement.wallJumpCancelDuration = oldSettings.wallJumpCancelDuration;
    }
    
    private void ConfigureJumping(PlayerJumping jumping)
    {
        jumping.jumpForce = oldSettings.speed;
        jumping.maxAirJumps = oldSettings.maxAirJumps;
        jumping.wallJumpHorizontalImpulse = oldSettings.wallJumpHorizontalImpulse;
        jumping.wallJumpVerticalImpulse = oldSettings.wallJumpVerticalImpulse;
        jumping.glideLockoutAfterWallJump = oldSettings.glideLockoutAfterWallJump;
    }
    
    private void ConfigureCollisionDetector(PlayerCollisionDetector collisionDetector)
    {
        collisionDetector.groundLayer = oldSettings.groundLayer;
        collisionDetector.groundCheckRadius = oldSettings.groundCheckRadius;
        collisionDetector.groundCheck = oldSettings.groundCheck;
    }
    
    private void ConfigureInputBuffer(PlayerInputBuffer inputBuffer)
    {
        inputBuffer.jumpBufferTime = oldSettings.jumpInputBufferTime;
        inputBuffer.wallCoyoteTime = oldSettings.wallCoyoteTime;
        inputBuffer.awayInputBufferTime = oldSettings.awayInputBufferTime;
    }
}