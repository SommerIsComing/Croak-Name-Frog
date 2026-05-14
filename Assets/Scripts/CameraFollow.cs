using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Reflection;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam;

    private void OnEnable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }
    }

    private void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (vcam == null)
        {
            Debug.LogWarning("CameraFollow: CinemachineCamera reference is missing.");
            return;
        }

        AssignCameraTarget(playerInput.transform);
    }

    private void AssignCameraTarget(Transform target)
    {
        vcam.LookAt = target;
        vcam.Follow = target;

        // Cinemachine 3 uses a Tracking Target field in the Target settings.
        var targetProperty = typeof(CinemachineCamera).GetProperty("Target", BindingFlags.Public | BindingFlags.Instance);
        if (targetProperty == null || !targetProperty.CanRead || !targetProperty.CanWrite)
        {
            return;
        }

        object targetSettings = targetProperty.GetValue(vcam);
        if (targetSettings == null)
        {
            return;
        }

        var settingsType = targetSettings.GetType();
        var trackingField = settingsType.GetField("TrackingTarget", BindingFlags.Public | BindingFlags.Instance);
        if (trackingField != null)
        {
            trackingField.SetValue(targetSettings, target);
            targetProperty.SetValue(vcam, targetSettings);
            return;
        }

        var trackingProperty = settingsType.GetProperty("TrackingTarget", BindingFlags.Public | BindingFlags.Instance);
        if (trackingProperty != null && trackingProperty.CanWrite)
        {
            trackingProperty.SetValue(targetSettings, target);
            targetProperty.SetValue(vcam, targetSettings);
        }
    }
}
