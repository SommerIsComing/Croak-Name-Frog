using UnityEngine;
using System;

//to play sound effects call the function "SoundManager.PlaySound(SoundType.XXXX);", with the XXXX being the sound type from the enum below, from any script where sound should play
//to add more sound types just add it to the enum below and assign the clips to the sound manager in the inspector. The arrays should update automatically in the editor when reenabling the manager

public enum SoundType
{
    FOOTSTEP,
    JUMP,
    SWORD,
    LAND,
    STAFF, 
    HURT,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager soundManager;
    private AudioSource audioSource;

    void Awake()
    {
        //Ensure a SoundManager exists and there is only one and that it persists across loads (we might not need DontDestroyOnLoad. This is TBD)
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1) //Function to play random sound from the clips array
    {
        AudioClip[] clips = soundManager.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        soundManager.audioSource.PlayOneShot(randomClip, volume);
    }

#if UNITY_EDITOR //Editor utility. If in editor, update the SoundList array names with the names in SoundType enum, in the inspector
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i =0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable] //Don't really understand what mr. tutorial guy said here yet, but it's to do with the setting of array names in the inspector
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; } //Getter to get the sounds from AudioClip array to play? Seems fucky...
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
