using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Use Unity.Cinemachine if on Cinemachine v3.x

public class CameraGroupBinder : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CameraAverageRotation cameraRotation;

    private void Start()
    {
        if (targetGroup == null)
            return;

        foreach (var playerInput in PlayerInput.all)
        {
            if (playerInput == null)
                continue;

            OnPlayerJoined(playerInput);
        }
    }

    // Link this function directly to the PlayerInputManager's "Player Joined Event"
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (targetGroup == null || playerInput == null)
        {
            return;
        }

        // Get the transform of the newly spawned player prefab
        Transform playerTransform = playerInput.transform;

        targetGroup.RemoveMember(playerTransform);

        // Add member: Transform, Weight (importance), Radius (padding)
        targetGroup.AddMember(playerTransform, 1f, 1.5f);

        if (cameraRotation != null)
            cameraRotation.AddPlayer(playerTransform);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        if (targetGroup == null || playerInput == null)
        {
            return;
        }

        targetGroup.RemoveMember(playerInput.transform);

        if (cameraRotation != null)
            cameraRotation.RemovePlayer(playerInput.transform);
    }
}
