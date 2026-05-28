using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

public class NPC_Manager : MonoBehaviour 
{
    public static NPC_Manager npcManager;
    [SerializeField] private NPC_DataBASE npcDataBase;

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

    public NPC_Data AssignNPC(string npcID)
    {
        return npcDataBase.GetNPCByID(npcID);
    }

    public void DisplayDialogue(NPC_Data npcData)
    {
        //UIEvent.OnDialogueStart?.Invoke(npcData);
    }
}
