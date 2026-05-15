using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationLerpSpeed = 12f;
    private Vector2 move, mouseLook, joystickLook;
    private Vector3 rotationTarget;
    private Rigidbody rb;


    [SerializeField] private float superJumpHoldTime = 0.4f;
    private bool jumpHeld;
    private float jumpHoldTimer;
    private PlayerJump playerJump;
    private AbilityHolder abilityHolder;
    [SerializeField] private float airMultiplier = 0.5f;
    private bool jumpRequested;
    public bool isPc;
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpHeld = true;
            jumpHoldTimer = 0f;
        }

        if (context.canceled)
        {
            jumpHeld = false;
            if (jumpHoldTimer >= superJumpHoldTime)
            {
                TriggerSuperJump();
            }
            else
            {
                TryJump();
            }

            jumpHoldTimer = 0f;
        }
    }

    public void OnMouseLook(InputAction.CallbackContext context)
    {
        mouseLook = context.ReadValue<Vector2>();
    }

    public void OnJoystickLook(InputAction.CallbackContext context)
    {
        joystickLook = context.ReadValue<Vector2>();
    }

    public void OnSuperJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TriggerSuperJump();
        }
    }

    public void OnToungeSwing(InputAction.CallbackContext context)
    {
        if (context.performed && abilityHolder != null)
        {
            abilityHolder.TriggerAbility();
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
        if (isPc)
        {
            if (Camera.main != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(mouseLook);

                if (Physics.Raycast(ray, out hit))
                {
                    rotationTarget = hit.point;
                }
            }
        }
        if (jumpHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (isPc)
        {
            MovePlayerWithAim();
        }
        else
        {
            if (joystickLook.x == 0 && joystickLook.y == 0)
            {
                MovePlayer();
            }
            else
            {
                MovePlayerWithAim();
            }
        }

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
        }

        rb.MovePosition(rb.position + moveDirection * moveSpeed * controlMultiplier * Time.fixedDeltaTime);
    }

    public void MovePlayerWithAim()
    {
        if (isPc)
        {
            var lookPos = rotationTarget - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookPos);
                Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, rotation, rotationLerpSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(smoothedRotation);
            }

            Vector3 aimDirection = new Vector3(rotationTarget.x, 0, rotationTarget.z);
            if (aimDirection != Vector3.zero)
            {
                // Rotation is handled above via Rigidbody for smooth physics updates.
            }
        }
        else
        {
            Vector3 aimDirection = new Vector3(joystickLook.x, 0, joystickLook.y);
            if (aimDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
                Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationLerpSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(smoothedRotation);
            }
        }

        Vector3 moveDirection = new Vector3(move.x, 0, move.y);
        float controlMultiplier = GetAirControlMultiplier();

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

    private void TriggerSuperJump()
    {
        if (abilityHolder != null && abilityHolder.ability != null)
        {
            abilityHolder.ability.Activate(gameObject);
        }
    }
}
