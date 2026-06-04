using System;
using UnityEngine;
using UnityEngine.Playables;

public class PlayCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineHolder;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggered Cutscene");
            
            //mute BG music
            musicManager.volume = 0f;
            
            //REPLACE THIS WITH A FOR-LOOP OR SOME SHIT, THIS IS SMOOTHED BRAINED
            //Searches for the player controller and deletes it before starting cutscene. Then does it again to assign player 2 and deletes it if it exist
            player1 = GameObject.Find("TruePlayer");
            player2 = GameObject.Find("Player2(Clone)");


            if (player1 != null)
            {
                //player1.transform.position = voidSpawn.position;
                Destroy(player1);
                Debug.Log("player 1 banished");
            }

            if (player2 != null)
            {
                //player2.transform.position = voidSpawn.position;
                Destroy(player2);
                Debug.Log("player 2 banished");
            }
            
            
            //Start the assigend cutscene timeline
            timelineHolder.Play();
        }
    } 
}
