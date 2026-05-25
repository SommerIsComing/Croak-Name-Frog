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

    private VisualElement pauseMenuPage;
    private VisualElement questLogPage;
    private VisualElement collectiblesPage;

    public bool noteBookUIDisplaying = true;

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
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        buttonRoot = root.Q<VisualElement>("BookMarkButtonsContainer");
        pageRoot = root.Q<VisualElement>("PagesContainer");

        pauseMenuButton = buttonRoot.Q<Button>("PauseMenuButton");
        questLogButton = buttonRoot.Q<Button>("QuestLogButton");
        collectiblesButton = buttonRoot.Q<Button>("CollectiblesButton");

        pauseMenuPage = pageRoot.Q<VisualElement>("PauseMenuPage");
        questLogPage = pageRoot.Q<VisualElement>("QuestLogPage");
        collectiblesPage = pageRoot.Q<VisualElement>("CollectiblesPage");

        pauseMenuButton.clicked += DisplayPauseMenuPage;
        questLogButton.clicked += DisplayQuestLog;
        collectiblesButton.clicked += DisplayCollectibles;

        UIEvent.OnPauseMenuNeeded += DisplayNoteBook;

        HideNoteBook();
    }

    private void DisplayNoteBook()
    {
        if (!noteBookUIDisplaying)
        {
            root.style.display = DisplayStyle.Flex;
            noteBookUIDisplaying = true;
        }
        else
        {
            HideNoteBook();
        }
    }

    private void HideNoteBook()
    {
        if (noteBookUIDisplaying)
        {
            root.style.display = DisplayStyle.None;

            noteBookUIDisplaying = false;
        }
    }

    private void DisplayPauseMenuPage()
    {
        if (noteBookUIDisplaying)
        {
            pauseMenuPage.style.display = DisplayStyle.Flex;
            questLogPage.style.display = DisplayStyle.None;
            collectiblesPage.style.display = DisplayStyle.None;

            noteBookUIDisplaying = true;
        }
    }

    private void DisplayQuestLog()
    {
        pauseMenuPage.style.display = DisplayStyle.None;
        questLogPage.style.display = DisplayStyle.Flex;
        collectiblesPage.style.display = DisplayStyle.None;

        noteBookUIDisplaying = true;
    }

    private void DisplayCollectibles()
    {
        pauseMenuPage.style.display = DisplayStyle.None;
        questLogPage.style.display = DisplayStyle.None;
        collectiblesPage.style.display = DisplayStyle.Flex;

        noteBookUIDisplaying = true;
    }

}
