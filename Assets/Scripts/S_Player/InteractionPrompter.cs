using UnityEngine;
using UnityEngine.UIElements;

public class InteractionPrompter : MonoBehaviour
{
    public static InteractionPrompter interactionPrompter;
    
    private VisualElement root;

    private Transform currentTarget;
    private Camera mainCamera;

    [SerializeField] private float yTargetOffset;
    [SerializeField] private float xTargetOffset;

    private void Awake()
    {
        if(interactionPrompter == null)
        {
            interactionPrompter = this;
        }
        
        root = GameObject.FindGameObjectWithTag("PlayerHUDroot").GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("InteractionContainer");

        mainCamera = Camera.main;
    }

    private void Start()
    {
        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if(currentTarget == null) { return; }

        Vector3 worldPosition = currentTarget.position + Vector3.up * yTargetOffset;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if(screenPosition.z < 0)
        {
            root.style.display = DisplayStyle.None;
            return;
        }
        root.style.display = DisplayStyle.Flex;

        root.style.left = screenPosition.x + xTargetOffset;

        root.style.top = Screen.height - screenPosition.y;
    }

    public void ShowInteractionPrompt(Transform currentClosestInteractable)
    {
        currentTarget = currentClosestInteractable;
    }

    public void HideInteractionPrompt()
    {
        currentTarget = null;
        root.style.display = DisplayStyle.None;
    }
}
