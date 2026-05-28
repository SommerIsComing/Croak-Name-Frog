using UnityEngine;

public class UnlockSuperJump : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private string abilityName = "SuperJump";

    private void OnTriggerEnter(Collider other)
    {
        AbilityHolder holder = other.GetComponent<AbilityHolder>();
        if (holder != null)
        {
            holder.UnlockAbilityByName(abilityName);
            Destroy(gameObject); // boots disappear after pickup
        }
    }
}
