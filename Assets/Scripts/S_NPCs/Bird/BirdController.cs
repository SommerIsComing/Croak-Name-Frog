using UnityEngine;

// Styrer fuglen i spillet, gør den interaktiv samt afspiller en animation når alle edderkopper er dræbt
public class BirdController : MonoBehaviour, Interactable
{
    public bool isInteractable = false;
    public string npcName;

    private void OnEnable()
    {
        GameEvent.OnAnimNeeded += PlayAnim;

    }

    private void OnDisable()
    {
        GameEvent.OnAnimNeeded -= PlayAnim;

    }

    public void PlayAnim(string animTriggerName)
    {
        if (animTriggerName == "spidersCleared")
        {
            GetComponent<Animator>().SetTrigger(animTriggerName);
            isInteractable = true;
        }
    }

    public void Interact()
    {
        if (isInteractable)
        {
            QuestEvents.OnNPCTalkedTo?.Invoke(npcName);
            Debug.Log("Interacted with the player");
            isInteractable = false;
        }
    }
}
