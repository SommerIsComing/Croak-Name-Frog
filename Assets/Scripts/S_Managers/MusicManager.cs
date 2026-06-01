using UnityEngine;


public class MusicManager : MonoBehaviour
{
private AudioSource audioSource;
public AudioClip[] songs;
public float volume; 
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
        ChangeSong(Random.Range(0, songs.Length));
    }

    void Update()
    {
        audioSource.volume = volume; 
        if (!audioSource.isPlaying)
        ChangeSong(Random.Range(0, songs.Length));
    }

    public void ChangeSong(int songPicked)
    {
        audioSource.clip = songs[songPicked];
        audioSource.Play();
    }
}
