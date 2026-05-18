using UnityEngine;

//subclass der indeholder data for et enter area objective, som kan være en del af en quest
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Objectives/EnterArea")]
public class EnterAreaObjective : ObjectiveData
{
    public string areaName;
}
