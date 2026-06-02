using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC_UI : MonoBehaviour
{
    public static NPC_UI npc_UI;
    
    VisualElement root;
    private VisualElement ui_npcImage;
    private VisualElement ui_textBox;
    private Label ui_dialogueText;
    private Label ui_npcNameText;

    private NPC_DialogueObject currentDialogue;
    private int currentDialogueIndex;
    private bool isDialogueDisplaying;

    private void OnEnable()
    {
        UIEvent.OnDialogueStart += StartDialogue;
        UIEvent.OnDialogueContinue += ContinueDialogue;
    }

    private void OnDisable()
    {
        UIEvent.OnDialogueStart -= StartDialogue;
        UIEvent.OnDialogueContinue -= ContinueDialogue;
    }

    private void Awake()
    {
        npc_UI = this;
        
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("DialogueContainer");

        ui_npcImage = root.Q<VisualElement>("NPC_Image");

        ui_textBox = root.Q<VisualElement>("TextBox");

        ui_npcNameText = ui_textBox.Q<Label>("NPC_NameText");

        ui_dialogueText = ui_textBox.Q<Label>("DialogueText");
    }

    private void Start()
    {
        root.style.display = DisplayStyle.None;
        isDialogueDisplaying = false;
    }

    private void StartDialogue(NPC_DialogueObject dialogue)
    {
        if(dialogue == null) { return; }

        currentDialogue = dialogue;

        currentDialogueIndex = 0;

        root.style.display = DisplayStyle.Flex;
        isDialogueDisplaying = true;

        GameEvent.OnDialogueInteractionStart?.Invoke();

        UpdateDialogue();
    }

    private void UpdateDialogue()
    {
        ui_dialogueText.text = currentDialogue.dialogueText[currentDialogueIndex];

        ui_npcImage.style.backgroundImage = new StyleBackground(currentDialogue.npcSpeakerImage);

        ui_npcNameText.text = currentDialogue.npcSpeakerName;

        isDialogueDisplaying = true;
    }

    public void ContinueDialogue()
    {
        if (!isDialogueDisplaying) { return; }

        currentDialogueIndex++;

        isDialogueDisplaying = true;

        if (currentDialogueIndex >= currentDialogue.dialogueText.Count)
        {
            EndDialogue();
            return;
        }

        UpdateDialogue();
        
    }

    private void EndDialogue()
    {
        root.style.display = DisplayStyle.None;
        isDialogueDisplaying = false;
        currentDialogue = null;
        currentDialogueIndex = 0;

        GameEvent.OnDialogueInteractionEnd?.Invoke();
        UIEvent.OnUIQuestRefresh?.Invoke();
    }

    public bool IsDialogueDisplaying()
    {
        return isDialogueDisplaying;
    }
}
