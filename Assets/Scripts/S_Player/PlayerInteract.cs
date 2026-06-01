using System;
using UnityEngine;
using UnityEngine.InputSystem;

//Giver spilleren mulighed for at interagere med objekter i spillet, ved at tjekke for det nærmeste objekt og kalde Interact() metoden på det
public class PlayerInteract : MonoBehaviour
{
    private Interactable closestinteractable;

    //Metode der bliver kaldt når spilleren trykker på interaktions knappen, og kalder Interact() metoden på det nærmeste objekt hvis det findes
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (closestinteractable == null) { return; }
        
        if ((context.performed || context.started) && !(NPC_UI.npc_UI.IsDialogueDisplaying()))
        {

            closestinteractable.Interact();
            InteractionPrompter.interactionPrompter.HideInteractionPrompt();
        }
        else if(context.performed && NPC_UI.npc_UI.IsDialogueDisplaying())
        {
            NPC_UI.npc_UI.ContinueDialogue();
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MonoBehaviour interactable = other.GetComponent<MonoBehaviour>();

        if(interactable is Interactable interactableObject && interactableObject.IsInteractable())
        {
            closestinteractable = interactableObject;
            InteractionPrompter.interactionPrompter.ShowInteractionPrompt(((MonoBehaviour)closestinteractable).transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MonoBehaviour interactable = other.GetComponent<MonoBehaviour>();

        if(interactable != null && interactable == closestinteractable)
        {
            closestinteractable = null;
            InteractionPrompter.interactionPrompter.HideInteractionPrompt();
        }
    }
}
