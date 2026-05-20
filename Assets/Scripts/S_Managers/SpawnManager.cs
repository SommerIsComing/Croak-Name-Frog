using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    
    private PlayerInputManager inputManager;
    private int playerIndex = 0;

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        inputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        // Ensure we don't go out of bounds if there are more players than spawn points
        if (playerIndex < spawnPoints.Length)
        {
            playerInput.transform.position = spawnPoints[playerIndex].position;
            playerInput.transform.rotation = spawnPoints[playerIndex].rotation;
            playerIndex++;
        }
        else
        {
            Debug.LogWarning("More players joined than available spawn points!");
        }
    }
}
