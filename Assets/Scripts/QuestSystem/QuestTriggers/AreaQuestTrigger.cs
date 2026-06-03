using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class AreaQuestTrigger : MonoBehaviour
{
    [SerializeField] private Transform compassAnchor;
    [SerializeField] private string areaID;


    private void OnEnable()
    {
        Transform anchor = compassAnchor != null ? compassAnchor : transform;

        if (!string.IsNullOrWhiteSpace(areaID) && QuestTargetRegistry.questTargetRegistry != null)
        {
            QuestTargetRegistry.questTargetRegistry.RegisterTarget(areaID, anchor);
        }
    }

    private void OnDisable()
    {
        Transform anchor = compassAnchor != null ? compassAnchor : transform;

        if (!string.IsNullOrWhiteSpace(areaID) && QuestTargetRegistry.questTargetRegistry != null)
        {
            QuestTargetRegistry.questTargetRegistry.UnregisterTarget(areaID, anchor);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            QuestEvents.OnAreaEntered?.Invoke(areaID);
        }
    }
}
