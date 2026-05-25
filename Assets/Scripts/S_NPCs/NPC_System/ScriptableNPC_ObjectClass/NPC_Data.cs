using UnityEngine;
using UnityEngine.TextCore.Text;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "Scriptable Objects/NPC/NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;

    public string animTriggerName;

    [TextArea(3,5)] public List<string> defaultDialogueText = new List<string>();

    [TextArea(3, 5)] public List<string> activeQuestdialogueText = new List<string>();

    [TextArea(3, 5)] public List<string> completedQuestDialogueText = new List<string>();
}
