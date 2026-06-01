using UnityEngine;

public class PlayerWeaponVisuals : MonoBehaviour
{
    [SerializeField] private GameObject swordVisual;
    [SerializeField] private GameObject shooterVisual;

    public void SetSwordUnlocked(bool unlocked)
    {
        if (swordVisual != null)
        {
            swordVisual.SetActive(unlocked);
        }
    }

    public void SetShooterUnlocked(bool unlocked)
    {
        if (shooterVisual != null)
        {
            shooterVisual.SetActive(unlocked);
        }
    }
}