using UnityEngine;
using System.Collections;

public class UnlockShooter : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string abilityNameToUnlock = "Shooter";
    [SerializeField] private string abilityNameToLock = "Sword";
    [SerializeField] private bool lockAnotherAbilityOnPickup = true;
    [SerializeField] private float respawnTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(playerTag))
        {
            return;
        }
        AbilityHolder abilityHolder = other.GetComponent<AbilityHolder>();
        if (abilityHolder != null)
        {
            abilityHolder.UnlockAbilityByName(abilityNameToUnlock);

            PlayerWeaponVisuals visuals = other.GetComponent<PlayerWeaponVisuals>();
            if (visuals != null)
            {
                visuals.SetShooterUnlocked(true);
            }

            if (lockAnotherAbilityOnPickup && string.IsNullOrEmpty(abilityNameToLock) == false)
            {
                abilityHolder.LockAbilityByName(abilityNameToLock);

                if (visuals != null)
                {
                    visuals.SetSwordUnlocked(false);
                }
            }

            StartCoroutine(ReactivateObject());
        }
    }

    private IEnumerator ReactivateObject()
    {
        SetVisible(false);
        yield return new WaitForSeconds(respawnTime);
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = visible;
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = visible;
    }
}
