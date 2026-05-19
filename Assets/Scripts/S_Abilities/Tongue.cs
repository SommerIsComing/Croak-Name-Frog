using UnityEngine;

[CreateAssetMenu(fileName = "New Tongue", menuName = "Scriptable Abilities/Tongue")]
public class Tongue : AbilitySO
{
    [SerializeField] private float tongueRange = 20f;
    [SerializeField] private float tongueExtendSpeed = 30f;
    [SerializeField] private float tonguePullSpeed = 18f;
    [SerializeField] private float arrivalDistance = 1.5f;
    [SerializeField] private string tongueOriginName = "TongueOrigin";

    private Vector3 targetPosition;
    private Rigidbody rb;
    private LineRenderer line;
    private bool hasTarget;
    private bool isPulling;
    private bool isComplete;
    private float extendTimer = 0f;
    private float totalExtendTime = 0f;
    public override void Activate(GameObject parent)
    {
        hasTarget = false;
        isPulling = false;
        isComplete = false;
        extendTimer = 0f;
        totalExtendTime = 0f;
        rb = parent.GetComponent<Rigidbody>();
        line = parent.GetComponentInChildren<LineRenderer>();

        Transform origin = parent.transform.Find(tongueOriginName) ?? parent.transform;

        if (Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, tongueRange))
        {
            hasTarget = true;
            targetPosition = hit.point;

            float distance = Vector3.Distance(origin.position, targetPosition);
            totalExtendTime = distance / Mathf.Max(0.01f, tongueExtendSpeed);
            extendTimer = totalExtendTime;

            if (line != null)
            {
                line.enabled = true;
                line.positionCount = 2;
                line.SetPosition(0, origin.position);
                line.SetPosition(1, origin.position);
            }
        }
        else
        {
            isComplete = true; // No target hit, complete immediately
            if (line != null)
            {
                line.enabled = false;
            }
        }
    }

    public override void FixedActiveUpdate(GameObject parent)
    {
        if (!hasTarget || isComplete)
        {
            return;
        }

        Transform origin = parent.transform.Find(tongueOriginName) ?? parent.transform;
        Vector3 currentOrigin = origin.position;

        if (!isPulling)
        {
            extendTimer -= Time.fixedDeltaTime;
            float progress = 1f - Mathf.Clamp01(extendTimer / Mathf.Max(0.0001f, totalExtendTime));
            Vector3 tip = Vector3.Lerp(currentOrigin, targetPosition, progress);
            if (line != null)
            {
                line.SetPosition(0, currentOrigin);
                line.SetPosition(1, tip);
            }
            if (extendTimer <= 0f)
            {
                isPulling = true;
            }
            return; // Wait until tongue is fully extended before pulling
        }

        if (line != null)
        {
            line.SetPosition(0, currentOrigin);
            line.SetPosition(1, targetPosition);
        }

        if (Vector3.Distance(parent.transform.position, targetPosition) <= arrivalDistance)
        {
            isComplete = true;
        }

        Vector3 next = Vector3.MoveTowards(parent.transform.position, targetPosition, tonguePullSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = Vector3.zero; // Stop current movement
        rb.MovePosition(next);

        if (Vector3.Distance(rb.position, targetPosition) <= arrivalDistance)
        {
            isComplete = true;
        }
    }

    public override bool IsActiveComplete(GameObject parent)
    {
        return isComplete;
    }

    public override void Deactivate(GameObject parent)
    {
        if (line != null)
        {
            line.enabled = false;
        }
    }
}
