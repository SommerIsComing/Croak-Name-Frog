using UnityEngine;

public class PlayerAttackHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyHeath enemyHealth = other.GetComponent<EnemyHeath>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1); // påfører 1 skade til fjenden
            }
        }
    }
}
