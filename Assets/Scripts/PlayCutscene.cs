using UnityEngine;
using UnityEngine.Playables;

public class PlayCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineHolder;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private GameObject player;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggered Cutscene");
            
            //mute BG music
            musicManager.volume = 0f;
            
            //REPLACE THIS WITH A FOR-LOOP OR SOME SHIT, THIS IS SMOOTHED BRAINED
            //Searches for the player controller and deletes it before starting cutscene. Then does it again to assign player 2 and deletes it if it exist
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("Destroyed 1 players");
                Destroy(player);
            }
            
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("Destroyed 2 players");
                Destroy(player);
            }
            
            //Start the assigend cutscene timeline
            timelineHolder.Play();
        }
    } 
}
