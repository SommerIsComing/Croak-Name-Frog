using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Denne klasse repræsenterer en NPC i spillet. Den indeholder data om NPC'en og håndterer interaktioner og animationer baseret på de events, der bliver triggered.
public class NPC_Instance : MonoBehaviour, Interactable
{
    public NPC_Data npcData;

    public bool questGiven = false;

    public string npcID;

    public string questToGiveID;

    public bool isInteractable = false;

    public bool hasTalkedToPlayer = false;

    public List<NPC_Data> dialogue;

    private void Start()
    {
        npcData = NPC_Manager.npcManager.AssignNPC(npcID);
    }

    private void OnEnable()
    {
        GameEvent.OnAnimNeeded += PlayAnim;
        GameEvent.OnInteractionNeeded += MakeInteractable;
    }

    private void OnDisable()
    {
        GameEvent.OnAnimNeeded -= PlayAnim;
        GameEvent.OnInteractionNeeded -= MakeInteractable;
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
        if(isInteractable)
        {
            QuestEvents.OnNPCTalkedTo?.Invoke(npcID);

            NPC_Manager.npcManager.DisplayDialogue(npcData);

            GiveQuest();
        }
    }

    public void GiveQuest()
    {
        if (!questGiven && !string.IsNullOrEmpty(questToGiveID))
        {
            Debug.Log("Quest Given");
            questGiven = true;
            QuestEvents.OnQuestGivenByID?.Invoke(questToGiveID);
            SetHasTalkedToPlayer(true);
        }
    }

    public void PlayAnim(string triggerName)
    {
        if (triggerName == npcData.animTriggerName && !string.IsNullOrEmpty(npcData.animTriggerName))
        {
            GetComponentInChildren<Animator>().SetTrigger(npcData.animTriggerName);
        }
    }

    public NPC_Data GetNPCDialogue()
    {
        foreach(NPC_Data npcData in dialogue)
        {
            if (CorrectDialogueContext(npcData))
            {
                return npcData;
            }
        }

        return null;
    }

    private bool CorrectDialogueContext(NPC_Data npcData)
    {
        switch (npcData.conditionType)
        {
            case DialogueConditionType.None:
                return true;

            case DialogueConditionType.FirstTimeTalking:
                return !hasTalkedToPlayer;

            case DialogueConditionType.ObjectiveInProgress:
                return QuestManager.questManager.IsObjectiveInProgress(npcData.questID, npcData.requiredObjectiveIndex);

            //case DialogueConditionType.ObjectiveComplete:
                //return QuestManager.questManager.IsObjectiveComplete(npcData.questID, npcData.requiredObjectiveIndex);

            case DialogueConditionType.QuestComplete:
                return QuestManager.questManager.completedQuests.Exists(questInstance => questInstance.questData.questID == questToGiveID);

            default:
                return true;
        }
    }

    public void SetHasTalkedToPlayer(bool value)
    {
        hasTalkedToPlayer = value;
    }
}
