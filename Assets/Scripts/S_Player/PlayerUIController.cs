using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void DisplayPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIEvent.OnPauseMenuNeeded?.Invoke();
            UIEvent.OnUIQuestRefresh?.Invoke();
        }
    }

    public void ContinueDialogue(InputAction.CallbackContext context)
    {
        if (context.performed && NPC_UI.npc_UI.IsDialogueDisplaying())
        {
            UIEvent.OnDialogueContinue?.Invoke();
        }
    }
}
