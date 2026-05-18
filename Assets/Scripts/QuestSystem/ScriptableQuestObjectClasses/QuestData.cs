using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//scriptable object der indeholder data for en quest (med tilhørende objectives), som kan bruges i quest systemet
[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/Quest")]
public class QuestData : ScriptableObject
{
    //navn og ID af questen
    public string questName;
    public string questID;

    //beskrivelse af questen
    [TextArea] public string questDescription;

    //objectives for questen
    [SerializeReference] public List<ObjectiveData> questObjectives = new List<ObjectiveData>();

    //belønning for questen
    public int questReward;

}
