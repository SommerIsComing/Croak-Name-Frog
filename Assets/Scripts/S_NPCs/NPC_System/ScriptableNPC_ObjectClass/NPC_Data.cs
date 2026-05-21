using UnityEngine;
using UnityEngine.TextCore.Text;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "Scriptable Objects/NPC/NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;

    [TextArea(3,5)] public List<string> dialogueText = new List<string>();
}
