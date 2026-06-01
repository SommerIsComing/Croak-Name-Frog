using UnityEngine;
using UnityEngine.TextCore.Text;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "Scriptable Objects/NPC/NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;

    public string animTriggerName;

    public List<NPC_DialogueObject> dialogue = new List<NPC_DialogueObject>();
}
