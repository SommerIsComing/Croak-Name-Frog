using UnityEngine;
using UnityEngine.UIElements;

public class WorldspaceTooltip : MonoBehaviour
{
    [SerializeField] private UIDocument uiDoc;
    [SerializeField] private string toolTipText;
    [SerializeField] private Color textColor;
    [SerializeField] private float activationDistance = 7f;

    private bool isPlayerClose;


    void Awake()
    {       
        Label toolTipLabel = uiDoc.rootVisualElement.Q<Label>("ToolTipLabel");
        toolTipLabel.text = toolTipText;
        toolTipLabel.style.color = textColor;

        uiDoc.rootVisualElement.style.display = DisplayStyle.None;
    }

    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        isPlayerClose = false;

        foreach (GameObject player in players)
        {
        
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);


            if (distanceToPlayer <= activationDistance)
            {
                isPlayerClose = true;
                break;                
            }
        }
        
        uiDoc.rootVisualElement.style.display = isPlayerClose ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
