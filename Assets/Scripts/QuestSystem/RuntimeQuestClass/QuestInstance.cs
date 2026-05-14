using System;
using System.Collections.Generic;
using UnityEngine;

// QuestInstance klassen repræsenterer en aktiv quest i spillet, og indeholder information om questens status
[Serializable]
public class QuestInstance
{
    public QuestData questData;

    public bool isQuestCompleted;

    public int currentObjectiveIndex;

    public List<int> objectives = new List<int>();

    public int currentObjectiveProgress;

    public void PrepQuest()
    {
        objectives.Clear();

        // Initialiser objectives listen med 0 for hver questObjective i questData
        foreach (ObjectiveData objective in questData.questObjectives)
        {
            objectives.Add(0);
        }
    }

    public void NewObjectiveProgress()
    {
        currentObjectiveProgress = 0;
        currentObjectiveIndex++;
    }

    public void AddObjectiveProgress() { currentObjectiveProgress += 1; }
}
