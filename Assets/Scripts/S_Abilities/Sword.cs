using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Abilities/Sword")]
public class Sword : AbilitySO
{
    // Intentional no-op ability logic.
    // Unlock/gating is handled by AbilityHolder + pickups,
    // animation is triggered by PlayerController,
    // and hit detection is handled by the sword trigger MonoBehaviour.
}
