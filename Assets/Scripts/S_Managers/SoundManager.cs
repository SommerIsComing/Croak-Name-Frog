using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum SoundType
{
    FOOTSTEP,
    JUMP,
    SWORD,
    LAND,
    STAFF, 
    HURT,
}

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundManager soundManager;
    private AudioSource audioSource;

    void Awake()
    {
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

    // Update is called once per frame
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        soundManager.audioSource.PlayOneShot(soundManager.soundList[(int)sound], volume);
    }
}
