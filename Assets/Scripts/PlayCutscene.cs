using UnityEngine;
using UnityEngine.Playables;

public class PlayCutscene : MonoBehaviour
{
    [SerializeField] public PlayableDirector timelineHolder;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggered Cutscene");
            timelineHolder.Play();
        }
    } 
}
