using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
[SerializeField] private float moveSpeed = 5f;
[SerializeField] private float sprintMultiplier = 1.5f;
[SerializeField] private float rotationLerpSpeed = 12f;
[SerializeField, Range(0f, 0.45f)] private float cameraBoundsPadding = 0.06f;
[SerializeField] private bool keepInsideCameraBounds = true;
[SerializeField] private float airMultiplier = 0.5f;

[Header("Animation Settings")]
[SerializeField] private Animator animator;
[SerializeField] private string walkBoolName = "isWalking";
[SerializeField] private float moveDeadzone = 0.1f;
[SerializeField] private float superJumpHoldTime = 0.4f;

[Header("Ability Unlocking")]
[SerializeField] private UnlockShooter unlockShooter;
[SerializeField] private UnlockSword unlockSword;
[SerializeField] private string shooterAbilityName = "Shooter";
[SerializeField] private string swordAbilityName = "Sword";
[SerializeField] private string superJumpAbilityName = "SuperJump";

[Header("Attack Combo")]
[SerializeField] private string attackFirstTrigger = "isAttackingFirst";
[SerializeField] private string attackSecondTrigger = "isAttackingSecond";
[SerializeField] private string comboActiveBool = "comboActive";
[SerializeField] private float comboWindow = 0.35f;
[SerializeField] private float holdRepeatInterval = 0.22f;

[Header("Misc")]
[SerializeField] private Compass compass;

// Cached components
private Rigidbody rb;
private PlayerJump playerJump;
private AbilityHolder abilityHolder;

// Movement runtime
private float baseMoveSpeed;
private Vector2 move;
private bool isSprinting;
private bool jumpRequested;

// Jump runtime
private bool jumpHeld;
private float jumpHoldTimer;

// Combat runtime
private bool shootHeld;
private bool attackHeld;
private float lastAttackTime = -999f;
private bool comboChainActive;
private int nextAttackIndex = 1;
private Coroutine attackRepeatRoutine;

// UI/game locks
private bool isInDialogue;
private bool isInMenu;

