using NUnit.Framework;
using System.Xml.Linq;
using UnityEngine;

// ObjectiveInstance klassen repræsenterer en aktiv objective i spillet og indeholder information om objective status
[System.Serializable]
public class ObjectiveInstance
{
    //datafelt for at holde reference til det originale ObjectiveData scriptable object
    public ObjectiveData objectiveData;

    //holder styr på den aktuelle progress for det aktive objective
    public int currentObjectiveProgress;

    //indikerer om det aktive objective er fuldført eller ej
    public bool isObjectiveCompleted;

    //tjekker om det givne objective er færdigt ved at returnere sandt, i tilfælde hvor det aktive objective progress er over det krævede progress for det pågældende objective
    public bool IsObjectiveComplete()
    {
        if(currentObjectiveProgress >= objectiveData.requiredProgress)
        {
            isObjectiveCompleted = true;
        }
        else
        {
            isObjectiveCompleted = false;
        }
        return currentObjectiveProgress >= objectiveData.requiredProgress;
    }

    public bool IsRequiredQuestsComplete()
    {
        if(objectiveData.questsRequiredForCompletion == null || objectiveData.questsRequiredForCompletion.Count == 0) { return false; }
        
        foreach(QuestData quest in objectiveData.questsRequiredForCompletion)
        {
            bool questIsCompleted = QuestManager.questManager.completedQuests.Exists(q => q.questData.questID == quest.questID);

            if (!questIsCompleted)
            {
                return false;
            }
        }

        return true;
    }
}
