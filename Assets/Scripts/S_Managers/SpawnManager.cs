using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Respawn Point (on death)")]
    [SerializeField] private Transform respawnPoint;

    [Header("Scene Entry Point (on scene load)")]
    [SerializeField] private Transform sceneEntryPoint;

    public static SpawnManager spawnManager { get; private set; }

    private void Awake()
    {
        spawnManager = this;
        PlacePlayerAtSceneEntry();
    }

    private void PlacePlayerAtSceneEntry()
    {
        if (sceneEntryPoint == null)
        {
            return;
        }

        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            return;
        }

        ApplySpawn(player.transform, sceneEntryPoint.position, sceneEntryPoint.rotation);
        StartCoroutine(ApplySpawnAfterPhysics(player.transform, sceneEntryPoint.position, sceneEntryPoint.rotation));
    }

    public bool TryRespawnPlayer(PlayerInput playerInput)
    {
        if (playerInput == null || respawnPoint == null)
        {
            return false;
        }

        ApplySpawn(playerInput.transform, respawnPoint.position, respawnPoint.rotation);
        StartCoroutine(ApplySpawnAfterPhysics(playerInput.transform, respawnPoint.position, respawnPoint.rotation));
        return true;
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
