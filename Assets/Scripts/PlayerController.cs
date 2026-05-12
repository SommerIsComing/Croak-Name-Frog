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
    public bool isPc;
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnMouseLook(InputAction.CallbackContext context)
    {
        mouseLook = context.ReadValue<Vector2>();
    }

    public void OnJoystickLook(InputAction.CallbackContext context)
    {
        joystickLook = context.ReadValue<Vector2>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
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
    }

    public void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(move.x, 0, move.y);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationLerpSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
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

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

    }
}
