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
        closestinteractable?.Interact();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        MonoBehaviour interactable = other.GetComponent<MonoBehaviour>();

        if(interactable is Interactable interactableObject)
        {
            closestinteractable = interactableObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MonoBehaviour interactable = other.GetComponent<MonoBehaviour>();

        if(interactable != null && interactable == closestinteractable)
        {
            closestinteractable = null;
        }
    }
}
