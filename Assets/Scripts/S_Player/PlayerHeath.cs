using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerHeath : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 4;
    [SerializeField] private float respawnDelay = 1f;

    private bool isRespawning;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

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
        if (currentHealth <= 0 || isRespawning) return; // forhindrer yderligere skade, hvis spilleren allerede er død)
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
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (respawnDelay > 0f)
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        bool respawned = SpawnManager.Instance != null
            && SpawnManager.Instance.TryRespawnPlayer(playerInput);

        if (!respawned)
        {
            Debug.LogWarning("Respawn failed because SpawnManager or spawn point is missing.", this);
        }

        currentHealth = maxHealth;
        Debug.Log("Player respawned - Current player health: " + currentHealth);
        isRespawning = false;
    }
}
