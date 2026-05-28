using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/Objective Actions/MakeInteractableAction")]
public class MakeInteractableAction : ObjectiveAction
{
    public bool isInteractable;
    public string id;
    public override void ExecuteAction()
    {
        GameEvent.OnInteractionNeeded?.Invoke(id, isInteractable);
    }
}
