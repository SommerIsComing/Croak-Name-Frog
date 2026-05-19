using UnityEngine;

//objective executable actions der kan knytte sig til et objective, hvilket udføres i tilfælde af et færdigt objective
[System.Serializable]
public abstract class ObjectiveAction : ScriptableObject
{
    public abstract void ExecuteAction();
}
