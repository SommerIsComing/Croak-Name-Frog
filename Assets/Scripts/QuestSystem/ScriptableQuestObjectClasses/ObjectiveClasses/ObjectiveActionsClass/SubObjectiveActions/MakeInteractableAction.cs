using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/Objective Actions/MakeInteractableAction")]
public class MakeInteractableAction : ObjectiveAction
{
    public bool isInteractable;
    public string npcName;
    public override void ExecuteAction()
    {
        GameEvent.OnInteractionNeeded?.Invoke(npcName, isInteractable);
    }
}
