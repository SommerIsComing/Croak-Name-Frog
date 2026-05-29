using UnityEngine;
using System;

public static class UIEvent
{
    // Event for at opdatere quest UI, når der sker ændringer
    public static Action OnUIQuestRefresh;

    public static Action OnPauseMenuNeeded;

    public static Action<NPC_DialogueObject> OnDialogueStart;
    public static Action OnDialogueContinue;

    public static Action<string> OnPlayerTalkedToTheFirstTime;
}
