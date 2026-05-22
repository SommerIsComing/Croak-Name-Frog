using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NPC_DataBASE", menuName = "Scriptable Objects/NPC_DataBASE")]
public class NPC_DataBASE : ScriptableObject
{
    public List<NPC_Data> allNPCs = new List<NPC_Data>();

    public NPC_Data GetNPCByID(string npcID)
    {
        return allNPCs.Find(NPC_Data => NPC_Data.npcName == npcID);
    }
}
