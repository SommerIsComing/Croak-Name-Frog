using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    private PlayerHeath playerHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You drowned, pretty cringe"); 
            playerHealth = other.GetComponent<PlayerHeath>();
            playerHealth.Die();
        }
    }
}
