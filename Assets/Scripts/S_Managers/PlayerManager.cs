using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    [Header("Scene References")]
    [SerializeField] private UnlockShooter shooterUnlock;
    [SerializeField] private UnlockSword swordUnlock;

    public static PlayerManager playerManager { get; private set; }

    private Transform player;

    private void Awake()
    {
        playerManager = this;

        if (shooterUnlock == null)
        {
            shooterUnlock = Object.FindFirstObjectByType<UnlockShooter>();
        }

        if (swordUnlock == null)
        {
            swordUnlock = Object.FindFirstObjectByType<UnlockSword>();
        }
    }

    private void Start()
    {
        // Find the single player in the scene
        player = Object.FindFirstObjectByType<PlayerController>()?.transform;
        
        if (player == null)
        {
            Debug.LogWarning("PlayerManager: Could not find player with PlayerController component.", this);
        }

        // Ensure spawn points are set
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[1];
            spawnPoints[0] = player;
        }
    }

    public bool TryRespawnPlayer(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return false;
        }

        if (!TryGetSpawnPose(out Vector3 spawnPosition, out Quaternion spawnRotation))
        {
            return false;
        }

        ApplySpawn(playerTransform, spawnPosition, spawnRotation);
        AssignAbilityUnlockReferences(playerTransform);
        StartCoroutine(ApplySpawnAfterPhysics(playerTransform, spawnPosition, spawnRotation));
        return true;
    }

    private bool TryGetSpawnPose(out Vector3 position, out Quaternion rotation)
    {
        position = default;
        rotation = Quaternion.identity;

        // Use first spawn point if available
        if (spawnPoints != null && spawnPoints.Length > 0 && spawnPoints[0] != null)
        {
            position = spawnPoints[0].position;
            rotation = spawnPoints[0].rotation;
            return true;
        }

        // Fallback to player's current location
        if (player != null)
        {
            position = player.position;
            rotation = player.rotation;
            return true;
        }

        return false;
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
