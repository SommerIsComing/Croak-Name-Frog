using UnityEngine;

public class QuestZone : MonoBehaviour
{
    [SerializeField] private string questID;
    bool hasEnteredArea = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEnteredArea)
        {
            QuestEvents.OnQuestGivenByID.Invoke(questID);
            hasEnteredArea = true;
        }
    }
}
