using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (TryGetSpawnPoint(playerInput.playerIndex, out Transform spawnPoint))
        {
            ApplySpawn(playerInput.transform, spawnPoint);
            StartCoroutine(ApplySpawnAfterPhysics(playerInput.transform, spawnPoint));
        }
    }

    public bool TryRespawnPlayer(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            return false;
        }

        if (!TryGetSpawnPoint(playerInput.playerIndex, out Transform spawnPoint))
        {
            return false;
        }

        ApplySpawn(playerInput.transform, spawnPoint);
        StartCoroutine(ApplySpawnAfterPhysics(playerInput.transform, spawnPoint));
        return true;
    }

    private bool TryGetSpawnPoint(int spawnIndex, out Transform spawnPoint)
    {
        spawnPoint = null;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return false;
        }

        if (spawnIndex < 0 || spawnIndex >= spawnPoints.Length)
        {
            return false;
        }

        spawnPoint = spawnPoints[spawnIndex];
        return spawnPoint != null;
    }

    private static void ApplySpawn(Transform playerTransform, Transform spawnPoint)
    {
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = spawnPoint.position;
            rb.rotation = spawnPoint.rotation;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        playerTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
    }

    private IEnumerator ApplySpawnAfterPhysics(Transform playerTransform, Transform spawnPoint)
    {
        yield return new WaitForFixedUpdate();

        if (playerTransform == null || spawnPoint == null)
        {
            yield break;
        }

        ApplySpawn(playerTransform, spawnPoint);
    }
}
