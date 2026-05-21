using UnityEngine;

[CreateAssetMenu(fileName = "New SuperJump", menuName = "Abilities/SuperJump")]
public class SuperJump : AbilitySO
{
    [SerializeField] private float superJumpForce = 10f;
    public override void Activate(GameObject parent)
    {
        Rigidbody rb = parent.GetComponent<Rigidbody>();
        Animator animator = parent.GetComponent<Animator>();
        PlayerJump playerJump = parent.GetComponent<PlayerJump>();
        if (rb != null && playerJump != null && playerJump.IsGrounded)
        {
            rb.AddForce(Vector3.up * superJumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true);
        }
         else
        {
        animator.SetBool("isJumping", false);
        }
    
    }
}
