using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraAverageRotation : MonoBehaviour
{
    [SerializeField] private float rotationSmoothSpeed = 3f;
    [SerializeField] private float positionSmoothSpeed = 5f;
    [SerializeField] private bool keepCurrentY = true;
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [Header("Rotation Stability")]
    [SerializeField] private float turnDeadzoneDegrees = 8f;
    [SerializeField, Range(-1f, 1f)] private float antiFlipDotThreshold = -0.2f;

    [Header("Zoom")]
    [SerializeField] private bool zoomByOrbitalRadius = true;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private float minOrbitalRadius = 6f;
    [SerializeField] private float maxOrbitalRadius = 14f;
    [SerializeField] private float orbitalRadiusSmoothSpeed = 5f;
    [SerializeField] private float minPlayerDistanceForZoom = 3f;
    [SerializeField] private float maxPlayerDistanceForZoom = 20f;

    private readonly List<Transform> players = new List<Transform>();
    private Vector3 stableForward;
    private bool hasStableForward;

    public void AddPlayer(Transform player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

    public void RemovePlayer(Transform player)
    {
        players.Remove(player);
    }

    void LateUpdate()
    {
        List<Transform> activePlayers = GetActivePlayers();
        if (activePlayers.Count == 0)
            return;

        Vector3 avgPosition = Vector3.zero;
        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            avgPosition += activePlayers[i].position;
        }

        avgPosition /= activePlayers.Count;

        Vector3 averageForward = Vector3.zero;
        foreach (Transform p in activePlayers)
            averageForward += p.forward;

        averageForward.y = 0f;

        if (averageForward.sqrMagnitude < 0.001f)
            return;

        Vector3 stableHeading = GetStableHeading(averageForward.normalized);

        float zoomT = Mathf.InverseLerp(minPlayerDistanceForZoom, maxPlayerDistanceForZoom, GetMaxDistance(activePlayers));
        UpdateOrbitalRadiusZoom(zoomT);
        Vector3 desiredPosition = avgPosition;

        if (keepCurrentY)
            desiredPosition.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(stableHeading);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
    }

    private void UpdateOrbitalRadiusZoom(float zoomT)
    {
        if (!zoomByOrbitalRadius)
            return;

        if (orbitalFollow == null)
            return;

        float targetRadius = Mathf.Lerp(minOrbitalRadius, maxOrbitalRadius, zoomT);
        orbitalFollow.Radius = Mathf.Lerp(orbitalFollow.Radius, targetRadius, orbitalRadiusSmoothSpeed * Time.deltaTime);
    }

    private Vector3 GetStableHeading(Vector3 candidateForward)
    {
        if (!hasStableForward)
        {
            stableForward = candidateForward;
            hasStableForward = true;
            return stableForward;
        }

        if (Vector3.Dot(stableForward, candidateForward) < antiFlipDotThreshold)
            return stableForward;

        float yawDelta = Vector3.SignedAngle(stableForward, candidateForward, Vector3.up);
        if (Mathf.Abs(yawDelta) <= turnDeadzoneDegrees)
            return stableForward;

        stableForward = candidateForward;
        return stableForward;
    }

    private float GetMaxDistance(List<Transform> activePlayers)
    {
        if (activePlayers.Count < 2)
            return 0f;

        float maxDistance = 0f;
        for (int i = 0; i < activePlayers.Count - 1; i++)
        {
            for (int j = i + 1; j < activePlayers.Count; j++)
            {
                float distance = Vector3.Distance(activePlayers[i].position, activePlayers[j].position);
                if (distance > maxDistance)
                    maxDistance = distance;
            }
        }

        return maxDistance;
    }

    private List<Transform> GetActivePlayers()
    {
        if (targetGroup != null)
        {
            players.Clear();
            foreach (var target in targetGroup.Targets)
            {
                if (target.Object != null)
                    players.Add(target.Object);
            }
            return players;
        }

        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (players[i] == null)
                players.RemoveAt(i);
        }

        return players;
    }
}
