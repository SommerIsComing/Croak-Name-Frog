using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        vcam.LookAt = playerInput.transform;
    }
}
