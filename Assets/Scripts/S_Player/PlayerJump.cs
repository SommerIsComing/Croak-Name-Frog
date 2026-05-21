using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 0.25f;

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
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, groundLayer);
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    public void Jump()
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;

            // Calculate jump direction
            Vector3 jumpDirection = Vector3.up;

            // Apply jump force
            rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

            // Start cooldown
            Invoke(nameof(ResetJump), jumpCooldown);
            animator.SetBool("isJumping", true);
            }
         else
        {
            animator.SetBool("isJumping", false);
        }

     
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
