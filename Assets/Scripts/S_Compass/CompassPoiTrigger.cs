using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CompassPoiTrigger : MonoBehaviour
{
    [SerializeField] private CompassPin compassPin;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool disableOnReached = true;

    private void Awake()
    {
        if (compassPin == null)
        {
            compassPin = Object.FindFirstObjectByType<CompassPin>(FindObjectsInactive.Include);
        }

        Collider colliderComponent = GetComponent<Collider>();
        if (colliderComponent != null)
        {
            colliderComponent.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        if (compassPin != null)
        {
            compassPin.RemovePointOfInterest(transform);
        }

        if (disableOnReached)
        {
            gameObject.SetActive(false);
        }
    }
}
