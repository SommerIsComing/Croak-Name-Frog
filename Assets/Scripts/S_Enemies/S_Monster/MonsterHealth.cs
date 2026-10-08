using UnityEngine;
using UnityEngine.Events;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float hitInvulnerabilityDuration = 0.6f; // under dette flimrer monsteret og kan ikke tage skade
    [SerializeField] private float flashInterval = 0.06f;
    [Range(0.05f, 1f)]
    [SerializeField] private float minScaleFactor = 0.4f; // scale ved 1 hp tilbage, som andel af start-scale
    [SerializeField] private float shrinkSpeed = 8f;
    [SerializeField] private float deathDelay = 0f;

    private Vector3 startScale;
    private Vector3 targetScale;
    private float nextDamageAllowedTime;
    private bool isDead;
    private Renderer[] renderers;

    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        currentHealth = maxHealth;
        startScale = transform.localScale;
        targetScale = startScale;
    }

    private void Update()
    {
        if (isDead) return;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, shrinkSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < nextDamageAllowedTime) return;

        nextDamageAllowedTime = Time.time + hitInvulnerabilityDuration;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log("Damaged - Current monster health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // fuld hp = start-scale, 1 hp tilbage = minScaleFactor
        float t = maxHealth > 1 ? (currentHealth - 1f) / (maxHealth - 1f) : 1f;
        targetScale = startScale * Mathf.Lerp(minScaleFactor, 1f, t);

        StopAllCoroutines();
        StartCoroutine(FlickerRoutine());
        OnHit?.Invoke();
    }

    private System.Collections.IEnumerator FlickerRoutine()
    {
        bool visible = true;
        while (Time.time < nextDamageAllowedTime)
        {
            visible = !visible;
            SetRenderersVisible(visible);
            yield return new WaitForSeconds(flashInterval);
        }
        SetRenderersVisible(true);
    }

    private void SetRenderersVisible(bool value)
    {
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = value;
        }
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        SetRenderersVisible(true);
        Destroy(gameObject, deathDelay); // først, så en fejl i events ikke forhindrer døden

        var ai = GetComponent<MonsterAI>();
        if (ai != null) ai.enabled = false;
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;

        OnDeath?.Invoke();
        QuestEvents.OnEnemyKilled.Invoke(gameObject.name);
    }
}
