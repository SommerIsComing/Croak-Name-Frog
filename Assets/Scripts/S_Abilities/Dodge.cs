using UnityEngine;

[CreateAssetMenu(fileName = "Dodge", menuName = "Scriptable Abilities/Dodge")]
public class Dodge : AbilitySO
{
    [SerializeField] private float dodgeSpeed = 12f;
    //[SerializeField] private string dodgeTriggerName = "Dodge";

    private Rigidbody rb;
    private Vector3 dodgeDirection;

    public override void Activate(GameObject parent)
    {
        rb = parent.GetComponent<Rigidbody>();
        // animator = parent.GetComponent<Animator>();
        PlayerController controller = parent.GetComponent<PlayerController>();

        dodgeDirection = GetDodgeDirection(parent, controller);

        // if (animator != null)
        // {
        //     animator.SetTrigger(dodgeTriggerName);
        // }
    }

    public override void FixedActiveUpdate(GameObject parent)
    {
        if (rb == null) return;

        rb.MovePosition(rb.position + dodgeDirection * dodgeSpeed * Time.fixedDeltaTime);
    }

    public override void Deactivate(GameObject parent)
    {
        if (rb != null)
        {
            // stop horizontal momentum from the dodge, keep vertical (gravity/jump) intact
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    // Beregner dodge-retningen ud fra spillerens input og kameraets retning,
    // ligesom den almindelige bevægelse i PlayerController. Uden input dodges
    // spilleren i den retning den allerede vender.
    private Vector3 GetDodgeDirection(GameObject parent, PlayerController controller)
    {
        Vector2 move = controller != null ? controller.MoveInput : Vector2.zero;

        if (Camera.main != null)
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * move.y + right * move.x;
            if (direction.sqrMagnitude > 0.01f)
            {
                return direction.normalized;
            }
        }

        return parent.transform.forward;
    }
}
