using Unity.VisualScripting;
using UnityEngine;

public class NPC_Instance : MonoBehaviour, Interactable
{
    public NPC_Data npcData;

    public bool questGiven = false;

    public string npcID;

    public bool isInteractable = false;

    private void Awake()
    {
        npcData = NPC_Manager.npcManager.AssignNPC(npcID);
    }

    private void OnEnable()
    {
        GameEvent.OnAnimNeeded += PlayAnim;

    }

    private void OnDisable()
    {
        GameEvent.OnAnimNeeded -= PlayAnim;

    }

    public void Interactable(string npcName, bool interactable)
    {
        if(npcName == npcData.npcName)
        {
            isInteractable = interactable;
        }
    }

    public void Interact()
    {
        Debug.Log(npcData.dialogueText);
        QuestEvents.OnNPCTalkedTo?.Invoke(npcData.npcName);
    }

    public void GiveQuest()
    {
        if (!questGiven)
        {
            Debug.Log("Quest Given");
            questGiven = true;
            QuestEvents.OnQuestGivenByID?.Invoke(npcData.npcName);
        }
    }

    public void PlayAnim(string animTriggerName)
    {
        if (animTriggerName == npcData.npcName)
        {
            GetComponent<Animator>().SetTrigger(animTriggerName);
        }
    }
}
