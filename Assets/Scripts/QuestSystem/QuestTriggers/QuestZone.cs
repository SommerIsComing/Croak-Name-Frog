using UnityEngine;

public class QuestZone : MonoBehaviour
{
    [SerializeField] private string questID;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestEvents.OnQuestGivenByID.Invoke(questID);
        }
    }
}
