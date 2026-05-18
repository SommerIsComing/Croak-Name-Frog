using UnityEngine;

public class PlayerHeath : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Healed - Current player health: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if(currentHealth <= 0) return; // forhindrer yderligere skade, hvis spilleren allerede er død)
        currentHealth -= damage;
        Debug.Log("Damaged - Current player health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }
}
