using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationLerpSpeed = 12f;
    [SerializeField, Range(0f, 0.45f)] private float cameraBoundsPadding = 0.06f;
    [SerializeField] private bool keepInsideCameraBounds = true;
    [SerializeField] private float sprintMultiplier = 1.5f;
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
    private bool shootHeld;

    [Header("Ability Unlocking")]
    [SerializeField] private UnlockShooter unlockShooter;
    [SerializeField] private UnlockSword unlockSword;
    public bool isAnyAttackUnlocked => (unlockShooter != null && unlockShooter.shooterIsUnlocked) || (unlockSword != null && unlockSword.swordIsUnlocked);

    [Header("Attack Combo")]
    [SerializeField] private string attackFirstTrigger = "isAttackingFirst";
    [SerializeField] private string attackSecondTrigger = "isAttackingSecond";
    [SerializeField] private float comboWindow = 0.35f;
    [SerializeField] private float holdRepeatInterval = 0.22f;
    private bool attackHeld;
    private float lastAttackTime = -999f;
    private float nextAttackTime;
    private int nextAttackIndex = 1;

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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!isAnyAttackUnlocked || animator == null)
        {
            return;
        }

        if (context.started)
        {
            attackHeld = true;
            FireNextAttack();
        }

        if (context.canceled)
        {
            attackHeld = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            moveSpeed *= sprintMultiplier;
        }
        if (context.canceled)
        {
            moveSpeed /= sprintMultiplier;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerJump = GetComponent<PlayerJump>();
        abilityHolder = GetComponent<AbilityHolder>();
        
        DontDestroyOnLoad(gameObject); //Makes player persistant across scenes
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }

        if (shootHeld && abilityHolder != null)
        {
            abilityHolder.TriggerAbilityByName("Shooter");
        }

        UpdateWalkAnimation();   

        if (attackHeld && Time.time >= nextAttackTime)
        {
            FireNextAttack();
        } 
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        MovePlayer();
        KeepPlayerInsideCameraBounds();

        if (jumpRequested)
        {
            TryJump();
            jumpRequested = false;
        }
    }

    public void MovePlayer()
    {
        float controlMultiplier = GetAirControlMultiplier();

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * move.y + right * move.x;


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

    private void KeepPlayerInsideCameraBounds()
    {
        if (!keepInsideCameraBounds)
        {
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 viewportPos = cam.WorldToViewportPoint(rb.position);
        if (viewportPos.z <= 0f)
        {
            return;
        }

        float min = cameraBoundsPadding;
        float max = 1f - cameraBoundsPadding;

        float clampedX = Mathf.Clamp(viewportPos.x, min, max);
        float clampedY = Mathf.Clamp(viewportPos.y, min, max);

        if (Mathf.Approximately(clampedX, viewportPos.x) && Mathf.Approximately(clampedY, viewportPos.y))
        {
            return;
        }

        Vector3 clampedWorld = cam.ViewportToWorldPoint(new Vector3(clampedX, clampedY, viewportPos.z));
        clampedWorld.y = rb.position.y;
        rb.MovePosition(clampedWorld);
    }

    private void FireNextAttack()
    {
        // If too much time passed, restart combo from attack 1.
        if (Time.time - lastAttackTime > comboWindow)
        {
            nextAttackIndex = 1;
        }

        if (nextAttackIndex == 1)
        {
            animator.SetTrigger(attackFirstTrigger);
            nextAttackIndex = 2;
        }
        else
        {
            animator.SetTrigger(attackSecondTrigger);
            nextAttackIndex = 1;
        }

        // Optional: if you still want hold-shot behavior, call it here.
        if (abilityHolder != null)
        {
            abilityHolder.TriggerAbilityByName("Shooter");
        }

        lastAttackTime = Time.time;
        nextAttackTime = Time.time + holdRepeatInterval;
    }
}
