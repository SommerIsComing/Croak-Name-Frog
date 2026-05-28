using UnityEngine;
using System.Collections;

public class UnlockSword : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string abilityNameToUnlock = "Sword";
    [SerializeField] private string abilityNameToLock = "Shooter";
    [SerializeField] private bool lockAnotherAbilityOnPickup = true;

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
            if (lockAnotherAbilityOnPickup && string.IsNullOrEmpty(abilityNameToLock) == false)
            {
                abilityHolder.LockAbilityByName(abilityNameToLock);
            }
            this.gameObject.SetActive(false);
            StartCoroutine(ReactivateObject());
        }
    }

    private IEnumerator ReactivateObject()
    {
        yield return new WaitForSeconds(5f); // Adjust the delay as needed
        this.gameObject.SetActive(true);
    }
}
