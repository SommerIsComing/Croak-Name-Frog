using UnityEngine;

// Interface for at gøre objekter i spillet interaktive, og sørge for at alle interaktive objekter har en Interact() metode
public interface Interactable
{
    void Interact();

    bool IsInteractable();
}
