using UnityEngine;

public class SwordTriggerDamage : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag))
        {
            return;
        }

        EnemyHeath enemyHealth = other.GetComponentInParent<EnemyHeath>();
        if (enemyHealth == null)
        {
            return;
        }

        enemyHealth.TakeDamage(damage);
    }
}
