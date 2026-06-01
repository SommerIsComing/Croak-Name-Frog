using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, float speed, float lifetime)
    {
        rb.linearVelocity = direction.normalized * speed;
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
