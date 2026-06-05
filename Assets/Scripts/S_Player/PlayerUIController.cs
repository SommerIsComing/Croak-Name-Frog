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

    public void OnControllerCancel(InputAction.CallbackContext context)
    {
        if (UI_Manager.uiManager.noteBookUIDisplaying)
        {
            UIEvent.OnControllerCancel?.Invoke();
        }
    }
}
