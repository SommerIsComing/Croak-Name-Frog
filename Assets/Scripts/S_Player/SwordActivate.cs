using UnityEngine;

public class SwordActivate : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private int damage = 1;

    private void OnCollisionEnter(Collision collision)
    {
        EnemyHeath enemyHealth = collision.collider.GetComponentInParent<EnemyHeath>();
        if (enemyHealth == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(enemyTag) && !enemyHealth.CompareTag(enemyTag))
        {
            return;
        }

        enemyHealth.TakeDamage(damage);
    }
}

