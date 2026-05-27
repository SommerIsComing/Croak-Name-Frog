using UnityEngine;

public class UnlockShooter : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string abilityName = "Shooter";
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(playerTag))
        {
            return;
        }
        AbilityHolder abilityHolder = other.GetComponent<AbilityHolder>();
        if (abilityHolder != null)
        {
            abilityHolder.UnlockAbilityByName(abilityName);
            Destroy(gameObject);
        }
    }
}
