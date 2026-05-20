using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Use Unity.Cinemachine if on Cinemachine v3.x

public class CameraGroupBinder : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;

    // Link this function directly to the PlayerInputManager's "Player Joined Event"
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (targetGroup == null || playerInput == null)
        {
            return;
        }

        // Get the transform of the newly spawned player prefab
        Transform playerTransform = playerInput.transform;

        // Add member: Transform, Weight (importance), Radius (padding)
        targetGroup.AddMember(playerTransform, 1f, 1.5f);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        if (targetGroup == null || playerInput == null)
        {
            return;
        }

        targetGroup.RemoveMember(playerInput.transform);
    }
}
