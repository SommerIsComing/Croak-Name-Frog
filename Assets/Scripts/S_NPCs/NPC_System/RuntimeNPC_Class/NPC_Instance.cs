using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

//Denne klasse repræsenterer en NPC i spillet. Den indeholder data om NPC'en og håndterer interaktioner og animationer baseret på de events, der bliver triggered.
public class NPC_Instance : MonoBehaviour, Interactable
{
    public NPC_Data npcData;

    public bool questGiven = false;

    public string npcID;

    public string questToGiveID;

    public bool isInteractable = false;

    public bool hasTalkedToPlayer = false;


    private void OnEnable()
    {
        GameEvent.OnAnimNeeded += PlayAnim;
        GameEvent.OnInteractionNeeded += MakeInteractable;
        UIEvent.OnPlayerTalkedToTheFirstTime += SetHasTalkedToPlayer;
    }

    private void OnDisable()
    {
        GameEvent.OnAnimNeeded -= PlayAnim;
        GameEvent.OnInteractionNeeded -= MakeInteractable;
    }

    private void Start()
    {
        npcData = NPC_Manager.npcManager.AssignNPC(npcID);
    }

    public void MakeInteractable(string npcName, bool interactable)
    {
        if(npcName == npcData.npcName)
        {
            isInteractable = interactable;
        }
    }

    public void Interact()
    {
        if (isInteractable)
        {
            Debug.Log("Interacted");

            NPC_UI.npc_UI.SetDialogueDisplaying(true);

            GiveQuest();

            NPC_DialogueObject currentDialogue = GetNPCDialogue();

            NPC_Manager.npcManager.DisplayDialogue(currentDialogue);

            QuestEvents.OnNPCTalkedTo?.Invoke(npcID);
        }
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void GiveQuest()
    {
        if (!questGiven && !string.IsNullOrEmpty(questToGiveID))
        {
            Debug.Log("Quest Given");
            questGiven = true;
            QuestEvents.OnQuestGivenByID?.Invoke(questToGiveID);
        }
    }

    public void PlayAnim(string triggerName)
    {
        if (triggerName == npcData.animTriggerName && !string.IsNullOrEmpty(npcData.animTriggerName))
        {
            GetComponentInChildren<Animator>().SetTrigger(npcData.animTriggerName);
        }
    }

    public NPC_DialogueObject GetNPCDialogue()
    {
        foreach(NPC_DialogueObject npcDialogue in npcData.dialogue)
        {
            if (CorrectDialogueContext(npcDialogue))
            {
                return npcDialogue;
            }
        }

        return null;
    }

    private bool CorrectDialogueContext(NPC_DialogueObject npcDialogue)
    {
        switch (npcDialogue.conditionType)
        {
            case DialogueConditionType.QuestComplete:
                return QuestManager.questManager.completedQuests.Exists(questInstance => questInstance.questData.questID == questToGiveID);

            case DialogueConditionType.RequirmentsMet:
                QuestData quest = QuestManager.questManager.questDataBase.GetQuestByID(questToGiveID);
                QuestInstance runtimeQuest = new QuestInstance();

                runtimeQuest.questData = quest;
                ObjectiveInstance objective = runtimeQuest.runtimeObjectives[npcDialogue.requiredObjectiveIndex];

                return objective.IsRequiredQuestsComplete();

            case DialogueConditionType.ObjectiveComplete:
                return QuestManager.questManager.IsObjectiveComplete(npcDialogue.questID, npcDialogue.requiredObjectiveIndex);

            case DialogueConditionType.ObjectiveInProgress:
                return QuestManager.questManager.IsObjectiveInProgress(npcDialogue.questID, npcDialogue.requiredObjectiveIndex);

            case DialogueConditionType.FirstTimeTalking:

                return !hasTalkedToPlayer;

            case DialogueConditionType.None:
                return true;

            default:
                return true;
        }
    }

    public void SetHasTalkedToPlayer(string npcName)
    {
        if(npcName == npcData.npcName && hasTalkedToPlayer != true)
        {
            hasTalkedToPlayer = true;
        }
    }
}
