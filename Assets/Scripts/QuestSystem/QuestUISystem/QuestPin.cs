using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Label = UnityEngine.UIElements.Label;

public class QuestPin : MonoBehaviour
{
    private VisualElement root;
    private VisualElement questPinContainer;
    private Label pinnedQuestTitle;
    private Label pinnedQuestObjective;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("QuestTrackerContainer");
        pinnedQuestTitle = root.Q<Label>("PinnedQuestTitle");
        pinnedQuestObjective = root.Q<Label>("PinnedQuestObjective");
    }

    private void Start()
    {
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        UIEvent.OnUIQuestRefresh += UpdatePinnedQuest;
        UIEvent.OnPauseMenuNeeded += TogglePinnedObjective;
    }

    private void OnDisable()
    {
        UIEvent.OnUIQuestRefresh -= UpdatePinnedQuest;
        UIEvent.OnPauseMenuNeeded -= TogglePinnedObjective;
    }

    private void UpdatePinnedQuest()
    {
        if (QuestManager.questManager.pinnedQuest == null)
        {
             root.style.display = DisplayStyle.None;
             return;
        }

        QuestInstance pinnedQuest = QuestManager.questManager.pinnedQuest;

        if (pinnedQuest.runtimeObjectives.Count == 0)
        {
            pinnedQuest = null;
            root.style.display = DisplayStyle.None;
            return;
        }

        if(pinnedQuest.isQuestCompleted == true || pinnedQuest.currentObjectiveIndex >= pinnedQuest.runtimeObjectives.Count)
        {
            if (QuestManager.questManager.pinnedQuest != null)
            {
                QuestManager.questManager.AutoPinQuest();
                pinnedQuest = QuestManager.questManager.pinnedQuest;
                root.style.display = DisplayStyle.Flex;
                return;
            }
            else
            {
                pinnedQuest = null;
                root.style.display = DisplayStyle.None;
                return;
            }
        }

        root.style.display = DisplayStyle.Flex;

        pinnedQuestTitle.text = pinnedQuest.questData.questName;

        ObjectiveInstance currentPinnedObjective = pinnedQuest.runtimeObjectives[pinnedQuest.currentObjectiveIndex];

        if (!currentPinnedObjective.isObjectiveCompleted)
        {
            if(currentPinnedObjective.objectiveData.requiredProgress <= 1)
            {
                pinnedQuestObjective.text = $"{currentPinnedObjective.objectiveData.objectiveDescription} ";
            }
            else
            {
                pinnedQuestObjective.text = $"{currentPinnedObjective.objectiveData.objectiveDescription} " + $"{currentPinnedObjective.currentObjectiveProgress}/" +
                $"{currentPinnedObjective.objectiveData.requiredProgress}";
            }
            return;
        }
    }

    private void TogglePinnedObjective()
    {
        if (UI_Manager.uiManager.noteBookUIDisplaying)
        {
            root.style.display = DisplayStyle.None;
        }
        else if(!UI_Manager.uiManager.noteBookUIDisplaying)
        {
            root.style.display = DisplayStyle.Flex;
        }
    }

    public void HideQuestPins()
    {
        root.style.display = DisplayStyle.None;
    }
}
