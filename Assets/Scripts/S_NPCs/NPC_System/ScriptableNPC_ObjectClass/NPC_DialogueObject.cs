//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NPC_DialogueObject", menuName = "Scriptable Objects/NPC/Dialogue")]
public class NPC_DialogueObject : ScriptableObject
{
    [TextArea] public List<string> dialogueText = new List<string>();

    public Background npcSpeakerImage;

    public string npcSpeakerName;

    public string npcID;

    public DialogueConditionType conditionType;

    public string questID;

    public int requiredObjectiveIndex;
}

public enum DialogueConditionType
{
    None,
    FirstTimeTalking,
    ObjectiveInProgress,
    ObjectiveComplete,
    QuestComplete
}

