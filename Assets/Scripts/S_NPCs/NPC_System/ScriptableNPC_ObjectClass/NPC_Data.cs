using UnityEngine;
using UnityEngine.TextCore.Text;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "Scriptable Objects/NPC/NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;

    public string animTriggerName;

    [TextArea(3,6)] public List<string> defaultDialogueText = new List<string>();

    [TextArea(3, 6)] public List<string> firstTimeDialogueText = new List<string>();

    [TextArea(3, 6)] public List<string> objectiveInProgressDialogueText = new List<string>();

    [TextArea(3, 6)] public List<string> objectiveCompleteDialogueText = new List<string>();

    [TextArea(3, 6)] public List<string> QuestCompleteDialogueText = new List<string>();

    public DialogueConditionType conditionType;

    public string questID;

    public int requiredObjectiveIndex;

    public bool needsQuestComplete;

    public bool oneTimeOnly;
}

public enum DialogueConditionType
{
    None,
    FirstTimeTalking,
    ObjectiveInProgress,
    ObjectiveComplete,
    QuestComplete
}