public bool isAnyAttackUnlocked => abilityHolder != null &&
                                   (abilityHolder.IsAbilityUnlockedByName(shooterAbilityName) ||
                                    abilityHolder.IsAbilityUnlockedByName(swordAbilityName));

    private void OnEnable()
    {
        GameEvent.OnDialogueInteractionStart += HandleDialogueStart;
        GameEvent.OnDialogueInteractionEnd += HandleDialogueEnd;

        UIEvent.OnPauseMenuOpened += HandlePauseMenuOpened;
        UIEvent.OnPauseMenuClosed += HandlePauseMenuClosed;
    }

    private void OnDisable()
    {
        GameEvent.OnDialogueInteractionStart -= HandleDialogueStart;
        GameEvent.OnDialogueInteractionEnd -= HandleDialogueEnd;

        UIEvent.OnPauseMenuOpened -= HandlePauseMenuOpened;
        UIEvent.OnPauseMenuClosed -= HandlePauseMenuClosed;
    }

    private void HandleDialogueStart()
    {
        isInDialogue = true;
        ApplyMoveSpeed();
    }

    private void HandleDialogueEnd()
    {
        isInDialogue = false;
        ApplyMoveSpeed();
    }

    private void HandlePauseMenuOpened()
    {
        isInMenu = true;
        ApplyMoveSpeed();
    }

    private void HandlePauseMenuClosed()
    {
        isInMenu = false;
        ApplyMoveSpeed();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isInDialogue && !isInMenu)
        {
            move = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isInDialogue || isInMenu)
        {
            return;
        }

        bool canUseSuperJump = CanUseSuperJump();
        
            if (context.started)
            {
                jumpHeld = canUseSuperJump;
                animator.SetBool("isWindingUp", canUseSuperJump);
                jumpHoldTimer = 0f;
            }

            if (context.canceled)
            {
                jumpHeld = false;
                animator.SetBool("isWindingUp", false);

                if (canUseSuperJump && jumpHoldTimer >= superJumpHoldTime)
                {
                    abilityHolder.TriggerAbilityByName(superJumpAbilityName);
                }
                else
                {
                    if (playerJump != null)
                    {
                        playerJump.QueueJump();
                    }
                }

        jumpHoldTimer = 0f;
        }

    }

    public void OnSuperJump(InputAction.CallbackContext context)
    {
        if (!isInDialogue && !isInMenu)
        {
            if (context.performed && abilityHolder != null)
            {
                abilityHolder.TriggerAbilityByName(superJumpAbilityName);

            }
        }
    }

    public void OnTongue(InputAction.CallbackContext context)
    {
        if (!isInDialogue && !isInMenu)
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
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!isInDialogue && !isInMenu)
        {
            if (!isAnyAttackUnlocked || animator == null || UI_Manager.uiManager.noteBookUIDisplaying)
            {
                return;
            }

            if (context.started)
            {
                attackHeld = true;
                FireNextAttack();

                if (attackRepeatRoutine != null)
                {
                    StopCoroutine(attackRepeatRoutine);
                }

                attackRepeatRoutine = StartCoroutine(AttackRepeatLoop());
            }

            if (context.canceled)
            {
                attackHeld = false;

                if (attackRepeatRoutine != null)
                {
                    StopCoroutine(attackRepeatRoutine);
                    attackRepeatRoutine = null;
                }

                ResetComboState();
            }
        }
        
    }

    public void OnCompass(InputAction.CallbackContext context)
    {
        if (!isInDialogue && !isInMenu)
        {
            if (context.started)
            {
                if (compass != null)
                {
                    compass.ActivateCompass();
                }
            }

            if (context.canceled)
            {
                if (compass != null)
                {
                    compass.DeactivateCompass();
                }
            }
        }
    }

    // Animation Event hook: call this from attack clips when the projectile should fire.
    public void FireShooterFromAnimationEvent()
    {
        if (abilityHolder == null)
        {
            return;
        }

        abilityHolder.TriggerAbilityByName(shooterAbilityName);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) isSprinting = true;
        if (context.canceled) isSprinting = false;

        ApplyMoveSpeed();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerJump = GetComponent<PlayerJump>();
        abilityHolder = GetComponent<AbilityHolder>();

        baseMoveSpeed = moveSpeed;
        ApplyMoveSpeed();
        if (compass == null)
        {
            compass = Object.FindFirstObjectByType<Compass>(FindObjectsInactive.Include);
        }
        ResolveAbilityUnlockReferences();
        
        DontDestroyOnLoad(gameObject); //Makes player persistant across scenes
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInDialogue && !isInMenu)
        {
            if (jumpHeld)
            {
                jumpHoldTimer += Time.deltaTime;
            }

            if (shootHeld && abilityHolder != null)
            {
                abilityHolder.TriggerAbilityByName("Shooter");
            }

            if (comboChainActive && !attackHeld && Time.time - lastAttackTime > comboWindow)
            {
                ResetComboState();
            }

            UpdateWalkAnimation();
        }
    }

    void FixedUpdate()
    {
        if (!isInDialogue && !isInMenu)
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
    }

    private void ApplyMoveSpeed()
    {
        bool canMove = !isInDialogue && !isInMenu;
        moveSpeed = baseMoveSpeed * ((isSprinting && canMove) ? sprintMultiplier : 1f);
    }

    public void MovePlayer()
    {
        if(rb == null || Camera.main == null) { return; }
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

    private void ResolveAbilityUnlockReferences()
    {
        if (unlockShooter == null)
        {
            unlockShooter = Object.FindFirstObjectByType<UnlockShooter>(FindObjectsInactive.Include);
        }

        if (unlockSword == null)
        {
            unlockSword = Object.FindFirstObjectByType<UnlockSword>(FindObjectsInactive.Include);
        }
    }

    public void SetAbilityUnlockReferences(UnlockShooter shooterUnlock, UnlockSword swordUnlock)
    {
        unlockShooter = shooterUnlock;
        unlockSword = swordUnlock;
    }

    private void FireNextAttack()
    {
        if (animator == null)
        {
            return;
        }

        // If too much time passed, restart combo from attack 1.
        if (Time.time - lastAttackTime > comboWindow)
        {
            nextAttackIndex = 1;
        }

        animator.SetBool(comboActiveBool, true);

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

        lastAttackTime = Time.time;
        comboChainActive = true;
    }

    private void ResetComboState()
    {
        if (animator != null)
        {
            animator.ResetTrigger(attackFirstTrigger);
            animator.ResetTrigger(attackSecondTrigger);
            animator.SetBool(comboActiveBool, false);
        }

        comboChainActive = false;
    nextAttackIndex = 1;
    }

    private IEnumerator AttackRepeatLoop()
    {
        while (attackHeld)
        {
            yield return new WaitForSeconds(holdRepeatInterval);

            if (!attackHeld)
            {
                yield break;
            }

            FireNextAttack();
        }

        attackRepeatRoutine = null;
    }

    private bool CanUseSuperJump()
    {
        return abilityHolder != null && abilityHolder.IsAbilityUnlockedByName(superJumpAbilityName);
    }
}
