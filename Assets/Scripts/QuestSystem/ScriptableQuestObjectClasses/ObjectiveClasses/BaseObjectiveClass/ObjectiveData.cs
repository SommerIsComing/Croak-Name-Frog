using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//overordnede class der indeholder data for et generelt objective, som kan være en del af en quest
[System.Serializable]
public abstract class ObjectiveData : ScriptableObject
{
    public string objectiveDescription;

    public int requiredProgress;

    public List<ObjectiveAction> actionsUponCompletion = new List<ObjectiveAction>();

    public List<QuestData> questsRequiredForCompletion = new List<QuestData>();
}
