using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SwordActivate : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string swordAbilityName = "Sword";
    [SerializeField] private int damage = 1;
    [SerializeField] private Collider hitboxCollider;
    [SerializeField] private UnityEvent OnAttack;

    private AbilityHolder abilityHolder;
    private bool isAttackWindowActive;

    private void Awake()
    {
        abilityHolder = GetComponent<AbilityHolder>();
        if (abilityHolder == null)
            abilityHolder = GetComponentInParent<AbilityHolder>();

        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider>();

        if (hitboxCollider != null)
            hitboxCollider.enabled = false; // start off
    }

    // Call from animation event at swing start. The attack clips are shared between
    // weapons, so only react if the Sword is the currently equipped weapon.
    public void EnableAttackWindow()
    {
        if (abilityHolder != null && !abilityHolder.IsAbilityUnlockedByName(swordAbilityName))
        {
            return;
        }

        isAttackWindowActive = true;
        OnAttack?.Invoke();

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

