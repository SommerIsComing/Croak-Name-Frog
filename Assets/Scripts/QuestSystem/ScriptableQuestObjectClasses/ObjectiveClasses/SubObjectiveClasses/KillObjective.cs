using UnityEngine;

//subclass der indeholder data for et kill objective, som kan være en del af en quest
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Objectives/Kill")]
public class KillObjective : ObjectiveData
{
    public string enemyName;
}
