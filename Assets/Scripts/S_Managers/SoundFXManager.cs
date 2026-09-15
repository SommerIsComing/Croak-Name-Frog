using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySound(SoundEffectSO sound, Transform spawnTransform)
    {
        AudioClip clip = sound.GetClip();
        if (clip == null) return;

        AudioSource audioSource = Instantiate(soundFXPrefab, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = sound.GetVolume();
        audioSource.pitch = sound.GetPitch();
        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }
}
