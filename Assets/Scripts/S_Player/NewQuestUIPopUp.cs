using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class NewQuestUIPopUp : MonoBehaviour
{
    private VisualElement newQuestRoot;
    private Label newQuestNameText;
    [SerializeField] private float popUpDuration;

    private VisualElement completedQuestRoot;
    private Label completedQuestNameText;

    private void Awake()
    {
        newQuestRoot = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("NewQuestContainer");
        newQuestNameText = newQuestRoot.Q<Label>("NewQuestNameText");

        completedQuestRoot = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("CompletedQuestContainer");
        completedQuestNameText = completedQuestRoot.Q<Label>("CompletedQuestNameText");
    }

    private void Start()
    {
        newQuestRoot.style.display = DisplayStyle.None;
        completedQuestRoot.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        UIEvent.OnNewQuest += NewQuestPopUp;

        UIEvent.OnQuestCompleted += QuestCompletedPopUp;
    }

    private void OnDisable()
    {
        UIEvent.OnNewQuest -= NewQuestPopUp;

        UIEvent.OnQuestCompleted -= QuestCompletedPopUp;
    }

    private void NewQuestPopUp(QuestInstance quest)
    {
        newQuestNameText.text = quest.questData.questName;

        StartCoroutine(DisplayPopUp());
    }

    IEnumerator DisplayPopUp()
    {
        yield return new WaitWhile(NPC_UI.npc_UI.IsDialogueDisplaying);

        newQuestRoot.style.display = DisplayStyle.Flex;

        yield return new WaitForSeconds(popUpDuration);

        newQuestRoot.style.display = DisplayStyle.None;
    }

    private void QuestCompletedPopUp(QuestInstance quest)
    {
        completedQuestNameText.text = quest.questData.questName;

        StartCoroutine(DisplayCompletedPopUp());
    }

    IEnumerator DisplayCompletedPopUp()
    {
        yield return new WaitWhile(NPC_UI.npc_UI.IsDialogueDisplaying);

        completedQuestRoot.style.display = DisplayStyle.Flex;

        yield return new WaitForSeconds(popUpDuration);

        completedQuestRoot.style.display = DisplayStyle.None;
    }
}
