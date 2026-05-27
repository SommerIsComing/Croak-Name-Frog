using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Label = UnityEngine.UIElements.Label;

public class QuestPin : MonoBehaviour
{
    private VisualElement root;
    private Label pinnedQuestTitle;
    private Label pinnedQuestObjective;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        pinnedQuestTitle = root.Q<Label>("PinnedQuestTitle");
        pinnedQuestObjective = root.Q<Label>("PinnedQuestObjective");
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

        if(pinnedQuest == null)
        {
            pinnedQuestTitle.text = "No Pinned Quest";
            pinnedQuestObjective.text = "Find something to do!";
            return;
        }

        if(pinnedQuest.runtimeObjectives.Count == 0)
        {
            pinnedQuestTitle.text = "No Pinned Quest";
            pinnedQuestObjective.text = "Find something to do!";
            return;
        }

        if(pinnedQuest.currentObjectiveIndex >= pinnedQuest.runtimeObjectives.Count)
        {
            pinnedQuestTitle.text = pinnedQuest.questData.questName;
            pinnedQuestObjective.text = "Quest Completed!";
            return;
        }

        pinnedQuestTitle.text = pinnedQuest.questData.questName;

        ObjectiveInstance currentPinnedObjective = pinnedQuest.runtimeObjectives[pinnedQuest.currentObjectiveIndex];

        if (!currentPinnedObjective.isObjectiveCompleted)
        {
            pinnedQuestObjective.text = $"{currentPinnedObjective.objectiveData.objectiveDescription} " + $"{currentPinnedObjective.currentObjectiveProgress}/" + 
            $"{currentPinnedObjective.objectiveData.requiredProgress}";
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
