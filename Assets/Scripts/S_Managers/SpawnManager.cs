using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Scene References")]
    [SerializeField] private UnlockShooter shooterUnlock;
    [SerializeField] private UnlockSword swordUnlock;

    public static SpawnManager spawnManager { get; private set; }

    private void Awake()
    {
        spawnManager = this;

        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        }

        if (shooterUnlock == null)
        {
            shooterUnlock = Object.FindFirstObjectByType<UnlockShooter>();
        }

        if (swordUnlock == null)
        {
            swordUnlock = Object.FindFirstObjectByType<UnlockSword>();
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            return;
        }

        if (!TryGetSpawnPose(playerInput, out Vector3 spawnPosition, out Quaternion spawnRotation))
        {
            return;
        }

        ApplySpawn(playerInput.transform, spawnPosition, spawnRotation);
        AssignAbilityUnlockReferences(playerInput.transform);
        StartCoroutine(ApplySpawnAfterPhysics(playerInput.transform, spawnPosition, spawnRotation));
    }

    public bool TryRespawnPlayer(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            return false;
        }

        if (!TryGetSpawnPose(playerInput, out Vector3 spawnPosition, out Quaternion spawnRotation))
        {
            return false;
        }

        ApplySpawn(playerInput.transform, spawnPosition, spawnRotation);
        AssignAbilityUnlockReferences(playerInput.transform);
        StartCoroutine(ApplySpawnAfterPhysics(playerInput.transform, spawnPosition, spawnRotation));
        return true;
    }

    private void AssignAbilityUnlockReferences(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return;
        }

        PlayerController controller = playerTransform.GetComponent<PlayerController>();
        if (controller == null)
        {
            return;
        }

        controller.SetAbilityUnlockReferences(shooterUnlock, swordUnlock);
    }

    private bool TryGetSpawnPose(PlayerInput playerInput, out Vector3 position, out Quaternion rotation)
    {
        position = default;
        rotation = Quaternion.identity;

        foreach (PlayerInput otherInput in PlayerInput.all)
        {
            if (otherInput == null || otherInput == playerInput)
            {
                continue;
            }

            Transform anchor = otherInput.transform;
            if (anchor == null)
            {
                continue;
            }

            position = anchor.position + Vector3.up * 4f;
            rotation = anchor.rotation;
            return true;
        }

        if (TryGetSpawnPoint(playerInput.playerIndex, out Transform spawnPoint))
        {
            position = spawnPoint.position;
            rotation = spawnPoint.rotation;
            return true;
        }

        return false;
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

    private static void ApplySpawn(Transform playerTransform, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = spawnPosition;
            rb.rotation = spawnRotation;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        playerTransform.SetPositionAndRotation(spawnPosition, spawnRotation);
    }

    private IEnumerator ApplySpawnAfterPhysics(Transform playerTransform, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        yield return new WaitForFixedUpdate();

        if (playerTransform == null)
        {
            yield break;
        }

        ApplySpawn(playerTransform, spawnPosition, spawnRotation);
    }
}
