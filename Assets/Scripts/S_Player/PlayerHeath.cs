using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerHeath : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 4;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackUpward = 2f;
    [SerializeField] private float flashInterval = 0.02f;

    private bool isInvulnerable;
    private Rigidbody rb;
    private Renderer[] renderers;

    private bool isRespawning;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>();
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

    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        if (currentHealth <= 0 || isRespawning || isInvulnerable) return; // forhindrer yderligere skade, hvis spilleren allerede er død
        currentHealth -= damage;
        Debug.Log("Damaged - Current player health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        Vector3 away = (transform.position - attackerPosition).normalized;
        Vector3 knockbackDir = new Vector3(away.x, 0f, away.z).normalized;

        ApplyKnockback(knockbackDir);
        StartCoroutine(InvulnerabilityRoutine());
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

    private void ApplyKnockback(Vector3 horizontalDir)
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        Vector3 force = horizontalDir * knockbackForce + Vector3.up * knockbackUpward;
        rb.AddForce(force, ForceMode.Impulse);
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        float timer = 0f;
        bool visible = true;

        while (timer < invulnerabilityDuration)
        {
            visible = !visible;
            SetRenderersVisible(visible);
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        SetRenderersVisible(true);
        isInvulnerable = false;
    }

    private void SetRenderersVisible(bool value)
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = value;
            }
        }
    }
}
