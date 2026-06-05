using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using System.Collections;

public class NPC_Manager : MonoBehaviour 
{
    public static NPC_Manager npcManager;
    [SerializeField] private NPC_DataBASE npcDataBase;
    [SerializeField] private NPC_Instance momNPC;

    private void Awake()
    {
        if (npcManager == null)
        {
            npcManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        yield return null; // Venter en frame for at sikre, at alle NPC'er er initialiseret
        momNPC.Interact();
    }

    public NPC_Data AssignNPC(string npcID)
    {
        return npcDataBase.GetNPCByID(npcID);
    }

    public void DisplayDialogue(NPC_DialogueObject npcDialogue)
    {
        if(npcDialogue == null) { Debug.Log("Dialogue is null"); return; }
        
        if(npcDialogue.conditionType == DialogueConditionType.FirstTimeTalking)
        {
            UIEvent.OnPlayerTalkedToTheFirstTime?.Invoke(npcDialogue.npcID);
        }
        
        UIEvent.OnDialogueStart?.Invoke(npcDialogue);
    }
}
