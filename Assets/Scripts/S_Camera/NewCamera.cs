using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewCamera : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private CinemachineOrbitalFollow orbitalFollow;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private InputActionReference cameraResetAction;

	[Header("Recentering")]
	[SerializeField] private float recenterWait = 1.0f;
	[SerializeField] private float recenterTime = 2.4f;
	[SerializeField] private float manualRecenterTime = 0.35f;
	[SerializeField] private float manualRecenterDuration = 0.4f;

	[Header("Recentering Gate")]
	[SerializeField] private float minMoveMagnitude = 0.1f;
	[SerializeField] private float forwardInputEnter = 0.3f;
	[SerializeField] private float forwardInputExit = 0.15f;

	private bool forwardGate;
	private bool hasAppliedRecentering;
	private bool lastAppliedEnabled;
	private float lastAppliedWait;
	private float lastAppliedTime;
	private float manualRecenterUntil;

	private void OnEnable()
	{
		if (cameraResetAction != null)
		{
			cameraResetAction.action.performed += HandleCameraReset;
			cameraResetAction.action.Enable();
		}
	}

	private void OnDisable()
	{
		if (cameraResetAction != null)
		{
			cameraResetAction.action.performed -= HandleCameraReset;
			cameraResetAction.action.Disable();
		}
	}

	private void Awake()
	{
		ResolveReferences();

		ApplyRecenteringConfig(false, recenterWait, recenterTime);
	}

	private void LateUpdate()
	{
		ResolveReferences();

		if (orbitalFollow == null || playerController == null)
		{
			return;
		}

		Vector2 moveInput = playerController.MoveInput;
		bool moving = moveInput.magnitude >= minMoveMagnitude;
		bool manualRecenterActive = Time.unscaledTime < manualRecenterUntil;

		if (!moving)
		{
			forwardGate = false;
		}
		else if (!forwardGate && moveInput.y >= forwardInputEnter)
		{
			forwardGate = true;
		}
		else if (forwardGate && moveInput.y <= forwardInputExit)
		{
			forwardGate = false;
		}

		bool shouldRecenter = manualRecenterActive || (moving && forwardGate);
		float activeWait = manualRecenterActive ? 0f : recenterWait;
		float activeTime = manualRecenterActive ? manualRecenterTime : recenterTime;

		ApplyRecenteringConfig(shouldRecenter, activeWait, activeTime);
	}

	private void ResolveReferences()
	{
		if (orbitalFollow == null)
		{
			orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
		}

		if (playerController == null)
		{
			playerController = Object.FindFirstObjectByType<PlayerController>();
		}
	}

	private void ApplyRecenteringConfig(bool enabled, float wait, float time)
	{
		if (orbitalFollow == null)
		{
			return;
		}

		if (hasAppliedRecentering &&
			lastAppliedEnabled == enabled &&
			Mathf.Approximately(lastAppliedWait, wait) &&
			Mathf.Approximately(lastAppliedTime, time))
		{
			return;
		}

		var horizontal = orbitalFollow.HorizontalAxis;
		var recentering = horizontal.Recentering;
		recentering.Enabled = enabled;
		recentering.Wait = wait;
		recentering.Time = time;
		horizontal.Recentering = recentering;
		orbitalFollow.HorizontalAxis = horizontal;

		hasAppliedRecentering = true;
		lastAppliedEnabled = enabled;
		lastAppliedWait = wait;
		lastAppliedTime = time;
	}

	private void HandleCameraReset(InputAction.CallbackContext context)
	{
		manualRecenterUntil = Time.unscaledTime + manualRecenterDuration;
	}
}
