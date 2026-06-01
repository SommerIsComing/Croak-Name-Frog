using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class AreaQuestTrigger : MonoBehaviour
{
    [SerializeField] private string areaID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            QuestEvents.OnAreaEntered?.Invoke(areaID);
        }
    }
}
