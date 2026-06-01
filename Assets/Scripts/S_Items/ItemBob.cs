using UnityEngine;

public class ItemBob : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float spinSpeed = 50f;

    private Vector3 startPosition;
    private float startX;
    private float startZ;
    private float currentY;

    private void Start()
    {
        startPosition = transform.position;
        Vector3 startAngles = transform.localEulerAngles;
        startX = startAngles.x;
        startZ = startAngles.z;
        currentY = startAngles.y;
    }

    void Update()
    {
        Vector3 bobbedPosition = startPosition;
        bobbedPosition.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = bobbedPosition;

        currentY += spinSpeed * Time.deltaTime;
        transform.localEulerAngles = new Vector3(startX, currentY, startZ);
    }
}
