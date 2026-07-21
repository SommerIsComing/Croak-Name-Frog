using System;
using UnityEngine;
using UnityEngine.InputSystem;

//Giver spilleren mulighed for at interagere med objekter i spillet, ved at tjekke for det nærmeste objekt og kalde Interact() metoden på det
public class PlayerInteract : MonoBehaviour
{
    private Interactable closestInteractable;

    //Metode der bliver kaldt når spilleren trykker på interaktions knappen, og kalder Interact() metoden på det nærmeste objekt hvis det findes
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (closestInteractable == null) { return; }
        
        if (context.performed && !(NPC_UI.npc_UI.IsDialogueDisplaying()))
        {
            closestInteractable.Interact();
            InteractionPrompter.interactionPrompter.HideInteractionPrompt();
        }
        else if(context.performed && NPC_UI.npc_UI.IsDialogueDisplaying())
        {
            UIEvent.OnDialogueContinue?.Invoke();
            InteractionPrompter.interactionPrompter.HideInteractionPrompt();
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MonoBehaviour interactable = other.GetComponent<MonoBehaviour>();

        if(interactable is Interactable interactableObject && interactableObject.IsInteractable() && !(NPC_UI.npc_UI.IsDialogueDisplaying()))
        {
            closestInteractable = interactableObject;
            InteractionPrompter.interactionPrompter.ShowInteractionPrompt(((MonoBehaviour)closestInteractable).transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MonoBehaviour interactable = other.GetComponent<MonoBehaviour>();
        MonoBehaviour closestInteractableBehaviour = closestInteractable as MonoBehaviour;

        if(interactable != null && interactable == closestInteractableBehaviour)
        {
            closestInteractable = null;
            InteractionPrompter.interactionPrompter.HideInteractionPrompt();
        }
    }
}
