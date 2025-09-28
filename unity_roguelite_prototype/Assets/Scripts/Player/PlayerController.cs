using UnityEngine;

/// <summary>
/// Handles basic player movement, jumping with coyote time and jump buffering, and a short dash.
/// This script does not implement ground detection – assign your own logic to the IsGrounded() method.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;

    [Header("Advanced Jump Timing")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;

    private Rigidbody2D rb;
    private bool isDashing;
    private float dashTimeRemaining;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from InputManager or fallback to legacy input
        float moveInput = 0f;
        bool jumpInput = false;
        bool dashInput = false;

        if (InputManager.Instance != null)
        {
            moveInput = InputManager.Instance.MoveInput.x;
            jumpInput = InputManager.Instance.JumpPressed;
            dashInput = InputManager.Instance.DashPressed;
        }
        else
        {
            // Fallback to legacy input system
            moveInput = Input.GetAxisRaw("Horizontal");
            jumpInput = Input.GetButtonDown("Jump");
            dashInput = Input.GetButtonDown("Fire3");
        }

        // Start dash
        if (dashInput)
        {
            StartDash();
        }

        // Jump buffer: record jump input
        if (jumpInput)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Update coyote timer when grounded
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // Execute jump if within coyote and buffer windows
        if (coyoteCounter > 0f && jumpBufferCounter > 0f)
        {
            Jump();
        }

        // Flip sprite to face movement direction
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // Horizontal movement (disabled during dash)
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        // Handle dash movement
        if (isDashing)
        {
            if (dashTimeRemaining > 0f)
            {
                rb.linearVelocity = new Vector2((facingRight ? 1f : -1f) * dashSpeed, 0f);
                dashTimeRemaining -= Time.fixedDeltaTime;
            }
            else
            {
                isDashing = false;
            }
        }
    }

    void StartDash()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}