using UnityEngine;

[System.Obsolete("This script has been replaced by the modular PlayerController system. Please use PlayerController instead.")]
public class player : MonoBehaviour
{
    [Header("Legacy Settings - Will be migrated automatically")]
    public Rigidbody2D rb;
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
    
    void Start()
    {
        Debug.LogWarning("The 'player' script is obsolete. Automatically migrating to the new PlayerController system...");
        
        var playerController = gameObject.GetComponent<PlayerController>();
        if (playerController == null)
        {
            // Add the migrator component and let it handle the migration
            var migrator = gameObject.AddComponent<PlayerSystemMigrator>();
            
            // Copy settings to migrator
            migrator.oldSettings.speed = speed;
            migrator.oldSettings.glideGravityScale = glideGravityScale;
            migrator.oldSettings.glideHorizontalMultiplier = glideHorizontalMultiplier;
            migrator.oldSettings.maxAirJumps = maxAirJumps;
            migrator.oldSettings.groundCheck = groundCheck;
            migrator.oldSettings.groundCheckRadius = groundCheckRadius;
            migrator.oldSettings.groundLayer = groundLayer;
            migrator.oldSettings.initialGlideBoostDuration = initialGlideBoostDuration;
            migrator.oldSettings.initialGlideBoostMultiplier = initialGlideBoostMultiplier;
            migrator.oldSettings.wallJumpHorizontalImpulse = wallJumpHorizontalImpulse;
            migrator.oldSettings.wallJumpVerticalImpulse = wallJumpVerticalImpulse;
            migrator.oldSettings.glideLockoutAfterWallJump = glideLockoutAfterWallJump;
            migrator.oldSettings.wallJumpCancelDuration = wallJumpCancelDuration;
            migrator.oldSettings.wallCoyoteTime = wallCoyoteTime;
            migrator.oldSettings.jumpInputBufferTime = jumpInputBufferTime;
            migrator.oldSettings.awayInputBufferTime = awayInputBufferTime;
            
            // Trigger migration immediately
            migrator.MigratePlayerSystem();
        }
        else
        {
            Debug.Log("PlayerController already exists. Removing obsolete player component.");
            DestroyImmediate(this);
        }
    }
}
