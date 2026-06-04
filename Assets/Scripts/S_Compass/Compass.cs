using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] private GameObject compass;

    public void ActivateCompass()
    {
        if (compass != null)
        {
            compass.SetActive(true);
        }
    }

    public void DeactivateCompass()
    {
        if (compass != null)
        {
            compass.SetActive(false);
        }
    }
}
