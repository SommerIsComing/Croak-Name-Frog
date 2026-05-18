using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// Scriptable object der indeholder en database af alle quests i spillet og derved kan finde en quest baseret på deres ID
[CreateAssetMenu(fileName = "QuestDataBASE", menuName = "Scriptable Objects/QuestDataBASE")]
public class QuestDataBASE : ScriptableObject
{
    public List<QuestData> allQuests = new List<QuestData>();

    public QuestData GetQuestByID(string questID)
    {
        return allQuests.Find(questData => questData.questID == questID);
    }
}
