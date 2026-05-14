using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager questManager;
    [SerializeField] private QuestDataBASE questDataBase;

    // Liste over aktive/færdige quests i spillet
    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public List<QuestInstance> completedQuests = new List<QuestInstance>();

    // Singleton pattern til QuestManager, der sikrer at der kun er én instans af QuestManager i spillet
    private void Awake()
    {
        if (questManager == null)
        {
            questManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Questmanageren lytter på events for at opdatere quest status, når spilleren samler genstande eller dræber fjender
        QuestEvents.OnItemCollected += HandleItemCollected;
        QuestEvents.OnEnemyKilled += HandleEnemyKilled;
        QuestEvents.OnNPCTalkedTo += HandleNPCTalkedTo;
        QuestEvents.OnAreaEntered += HandleAreaEntered;
        QuestEvents.GiveQuestByID += HandleGiveQuestByID;
    }

    private void OnDisable()
    {
        // Fjern event listeners, når QuestManager deaktiveres
        QuestEvents.OnItemCollected -= HandleItemCollected;
        QuestEvents.OnEnemyKilled -= HandleEnemyKilled;
        QuestEvents.OnNPCTalkedTo -= HandleNPCTalkedTo;
        QuestEvents.OnAreaEntered -= HandleAreaEntered;
        QuestEvents.GiveQuestByID -= HandleGiveQuestByID;
    }

    private void Start()
    {
        QuestEvents.GiveQuestByID?.Invoke("wings");

        QuestEvents.OnEnemyKilled?.Invoke("spider");
        QuestEvents.OnEnemyKilled?.Invoke("spider");
        QuestEvents.OnEnemyKilled?.Invoke("spider");
        QuestEvents.OnEnemyKilled?.Invoke("spider");
        QuestEvents.OnEnemyKilled?.Invoke("spider");
    }

    private void HandleItemCollected(string item)
    {
        // Gennemgår alle aktive quests og tjekker om det opsamlede item er relevant for nogen af questens objectives
        foreach (QuestInstance quest in activeQuests)
        {
            for (int i = 0; i < quest.questData.questObjectives.Count; i++)
            {
                ObjectiveData objective = quest.questData.questObjectives[i];
                if (objective is CollectObjective collectObjective)
                {
                    if (collectObjective.itemName == item)
                    {
                        quest.objectives[i]++;
                        CheckQuestCompletion(quest);
                    }
                }
            }
        }
    }

    private void HandleEnemyKilled(string enemy)
    {
        if(activeQuests.Count == 0) return;

        // Gennemgår alle aktive quests og tjekker om den dræbte fjende er relevant for nogen af questens objectives
        foreach (QuestInstance quest in activeQuests)
        {
            for (int i = 0; i < quest.questData.questObjectives.Count; i++)
            {
                ObjectiveData objective = quest.questData.questObjectives[i];
                if (objective is KillObjective killObjective)
                {
                    if(killObjective.enemyName == enemy)
                    {
                        quest.AddObjectiveProgress();
                        if (quest.currentObjectiveProgress >= killObjective.requiredProgress)
                        {
                            quest.objectives[i]++;
                            quest.NewObjectiveProgress();
                            CheckQuestCompletion(quest);
                        }
                    }
                }
            }
        }
    }

    private void HandleNPCTalkedTo(string npc)
    {
        foreach (QuestInstance quest in activeQuests)
        {
            for (int i = 0; i < quest.questData.questObjectives.Count; i++)
            {
                ObjectiveData objective = quest.questData.questObjectives[i];
                if (objective is TalkObjective talkObjective)
                {
                    if (talkObjective.npcName == npc)
                    {
                        quest.objectives[i]++;
                        CheckQuestCompletion(quest);
                    }
                }
            }
        }
    }

    private void HandleAreaEntered(string area)
    {
        foreach (QuestInstance quest in activeQuests)
        {
            for (int i = 0; i < quest.questData.questObjectives.Count; i++)
            {
                ObjectiveData objective = quest.questData.questObjectives[i];
                if (objective is EnterAreaObjective enterAreaObjective)
                {
                    if (enterAreaObjective.areaName == area)
                    {
                        quest.objectives[i]++;
                        CheckQuestCompletion(quest);
                    }
                }
            }
        }
    }

    public void HandleGiveQuestByID(string questID)
    {
        // giver en quest til spilleren baseret på questID
        QuestData questData = questDataBase.GetQuestByID(questID);

        bool alreadyHasQuest = activeQuests.Exists(questInstance => questInstance.questData.questID == questID);
        if (alreadyHasQuest) return;

        if (questData != null)
        {
            AddQuest(questData);
        }
    }

    //tilføj en quest til listen over aktive quests
    public void AddQuest(QuestData questData)
    {
        QuestInstance newQuest = new QuestInstance();
        newQuest.questData = questData;
        if(newQuest != null )
        {
            newQuest.PrepQuest();
            activeQuests.Add(newQuest);
        }
    }

    //tjekker om alle objectives i en quest er blevet gennemført. Er dette tilfældet angives questen som færdig
    public void CheckQuestCompletion(QuestInstance quest)
    {
        bool allObjectivesCompleted = true;

        // Gennemgår alle objectives i den pågældende quest og tjekker om de er fuldførte
        for (int i = 0; i > quest.questData.questObjectives.Count; i++)
        {
            ObjectiveData objective = quest.questData.questObjectives[i];

            if (quest.objectives[i] < objective.requiredProgress)
            {
                allObjectivesCompleted = false;
                break;
            }
        }

        if (allObjectivesCompleted)
        {
            CompleteQuest(quest);
        }

        // Opdater UI'et for quests ved at udløse en event
        QuestEvents.OnUIQuestRefresh?.Invoke();
    }

    //fjern en quest fra listen over aktive quests, og tilføj den til listen over færdige quests
    public void CompleteQuest(QuestInstance quest)
    {
        if (quest != null)
        {
            quest.isQuestCompleted = true;

            activeQuests.Remove(quest);
            completedQuests.Add(quest);
        }
    }

}
