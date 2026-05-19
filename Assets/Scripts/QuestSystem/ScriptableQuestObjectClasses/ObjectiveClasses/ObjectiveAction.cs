using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveAction", menuName = "Scriptable Objects/ObjectiveAction")]
public abstract class ObjectiveAction : ScriptableObject
{
    public abstract void ExecuteAction();
}
