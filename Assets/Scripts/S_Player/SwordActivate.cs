using UnityEngine;
using System.Collections.Generic;

public class SwordActivate : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private int damage = 1;
    [SerializeField] private Collider hitboxCollider;

    private bool isAttackWindowActive;

    private void Awake()
    {
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider>();

        if (hitboxCollider != null)
            hitboxCollider.enabled = false; // start off
    }

    // Call from animation event at swing start
    public void EnableAttackWindow()
    {
        isAttackWindowActive = true;

        if (hitboxCollider != null)
            hitboxCollider.enabled = true;
    }

    // Call from animation event at swing end
    public void DisableAttackWindow()
    {
        isAttackWindowActive = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttackWindowActive) return;
    if (!other.CompareTag(enemyTag)) return;

    EnemyHeath enemyHealth = other.GetComponent<EnemyHeath>();
    if (enemyHealth == null) return;

    enemyHealth.TakeDamage(damage);
    }
}

