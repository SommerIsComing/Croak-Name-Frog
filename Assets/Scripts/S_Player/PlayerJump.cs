using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float coyoteTime = 0.2f;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    bool readyToJump;
    

    [Header("Gravity Settings")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 20f;
    public float fallSpeedMultiplier = 2f; 
    public float maxLaunchSpeed = 15f;

    [Header("GroundCheck")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool grounded;
    [SerializeField] private float groundCheckOffset = 0.1f;
    public float groundDrag = 6f;
    public bool IsGrounded => grounded;
    public bool gravityEnabled = true;

    [SerializeField] Animator animator;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool rawGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundCheckOffset, groundLayer);
        grounded = rawGrounded && rb.linearVelocity.y <= 0.05f;

        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

         if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        Gravity();
        if (readyToJump && coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            PerformJump();
        }
    }

    public void QueueJump()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    public void Jump()
    {
        QueueJump();
    }

    public void PerformJump()
    {
        if (!readyToJump)
        {
            return;
        }

        readyToJump = false;

        // This prevents "jump just slows my fall" feeling
        Vector3 v = rb.linearVelocity;
        if (v.y < 0f)
        {
            v.y = 0f;
            rb.linearVelocity = v;
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;

        Invoke(nameof(ResetJump), jumpCooldown);
        animator.SetBool("isWalking", false);
        animator.SetBool("isJumping", true);
    }

    private void ResetJump()
    {
        readyToJump = true;
        animator.SetBool("isJumping", false);
    }

    private void Gravity()
    {
        if (!gravityEnabled) return;

        float gravityMultiplier = baseGravity;

        if (!grounded && rb.linearVelocity.y <= 0.05f)
        {
            animator.SetBool("isJumping", false);
        }

        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f) // Near the peak of the jump
        {
            gravityMultiplier = baseGravity * fallSpeedMultiplier; // Stronger gravity at the peak
        }
        else if (rb.linearVelocity.y < 0) // Falling
        {
            gravityMultiplier = baseGravity * fallSpeedMultiplier; // Increase gravity when falling
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed), rb.linearVelocity.z);
        }
        else if (rb.linearVelocity.y > maxLaunchSpeed) // Clamp upward velocity
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxLaunchSpeed, rb.linearVelocity.z);
        }

        if (gravityMultiplier != 1f)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }

        }
    
}
