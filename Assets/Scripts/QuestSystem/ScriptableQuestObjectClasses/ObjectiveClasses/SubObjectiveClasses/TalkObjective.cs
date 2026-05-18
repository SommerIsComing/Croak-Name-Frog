using UnityEngine;

//subclass der indeholder data for et talk objective, som kan være en del af en quest
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Objectives/Talk")]
public class TalkObjective : ObjectiveData
{
    public string npcName;
}
