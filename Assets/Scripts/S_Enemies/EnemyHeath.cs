using UnityEngine;

public class EnemyHeath : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float hitInvulnerabilityDuration = 0.15f;
    private float nextDamageAllowedTime;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < nextDamageAllowedTime) return;
        if (currentHealth <= 0) return;

        nextDamageAllowedTime = Time.time + hitInvulnerabilityDuration;
        currentHealth -= damage;
        Debug.Log("Damaged - Current enemy health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        QuestEvents.OnEnemyKilled.Invoke(gameObject.name);
    }
}
