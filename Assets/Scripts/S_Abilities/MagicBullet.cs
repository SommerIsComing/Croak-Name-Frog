using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float shootVolume = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, float speed, float lifetime)
    {
        rb.linearVelocity = direction.normalized * speed;
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
        Destroy(gameObject, lifetime);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, lifetime);
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        EnemyHeath enemyHealth = collision.gameObject.GetComponent<EnemyHeath>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
