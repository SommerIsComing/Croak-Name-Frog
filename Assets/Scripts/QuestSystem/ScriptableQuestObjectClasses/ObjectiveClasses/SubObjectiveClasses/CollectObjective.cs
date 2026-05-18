using UnityEngine;

//subclass der indeholder data for et collect objective, som kan være en del af en quest
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Objectives/Collect")]
public class CollectObjective : ObjectiveData
{
    public string itemName;
}
