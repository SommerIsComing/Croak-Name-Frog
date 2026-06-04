using UnityEngine;
using System.Collections.Generic;

public class CompassPin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform pinRect;

    [Header("POI Setup")]
    [SerializeField] private List<Transform> pointsOfInterest = new List<Transform>();

    [Header("Pin Rotation")]
    [SerializeField] private bool useCameraForwardAsNorth = false;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private float pinRotationOffset = 0f;
    [SerializeField] private bool forceCenterPivot = true;

    private Transform currentTarget;

    private void Awake()
    {
        if (pinRect == null)
        {
            pinRect = transform as RectTransform;
        }

        EnsurePinRotatesFromCenter();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }
    }

    private void OnValidate()
    {
        if (pinRect == null)
        {
            pinRect = transform as RectTransform;
        }

        EnsurePinRotatesFromCenter();
    }

    private void Update()
    {
        if (player == null || pinRect == null)
        {
            return;
        }

        currentTarget = GetClosestActivePoi();
        if (currentTarget == null)
        {
            return;
        }

        RotatePinTowards(currentTarget);
    }

    public void AddPointOfInterest(Transform poi)
    {
        if (poi == null || pointsOfInterest.Contains(poi))
        {
            return;
        }

        pointsOfInterest.Add(poi);
    }

    public void RemovePointOfInterest(Transform poi)
    {
        if (poi == null)
        {
            return;
        }

        int index = pointsOfInterest.IndexOf(poi);
        if (index < 0)
        {
            return;
        }

        pointsOfInterest.RemoveAt(index);
    }

    private Transform GetClosestActivePoi()
    {
        Transform closest = null;
        float closestDistanceSqr = float.MaxValue;

        for (int i = 0; i < pointsOfInterest.Count; i++)
        {
            Transform poi = pointsOfInterest[i];
            if (poi == null)
            {
                continue;
            }

            Vector3 toPoi = poi.position - player.position;
            toPoi.y = 0f;
            float distanceSqr = toPoi.sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closest = poi;
            }
        }

        return closest;
    }

    private void RotatePinTowards(Transform target)
    {
        Vector3 toTarget = target.position - player.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector3 northForward = player.forward;

        if (useCameraForwardAsNorth)
        {
            Camera cam = worldCamera != null ? worldCamera : Camera.main;
            if (cam != null)
            {
                northForward = cam.transform.forward;
            }
        }

        northForward.y = 0f;
        if (northForward.sqrMagnitude < 0.0001f)
        {
            northForward = Vector3.forward;
        }

        float signedAngle = Vector3.SignedAngle(northForward.normalized, toTarget.normalized, Vector3.up);
        pinRect.localEulerAngles = new Vector3(0f, 0f, -signedAngle + pinRotationOffset);
    }

    private void EnsurePinRotatesFromCenter()
    {
        if (!forceCenterPivot || pinRect == null)
        {
            return;
        }

        pinRect.pivot = new Vector2(0.5f, 0.5f);
    }
}
