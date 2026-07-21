using UnityEngine;

public class EnemyHeath : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float hitInvulnerabilityDuration = 0.15f;
    private float nextDamageAllowedTime;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < nextDamageAllowedTime) return;
        if (currentHealth <= 0) return;

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

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
        StartCoroutine(DeathSequence());
        QuestEvents.OnEnemyKilled.Invoke(gameObject.name);
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        // Destroy the enemy game object
        Destroy(gameObject);
    }
}
