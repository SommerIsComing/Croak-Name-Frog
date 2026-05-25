using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void DisplayPauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIEvent.OnPauseMenuNeeded?.Invoke();
        }
    }
}
