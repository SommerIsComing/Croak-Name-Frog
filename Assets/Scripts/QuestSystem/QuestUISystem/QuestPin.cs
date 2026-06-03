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
        root.style.display = DisplayStyle.Flex;
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
        QuestInstance pinnedQuest = QuestManager.questManager.pinnedQuest;

        if (pinnedQuest == null)
        {
            QuestManager.questManager.AutoPinQuest();
            pinnedQuest = QuestManager.questManager.pinnedQuest;

            if(pinnedQuest == null)
            {
                root.style.display = DisplayStyle.None;
                return;
            }
            root.style.display = DisplayStyle.Flex;
        }

        if(pinnedQuest.runtimeObjectives.Count == 0)
        {
            root.style.display = DisplayStyle.None;
            return;
        }

        if(pinnedQuest.isQuestCompleted == true)
        {
            root.style.display = DisplayStyle.None;
            return;
        }

        if(pinnedQuest.currentObjectiveIndex >= pinnedQuest.runtimeObjectives.Count)
        {
            QuestManager.questManager.AutoPinQuest();
            pinnedQuest = QuestManager.questManager.pinnedQuest;

            if (pinnedQuest == null)
            {
                root.style.display = DisplayStyle.None;
                return;
            }

            root.style.display = DisplayStyle.Flex;
        }

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
}
