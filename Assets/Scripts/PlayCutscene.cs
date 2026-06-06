using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class PlayCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineHolder;
    [SerializeField] private GameObject musicManager;
    [SerializeField] private GameObject uiManager;
    [SerializeField] private GameObject playerHUDRoot;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    private void Awake()
    {
        //REPLACE THIS WITH A FOR-LOOP OR SOME SHIT, THIS IS SMOOTHED BRAINED
        //Searches for the player controller and deletes it before starting cutscene. Then does it again to assign player 2 and deletes it if it exist
        player1 = GameObject.Find("TruePlayer");
        player2 = GameObject.Find("Player2(Clone)");
        musicManager = GameObject.FindGameObjectWithTag("MusicManager");
        uiManager = GameObject.FindGameObjectWithTag("UIManager");
        playerHUDRoot = GameObject.FindGameObjectWithTag("PlayerHUDroot");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggered Cutscene");
            
            //mute BG music
            musicManager.GetComponent<MusicManager>().volume = 0f;

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

            uiManager.GetComponent<UI_Manager>().HideNoteBook();
            playerHUDRoot.GetComponent<QuestPin>().HideQuestPins();

            //Start the assigend cutscene timeline
            timelineHolder.Play();
        }
    } 
}
