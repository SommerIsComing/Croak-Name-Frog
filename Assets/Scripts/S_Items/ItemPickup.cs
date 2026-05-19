using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string itemID;

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            QuestEvents.OnItemCollected?.Invoke(itemID);

            Destroy(gameObject);
        }
    }
}
