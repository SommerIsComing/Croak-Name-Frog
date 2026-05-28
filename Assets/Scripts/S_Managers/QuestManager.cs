using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// QuestManager klassen er ansvarlig for at håndtere alle aktive quests/objectives, opdatere quest/objective progress, tjekke completion samt handlinger relateret til Quest events
public class QuestManager : MonoBehaviour
{
    // Singleton instans af QuestManager, der kan tilgås globalt i spillet samt database reference til at hente quest data
    public static QuestManager questManager;
    [SerializeField] private QuestDataBASE questDataBase;

    // Liste over aktive/færdige quests i spillet
    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public List<QuestInstance> completedQuests = new List<QuestInstance>();
    public QuestInstance pinnedQuest = null;

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

        UIEvent.OnUIQuestRefresh?.Invoke();
    }

    private void OnEnable()
    {
        // Questmanageren lytter på events for at opdatere quest status
        QuestEvents.OnItemCollected += HandleItemCollected;
        QuestEvents.OnEnemyKilled += HandleEnemyKilled;
        QuestEvents.OnNPCTalkedTo += HandleNPCTalkedTo;
        QuestEvents.OnAreaEntered += HandleAreaEntered;
        QuestEvents.OnQuestGivenByID += HandleQuestGivenByID;
    }

    private void OnDisable()
    {
        // Fjern event listeners, når QuestManager deaktiveres
        QuestEvents.OnItemCollected -= HandleItemCollected;
        QuestEvents.OnEnemyKilled -= HandleEnemyKilled;
        QuestEvents.OnNPCTalkedTo -= HandleNPCTalkedTo;
        QuestEvents.OnAreaEntered -= HandleAreaEntered;
        QuestEvents.OnQuestGivenByID -= HandleQuestGivenByID;
    }

    private void HandleItemCollected(string item)
    {
        if(activeQuests.Count == 0) return;
        // Gennemgår alle aktive quests og tjekker om det opsamlede item er relevant for nogen af questens aktive objectives
        foreach (QuestInstance quest in activeQuests)
        {
            foreach (ObjectiveInstance objective in quest.runtimeObjectives)
            {
                if(objective.objectiveData is CollectObjective collectObjective)
                {
                    if (collectObjective.itemName == item)
                    {
                        if (quest.runtimeObjectives.IndexOf(objective) == quest.currentObjectiveIndex)
                        {
                            //er det et relevant item, opdateres objective progress. hvis det pågældende objective er completed, tjekkes det om questen er fuldført
                            objective.currentObjectiveProgress = Mathf.Min(objective.currentObjectiveProgress + 1, objective.objectiveData.requiredProgress);

                            UIEvent.OnUIQuestRefresh?.Invoke();

                            if (objective.IsObjectiveComplete())
                            {
                                if (quest.currentObjectiveIndex < quest.runtimeObjectives.Count)
                                {
                                    quest.currentObjectiveIndex += 1;
                                    UIEvent.OnUIQuestRefresh?.Invoke();
                                }

                                // Udfør eventuelle handlinger, der er knyttet til objective completion, før der tjekkes for quest completion
                                foreach (ObjectiveAction action in objective.objectiveData.actionsUponCompletion)
                                {
                                    action.ExecuteAction();
                                }

                                CheckQuestCompletion(quest);
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleEnemyKilled(string enemy)
    {
        // Gennemgår alle aktive quests og tjekker om den dræbte fjende er relevant for nogen af questens aktive objectives
        foreach (QuestInstance quest in activeQuests)
        {
            foreach (ObjectiveInstance objective in quest.runtimeObjectives)
            {
                if (objective.objectiveData is KillObjective killObjective)
                {
                    if (killObjective.enemyName == enemy)
                    {
                        if (quest.runtimeObjectives.IndexOf(objective) == quest.currentObjectiveIndex)
                        {
                            //er det et relevant enemy, opdateres objective progress. hvis det pågældende objective er completed, tjekkes det om questen er fuldført
                            objective.currentObjectiveProgress = Mathf.Min(objective.currentObjectiveProgress + 1, objective.objectiveData.requiredProgress);

                            UIEvent.OnUIQuestRefresh?.Invoke();

                            if (objective.IsObjectiveComplete())
                            {
                                if (quest.currentObjectiveIndex < quest.runtimeObjectives.Count)
                                {
                                    quest.currentObjectiveIndex += 1;
                                    UIEvent.OnUIQuestRefresh?.Invoke();
                                }

                                // Udfør eventuelle handlinger, der er knyttet til objective completion, før der tjekkes for quest completion
                                foreach (ObjectiveAction action in objective.objectiveData.actionsUponCompletion)
                                {
                                    action.ExecuteAction();
                                }

                                CheckQuestCompletion(quest);
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleNPCTalkedTo(string npc)
    {
        if(activeQuests.Count == 0) return;

        // Gennemgår alle aktive quests og tjekker om npc interaktionen er relevant for nogen af questens aktive objectives
        foreach (QuestInstance quest in activeQuests)
        {
            foreach (ObjectiveInstance objective in quest.runtimeObjectives)
            {
                if (objective.objectiveData is TalkObjective talkObjective)
                {
                    if (talkObjective.npcName == npc)
                    {
                        if (quest.runtimeObjectives.IndexOf(objective) == quest.currentObjectiveIndex)
                        {
                            //er det en relevant npc, opdateres objective progress. hvis det pågældende objective er completed, tjekkes det om questen er fuldført
                            objective.currentObjectiveProgress = Mathf.Min(objective.currentObjectiveProgress + 1, objective.objectiveData.requiredProgress);

                            UIEvent.OnUIQuestRefresh?.Invoke();

                            if (objective.IsObjectiveComplete())
                            {
                                if (quest.currentObjectiveIndex < quest.runtimeObjectives.Count)
                                {
                                    quest.currentObjectiveIndex += 1;
                                    UIEvent.OnUIQuestRefresh?.Invoke();
                                }

                                // Udfør eventuelle handlinger, der er knyttet til objective completion, før der tjekkes for quest completion
                                foreach (ObjectiveAction action in objective.objectiveData.actionsUponCompletion)
                                {
                                    action.ExecuteAction();
                                }

                                CheckQuestCompletion(quest);
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandleAreaEntered(string area)
    {
        // Gennemgår alle aktive quests og tjekker om området er relevant for nogen af questens aktive objectives
        foreach (QuestInstance quest in activeQuests)
        {
            foreach (ObjectiveInstance objective in quest.runtimeObjectives)
            {
                if (objective.objectiveData is EnterAreaObjective enterAreaObjective)
                {
                    if (enterAreaObjective.areaName == area)
                    {
                        if (quest.runtimeObjectives.IndexOf(objective) == quest.currentObjectiveIndex)
                        {
                            //er det et relevant område, opdateres objective progress. hvis det pågældende objective er completed, tjekkes det om questen er fuldført
                            objective.currentObjectiveProgress = Mathf.Min(objective.currentObjectiveProgress + 1, objective.objectiveData.requiredProgress);

                            UIEvent.OnUIQuestRefresh?.Invoke();

                            if (objective.IsObjectiveComplete())
                            {
                                if (quest.currentObjectiveIndex < quest.runtimeObjectives.Count)
                                {
                                    quest.currentObjectiveIndex += 1;
                                    UIEvent.OnUIQuestRefresh?.Invoke();
                                }

                                // Udfør eventuelle handlinger, der er knyttet til objective completion, før der tjekkes for quest completion
                                foreach (ObjectiveAction action in objective.objectiveData.actionsUponCompletion)
                                {
                                    action.ExecuteAction();
                                }

                                CheckQuestCompletion(quest);
                            }
                        }
                    }
                }
            }
        }
    }

    public void HandleQuestGivenByID(string questID)
    {
        // giver en quest til spilleren baseret på questID ved at søge igennem quest databasen
        QuestData questData = questDataBase.GetQuestByID(questID);

        // Tjekker om spilleren allerede har questen i sin aktive quest liste for at undgå at tilføje den flere gange
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
        // den nye quest-instance "newQuest" tildeles quest-dataen fra quest databasen (fra HandleQuestGivenByID()-metoden)
        newQuest.questData = questData;

        if(newQuest != null )
        {
            newQuest.PrepQuest();
            activeQuests.Add(newQuest);

            UIEvent.OnUIQuestRefresh?.Invoke();

            if(pinnedQuest == null)
            {
                PinQuest(newQuest);
            }
        }
    }

    //tjekker om alle aktive objectives i en aktiv quest er blevet gennemført. Er dette tilfældet angives questen som færdig
    public void CheckQuestCompletion(QuestInstance quest)
    {
        // Gennemgår alle aktive objectives i den pågældende aktive quest og tjekker om de er fuldførte
        foreach (ObjectiveInstance objective in quest.runtimeObjectives)
        {
            // Hvis et eneste objective ikke er fuldført, afsluttes metoden og questen forbliver incomplete
            if (!objective.IsObjectiveComplete()) return;
        }

        CompleteQuest(quest);
    }

    public bool IsObjectiveInProgress(string questID, int objectiveIndex)
    {
        QuestInstance quest = activeQuests.Find(q => q.questData.questID == questID);
        if(quest == null || quest.runtimeObjectives.Count == 0) return false;

        ObjectiveInstance currentObjective = quest.runtimeObjectives[quest.currentObjectiveIndex];
        return !currentObjective.IsObjectiveComplete();
    }

    //fjern en quest fra listen over aktive quests, og tilføj den til listen over færdige quests
    public void CompleteQuest(QuestInstance quest)
    {
        if (quest != null)
        {
            quest.isQuestCompleted = true;

            activeQuests.Remove(quest);
            completedQuests.Add(quest);

            UIEvent.OnUIQuestRefresh?.Invoke();
        }
    }

    public void PinQuest (QuestInstance quest)
    {
        pinnedQuest = quest;
        UIEvent.OnUIQuestRefresh?.Invoke();
    }

    public void UnpinQuest()
    {
        pinnedQuest = null;
        UIEvent.OnUIQuestRefresh?.Invoke();
    }

    public bool HasActiveQuests()
    {
        return activeQuests.Count > 0;
    }
}
