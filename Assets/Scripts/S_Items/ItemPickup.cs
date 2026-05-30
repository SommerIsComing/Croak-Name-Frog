using UnityEngine;

public class ItemPickup : MonoBehaviour, Interactable
{
    [SerializeField] private string itemID;
    [SerializeField] private bool hasPickedUp = false;
    [SerializeField] private bool hasQuest = false;
    [SerializeField] private bool isInteractableForPlayer = false;

    private void OnEnable()
    {
        GameEvent.OnInteractionNeeded += MakeInteractable;
    }

    public void Interact()
    {
        if (isInteractableForPlayer)
        {
            hasQuest = QuestManager.questManager.HasActiveQuests();

            if (hasQuest && !hasPickedUp)
            {
                QuestEvents.OnItemCollected?.Invoke(itemID);
                hasPickedUp = true;

                Destroy(gameObject);
            }
        }
    }

    private void MakeInteractable(string id, bool interactable)
    {
        if(itemID == id)
        {
            isInteractableForPlayer = interactable;
        }
    }
}
