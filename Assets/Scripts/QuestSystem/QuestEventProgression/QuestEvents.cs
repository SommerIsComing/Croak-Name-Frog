using System;
using UnityEngine;

public static class QuestEvents
{
    // Events for quest progression, der kan udløses, når spilleren udfører forskellige objectives
    public static Action<string> OnEnemyKilled;
    public static Action<string> OnItemCollected;
    public static Action<string> OnNPCTalkedTo;
    public static Action<string> OnAreaEntered;

    // Event for at give en quest til spilleren ved hjælp af quest ID
    public static Action<string> OnQuestGivenByID;
}
