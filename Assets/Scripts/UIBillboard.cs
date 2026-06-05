using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    [SerializeField] bool DisableVerticalTilt = true;

   Transform mainCamera; 
    void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if(mainCamera == null)
        {
            return;
        }
        
        Vector3 direction = transform.position - mainCamera.position;

        if (DisableVerticalTilt)
        {
            direction.y = 0f;
        }

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
