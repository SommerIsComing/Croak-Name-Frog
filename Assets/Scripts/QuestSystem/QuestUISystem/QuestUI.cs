using System;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset questTemplate;
    VisualElement root;

    private ListView activeQuestsListUI;
    private ListView completedQuestsListUI;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("PagesContainer").Q<VisualElement>("QuestLogPage");

        activeQuestsListUI = root.Q<ListView>("ActiveQuestsList");
        completedQuestsListUI = root.Q<ListView>("CompletedQuestsList");
    }

    private void Start()
    {
        ConfigureListViewBehavoir();
    }

    private void OnEnable()
    {
        UIEvent.OnUIQuestRefresh += UpdateUI;
    }

    private void OnDisable()
    {
        UIEvent.OnUIQuestRefresh -= UpdateUI;
    }

    // Metode til at bygge aktive quests (i activeQuests- og completedQuests-listen) om til items i ListView (baseret på en templet "questTemplet") for både aktive og fuldførte quests
    private void ConfigureListViewBehavoir()
    {
        activeQuestsListUI.itemsSource = QuestManager.questManager.activeQuests;
        completedQuestsListUI.itemsSource = QuestManager.questManager.completedQuests;

        // Sætter virtualizationMethod til DynamicHeight for at sikre, at ListView kan håndtere elementer med varierende højde baseret på antallet af objectives i hver quest
        activeQuestsListUI.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        completedQuestsListUI.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

        // Definerer makeItem-funktionen for begge ListViews, som instantiere en ny UI-element baseret på questTemplate
        activeQuestsListUI.makeItem = () => questTemplate.Instantiate();
        completedQuestsListUI.makeItem = () => questTemplate.Instantiate();

        // Definerer bindItem-funktionen for begge ListViews, som indsætter data fra hver quest i activeQuests- og completedQuests-listen
        activeQuestsListUI.bindItem = (element, index) =>
        {
            QuestInstance quest = QuestManager.questManager.activeQuests[index];

            BindQuest(element, quest, false);
        };

        completedQuestsListUI.bindItem = (element, index) =>
        {
            QuestInstance quest = QuestManager.questManager.completedQuests[index];

            BindQuest(element, quest, true);
        };
    }

    // Metode til at binde data fra aktive/completed quests til UI elementerne for hver item i ListView
    private void BindQuest(VisualElement element, QuestInstance quest, bool isCompletedQuest)
    {
        Label questTitel = element.Q<Label>("QuestTitel");
        questTitel.text = quest.questData.questName;

        if (isCompletedQuest)
        {
            questTitel.AddToClassList("completed-quest-objective");
            questTitel.text = $"✓ {quest.questData.questName}";
        }

        VisualElement objectivesContainer = element.Q<VisualElement>("ObjectivesContainer");
        objectivesContainer.Clear();

        //for hver runtimeObjective for den aktive quest, omdannes beskrivelsen samt progressionen af det givne objective til UI-elementer i form af Labels text
        foreach (ObjectiveInstance objective in quest.runtimeObjectives)
        {
            Label objectiveLabel = new Label();
            bool isObjectiveCompleted = objective.isObjectiveCompleted;

            objectiveLabel.text = $"- {objective.objectiveData.objectiveDescription}: " + $"{objective.currentObjectiveProgress}/" + $"{objective.objectiveData.requiredProgress}";

            if (isObjectiveCompleted)
            {
                objectiveLabel.AddToClassList("completed-quest-objective");
                objectiveLabel.text = $"✓ {objective.objectiveData.objectiveDescription}";
            }

            objectivesContainer.Add(objectiveLabel);
        }

        //pin button functionality
        Button pinnedQuestButton = element.Q<Button>("PinButton");

        // Fjern tidligere tilknyttet action-event for at undgå multiple event calls
        if (pinnedQuestButton.userData is Action oldAction)
        {
            pinnedQuestButton.clicked -= oldAction;
        }

        // Opret en ny Action-event for pin knappen, som pinner questen hvis den ikke er pinned, og unpinner den hvis den allerede er pinned
        Action newPinAction = () =>
        {
            if (!isCompletedQuest && quest != QuestManager.questManager.pinnedQuest)
            {
                QuestManager.questManager.PinQuest(quest);
            }
            else if (quest == QuestManager.questManager.pinnedQuest)
            {
                QuestManager.questManager.UnpinQuest();
            }
        };

        // Tilknyt den nye event til knappen
        pinnedQuestButton.userData = newPinAction;

        pinnedQuestButton.clicked += newPinAction;


        if (quest != QuestManager.questManager.pinnedQuest)
        {
            pinnedQuestButton.text = "Fastgør";

        }
        else
        {
            pinnedQuestButton.text = "Løsgør";
        }
    }

    // Metode til at opdatere UI, når der sker ændringer i quests
    private void UpdateUI()
    {
        //Genopbyg ListViews ved at udføre bindItem-funktionen igen
        activeQuestsListUI.Rebuild();
        completedQuestsListUI.Rebuild();
    }
}
