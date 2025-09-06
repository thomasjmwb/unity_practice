using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public float glideGravityScale = 0.3f; // How strong gravity is during glide (lower = slower fall)
    private float defaultGravityScale;     // Cached to restore normal gravity
    public float glideHorizontalMultiplier = 1.2f; // Extra horizontal speed while gliding

    // Double-jump settings
    public int maxAirJumps = 2;            // Number of extra jumps allowed while airborne
    private int airJumpsUsed = 0;
    private bool isGrounded = false;

    // Ground check
    public Transform groundCheck;          // Assign a child Transform at the player's feet
    public float groundCheckRadius = 0.1f; // Small radius for ground detection
    public LayerMask groundLayer;          // Set this to your ground layer in the Inspector

    // Initial glide boost
    public float initialGlideBoostDuration = 0.75f;   // Duration of the initial glide boost
    public float initialGlideBoostMultiplier = 2f;    // Multiplier applied on top of glideHorizontalMultiplier
    private float glideBoostTimeLeft = 0f;
    private bool wasGliding = false;
    private int glideBoostDirection = 0; // -1 left, 1 right, 0 none
    public float wallJumpHorizontalImpulse = 4.5f;
    public float wallJumpVerticalImpulse = 7.5f;

    // Prevent gliding right after a wall jump
    public float glideLockoutAfterWallJump = 0.4f; // duration you cannot glide after wall jump
    private float glideLockoutTimer = 0f;

    // Cancel wall-jump horizontal velocity if direction changes shortly after wall jump
    public float wallJumpCancelDuration = 0.5f; // how long we can cancel the wall-jump push by reversing input
    private float wallJumpCancelTimer = 0f;
    private int lastWallJumpDir = 0; // -1 from right wall (pushing left), +1 from left wall (pushing right)

    // Contact-based state (computed each physics step)
    private bool touchingGroundBelow = false;
    private bool touchingWallLeft = false;
    private bool touchingWallRight = false;
    private bool prevGrounded = false;

    // Wall-jump grace (coyote) and input buffers
    public float wallCoyoteTime = 0.12f;       // time after leaving wall you can still wall-jump
    public float jumpInputBufferTime = 0.12f;  // time to buffer jump input
    public float awayInputBufferTime = 0.12f;  // time to buffer "away" directional input
    private float wallLeftCoyoteTimer = 0f;
    private float wallRightCoyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private float leftAwayTimer = 0f;   // last time player pressed left (away from right wall)
    private float rightAwayTimer = 0f;  // last time player pressed right (away from left wall)

    void Start()
    {
        defaultGravityScale = rb.gravityScale;
    }

    void FixedUpdate()
    {
        // Reset contact state; will be set during OnCollisionStay2D in this physics step
        touchingGroundBelow = false;
        touchingWallLeft = false;
        touchingWallRight = false;

        rb.angularVelocity = 0f; // Ensure no angular motion accumulates
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Evaluate each contact relative to player position and normal
        foreach (var cp in collision.contacts)
        {
            Vector2 toContact = cp.point - (Vector2)transform.position;
            Vector2 n = cp.normal;

            // Ground: contact is below player and surface normal points up
            if (toContact.y < -0.05f && n.y > 0.5f)
            {
                touchingGroundBelow = true;
            }

            // Left wall: contact is left of player and normal points right (mostly horizontal)
            if (toContact.x < -0.05f && n.x > 0.5f && Mathf.Abs(n.y) < 0.9f)
            {
                touchingWallLeft = true;
                wallLeftCoyoteTimer = wallCoyoteTime;  // refresh coyote time while we have contact
            }

            // Right wall: contact is right of player and normal points left (mostly horizontal)
            if (toContact.x > 0.05f && n.x < -0.5f && Mathf.Abs(n.y) < 0.9f)
            {
                touchingWallRight = true;
                wallRightCoyoteTimer = wallCoyoteTime; // refresh coyote time while we have contact
            }
        }
    }

    void Update()
    {
        // Grounded based strictly on ground-under-player contacts from physics
        isGrounded = touchingGroundBelow;

        // Reset air jumps only on landing transition
        if (isGrounded && !prevGrounded)
        {
            airJumpsUsed = 0;
            // Optional: Debug.Log("[Ground] Landed: airJumps reset.");
        }

        // Timers
        if (glideLockoutTimer > 0f) glideLockoutTimer -= Time.deltaTime;
        if (wallJumpCancelTimer > 0f) wallJumpCancelTimer -= Time.deltaTime;
        if (wallLeftCoyoteTimer > 0f) wallLeftCoyoteTimer -= Time.deltaTime;
        if (wallRightCoyoteTimer > 0f) wallRightCoyoteTimer -= Time.deltaTime;
        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
        if (leftAwayTimer > 0f) leftAwayTimer -= Time.deltaTime;
        if (rightAwayTimer > 0f) rightAwayTimer -= Time.deltaTime;

        // Clear some timers on ground
        if (isGrounded)
        {
            wallJumpCancelTimer = 0f;
            lastWallJumpDir = 0;
            wallLeftCoyoteTimer = 0f;
            wallRightCoyoteTimer = 0f;
        }

        // Buffer jump input (W = jump)
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferTimer = jumpInputBufferTime;
        }

        // Buffer "away" direction inputs
        if (Input.GetKeyDown(KeyCode.A)) leftAwayTimer = awayInputBufferTime;
        if (Input.GetKeyDown(KeyCode.D)) rightAwayTimer = awayInputBufferTime;

        // Treat currently held inputs as active too
        bool leftAwayActive = leftAwayTimer > 0f || Input.GetKey(KeyCode.A);
        bool rightAwayActive = rightAwayTimer > 0f || Input.GetKey(KeyCode.D);

        // Wall-jump using buffers (prioritized over all other jumps)
        bool didWallJump = false;
        if (jumpBufferTimer > 0f && !isGrounded)
        {
            bool canUseLeft = touchingWallLeft || wallLeftCoyoteTimer > 0f;
            bool canUseRight = touchingWallRight || wallRightCoyoteTimer > 0f;

            if (canUseLeft && rightAwayActive)
            {
                WallJump(1);
                didWallJump = true;
            }
            else if (canUseRight && leftAwayActive)
            {
                WallJump(-1);
                didWallJump = true;
            }

            if (didWallJump)
            {
                jumpBufferTimer = 0f;  // consume jump buffer
                leftAwayTimer = 0f;    // optional: consume away buffers
                rightAwayTimer = 0f;
            }
        }

        // If buffered jump remains and we didn't wall-jump, use it for ground/air jumps
        if (!didWallJump && jumpBufferTimer > 0f)
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector3(0, speed), ForceMode2D.Impulse);
                jumpBufferTimer = 0f;
            }
            else if (airJumpsUsed < maxAirJumps)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(new Vector3(0, speed), ForceMode2D.Impulse);
                airJumpsUsed++;
                jumpBufferTimer = 0f;
            }
            // If neither is possible, keep buffer alive until it expires naturally
        }

        // Determine if we are gliding (Space to glide) — blocked during wall-jump lockout
        bool glideInput = Input.GetKey(KeyCode.Space);
        bool isGliding = rb.linearVelocity.y < 0f && glideInput && glideLockoutTimer <= 0f;

        // Read current horizontal input direction (-1, 0, 1) for movement
        int inputSign = 0;
        if (Input.GetKey(KeyCode.A)) inputSign = -1;
        else if (Input.GetKey(KeyCode.D)) inputSign = 1;

        // If within the cancel window and player presses opposite direction, cancel wall-jump horizontal velocity
        if (wallJumpCancelTimer > 0f && inputSign != 0 && lastWallJumpDir != 0 && inputSign != lastWallJumpDir)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            wallJumpCancelTimer = 0f; // cancel only once per wall jump
            lastWallJumpDir = 0;
        }

        // Handle initial glide boost timing and direction
        if (isGliding && !wasGliding)
        {
            glideBoostTimeLeft = initialGlideBoostDuration;
            glideBoostDirection = inputSign != 0
                ? inputSign
                : (rb.linearVelocity.x > 0f ? 1 : rb.linearVelocity.x < 0f ? -1 : 0);
        }

        // Cut the boost if player changes direction during the boost window
        if (isGliding && glideBoostTimeLeft > 0f && inputSign != 0 && glideBoostDirection != 0 && inputSign != glideBoostDirection)
        {
            glideBoostTimeLeft = 0f;
        }

        // Tick down boost while gliding; reset when not gliding
        if (isGliding && glideBoostTimeLeft > 0f)
        {
            glideBoostTimeLeft -= Time.deltaTime;
        }
        if (!isGliding)
        {
            glideBoostTimeLeft = 0f;
            glideBoostDirection = 0;
        }

        float glideMult = isGliding ? glideHorizontalMultiplier * (glideBoostTimeLeft > 0f ? initialGlideBoostMultiplier : 1f) : 1f;
        float effectiveSpeed = speed * glideMult;

        float horizontal = inputSign * effectiveSpeed;
        transform.Translate(new Vector3(horizontal * Time.deltaTime, 0, 0));

        if (isGliding)
        {
            rb.gravityScale = glideGravityScale;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }

        // Ensure player never visually rotates
        transform.rotation = Quaternion.identity;

        wasGliding = isGliding;
        prevGrounded = isGrounded;
    }

    private void WallJump(int directionAwayFromWall)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(new Vector2(wallJumpHorizontalImpulse * directionAwayFromWall, wallJumpVerticalImpulse), ForceMode2D.Impulse);

        // Cancel any glide and start lockout window
        glideBoostTimeLeft = 0f;
        glideBoostDirection = 0;
        wasGliding = false;
        glideLockoutTimer = glideLockoutAfterWallJump;

        // Start wall-jump cancel window
        lastWallJumpDir = directionAwayFromWall;
        wallJumpCancelTimer = wallJumpCancelDuration;
    }
    // ... existing code ...
}
