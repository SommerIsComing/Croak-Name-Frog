using System;
using System.Collections.Generic;
using UnityEngine;

// QuestInstance klassen repræsenterer en aktiv quest i spillet, og indeholder information om questens status
[Serializable]
public class QuestInstance
{
    //datafelt for at holde reference til det originale QuestData scriptable object
    public QuestData questData;

    //holder styr på om questen er completed eller ej
    public bool isQuestCompleted;

    //holder styr på den aktuelle index for det aktive objective i questen, som spilleren arbejder på
    public int currentObjectiveIndex;

    //liste over ObjectiveInstance objekter, der repræsenterer de aktive objectives i questen
    public List<ObjectiveInstance> runtimeObjectives = new List<ObjectiveInstance>();


    //metode til at forberede questen ved at oprette ObjectiveInstance objekter for hver objective i questen og tilføje dem til runtimeObjectives listen
    public void PrepQuest()
    {
        runtimeObjectives.Clear();

        foreach (ObjectiveData objective in questData.questObjectives)
        {
            ObjectiveInstance objectiveInstance = new ObjectiveInstance();

            objectiveInstance.objectiveData = objective;
            objectiveInstance.currentObjectiveProgress = 0;

            runtimeObjectives.Add(objectiveInstance);
        }
    }
    
}
