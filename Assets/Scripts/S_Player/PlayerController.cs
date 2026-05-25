using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationLerpSpeed = 12f;
    private Vector2 move;
    private Rigidbody rb;

    [Header("Animation")]
    [SerializeField] private string walkBoolName = "isWalking";
    [SerializeField] private float moveDeadzone = 0.1f;

    [SerializeField] private float superJumpHoldTime = 0.4f;
    private bool jumpHeld;
    private float jumpHoldTimer;
    private PlayerJump playerJump;
    private AbilityHolder abilityHolder;
    [SerializeField] private float airMultiplier = 0.5f;
    [SerializeField] Animator animator;
    private bool jumpRequested;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpHeld = true;
            animator.SetBool("isWindingUp", true);
            jumpHoldTimer = 0f;
        }
    
        if (context.canceled)
        {
            jumpHeld = false;
            animator.SetBool("isWindingUp", false);
            animator.SetBool("isJumping", true);
            if (jumpHoldTimer >= superJumpHoldTime)
            {
                abilityHolder.TriggerAbilityByName("SuperJump");
                
            }
            else
            {
                TryJump(); 
            }
            jumpHoldTimer = 0f;
        }
        else
        {
         animator.SetBool("isJumping", false);
        }

    }

    public void OnSuperJump(InputAction.CallbackContext context)
    {
        if (context.performed && abilityHolder != null)
        {
            abilityHolder.TriggerAbilityByName("SuperJump");

        }
    }

    public void OnTongue(InputAction.CallbackContext context)
    {
        if (context.performed && abilityHolder != null)
        {
            abilityHolder.TriggerAbilityByName("Tongue");
            animator.SetBool("isToungeMove", true);
        }
        else
        {
        animator.SetBool("isToungeMove", false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerJump = GetComponent<PlayerJump>();
        abilityHolder = GetComponent<AbilityHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }
        UpdateWalkAnimation();    
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        MovePlayer();

        if (jumpRequested)
        {
            TryJump();
            jumpRequested = false;
        }
    }

    public void MovePlayer()
    {
        float controlMultiplier = GetAirControlMultiplier();
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationLerpSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
             animator.SetBool("isWalking", true);
        }
        else
        {
            rb.angularVelocity = Vector3.zero; // Stop rotation when no input
            animator.SetBool("isWalking", false);
        }

        rb.MovePosition(rb.position + moveDirection * moveSpeed * controlMultiplier * Time.fixedDeltaTime);
    }

    public void TryJump()
    {
        if (playerJump != null)
        {
            playerJump.Jump();
        }
    }

    public float GetAirControlMultiplier()
    {
        if (playerJump != null && !playerJump.IsGrounded)
        {
            return airMultiplier;
        }
        return 1f;
    }
    private void UpdateWalkAnimation()
    {
        if (animator == null || playerJump == null)
        {
            return;
        }

        bool isGrounded = playerJump.IsGrounded;
        bool hasMoveInput = move.sqrMagnitude > (moveDeadzone * moveDeadzone);

        animator.SetBool(walkBoolName, isGrounded && hasMoveInput);
}
}
