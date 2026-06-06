using UnityEngine;


public class MusicManager : MonoBehaviour
{
public static MusicManager musicManager;
private AudioSource audioSource;
public AudioClip[] songs;
public float volume;

    private void Awake()
    {
        if (musicManager == null)
        {
            musicManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
