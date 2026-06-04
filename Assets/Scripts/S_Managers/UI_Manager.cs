using UnityEngine;
using UnityEngine.UIElements;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager uiManager;

    private VisualElement root;
    private VisualElement buttonRoot;
    private VisualElement pageRoot;

    private Button pauseMenuButton;
    private Button questLogButton;
    private Button collectiblesButton;
    private Button currentButton;

    public VisualElement pauseMenuPage;
    public VisualElement questLogPage;
    public VisualElement collectiblesPage;
    private ListView activeQuestsListUI;
    public bool noteBookUIDisplaying = false;

    private void Awake()
    {
        if (uiManager == null)
        {
            uiManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        root = GetComponent<UIDocument>().rootVisualElement;
        buttonRoot = root.Q<VisualElement>("BookMarkButtonsContainer");
        pageRoot = root.Q<VisualElement>("PagesContainer");

        pauseMenuButton = buttonRoot.Q<Button>("PauseMenuButton");
        questLogButton = buttonRoot.Q<Button>("QuestLogButton");
        collectiblesButton = buttonRoot.Q<Button>("CollectiblesButton");

        pauseMenuPage = pageRoot.Q<VisualElement>("PauseMenuPage");
        questLogPage = pageRoot.Q<VisualElement>("QuestLogPage");
        collectiblesPage = pageRoot.Q<VisualElement>("CollectiblesPage");

        activeQuestsListUI = questLogPage.Q<ListView>("ActiveQuestsList");
    }


    private void OnEnable()
    {
        pauseMenuButton.clicked += DisplayPauseMenuPage;
        questLogButton.clicked += DisplayQuestLog;
        collectiblesButton.clicked += DisplayCollectibles;

        UIEvent.OnPauseMenuNeeded += DisplayNoteBook;
        UIEvent.OnControllerCancel += ReturnToButtonTab;

        DisplayQuestLog();
        HideNoteBook();
    }

    private void OnDisable()
    {
        pauseMenuButton.clicked -= DisplayPauseMenuPage;
        questLogButton.clicked -= DisplayQuestLog;
        collectiblesButton.clicked -= DisplayCollectibles;

        UIEvent.OnPauseMenuNeeded -= DisplayNoteBook;
        UIEvent.OnControllerCancel -= ReturnToButtonTab;
    }

    private void DisplayNoteBook()
    {
        if (!noteBookUIDisplaying)
        {
            root.style.display = DisplayStyle.Flex;
            noteBookUIDisplaying = true;

            questLogButton.Focus();
            currentButton = questLogButton;
            UIEvent.OnUIQuestRefresh?.Invoke();
        }
        else
        {
            HideNoteBook();
            UIEvent.OnUIQuestRefresh?.Invoke();
        }
    }

    private void HideNoteBook()
    {
        if (noteBookUIDisplaying)
        {
            root.style.display = DisplayStyle.None;

            noteBookUIDisplaying = false;

            currentButton = null;

            root.Blur();

            activeQuestsListUI.ClearSelection();

            UIEvent.OnUIQuestRefresh?.Invoke();

        }
    }

    private void DisplayPauseMenuPage()
    {
        if (noteBookUIDisplaying)
        {
            pauseMenuPage.style.display = DisplayStyle.Flex;
            questLogPage.style.display = DisplayStyle.None;
            collectiblesPage.style.display = DisplayStyle.None;
            pauseMenuButton.Focus();
            currentButton = pauseMenuButton;

            noteBookUIDisplaying = true;
            UIEvent.OnUIQuestRefresh?.Invoke();
        }
    }

    private void DisplayQuestLog()
    {
        pauseMenuPage.style.display = DisplayStyle.None;
        questLogPage.style.display = DisplayStyle.Flex;
        collectiblesPage.style.display = DisplayStyle.None;
        currentButton = questLogButton;

        activeQuestsListUI.ClearSelection();

        activeQuestsListUI.selectionChanged += items =>
        {
            VisualElement selectedElement = activeQuestsListUI.Q<Button>("PinButton");

            selectedElement?.Focus();
        }; 

        noteBookUIDisplaying = true;
        UIEvent.OnUIQuestRefresh?.Invoke();
    }

    private void DisplayCollectibles()
    {
        pauseMenuPage.style.display = DisplayStyle.None;
        questLogPage.style.display = DisplayStyle.None;
        collectiblesPage.style.display = DisplayStyle.Flex;
        collectiblesButton.Focus();
        currentButton = collectiblesButton;

        noteBookUIDisplaying = true;
        UIEvent.OnUIQuestRefresh?.Invoke();
    }

    private void ReturnToButtonTab()
    {
        if(!noteBookUIDisplaying) { return; }

         activeQuestsListUI.ClearSelection();

         currentButton?.Focus();
    }

}
