using UnityEngine;

// Put this on each attack collider (left arm, right arm, body). The collider must be a trigger.
public class MonsterAttackHitbox : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float knockbackUpward = 3f;

    private int damage;
    private Transform attacker;
    private bool active;
    private bool hasHit;

    public void Activate(int damageAmount, Transform attackerTransform)
    {
        damage = damageAmount;
        attacker = attackerTransform;
        hasHit = false;
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void OnTriggerEnter(Collider other) => TryHit(other);
    private void OnTriggerStay(Collider other) => TryHit(other);

    private void TryHit(Collider other)
    {
        if (!active || hasHit) return;

        PlayerHeath player = other.GetComponentInParent<PlayerHeath>();
        if (player == null) return;

        hasHit = true; // max one hit per attack
        player.TakeDamage(damage, attacker != null ? attacker.position : transform.position, knockbackForce, knockbackUpward);
    }
}
