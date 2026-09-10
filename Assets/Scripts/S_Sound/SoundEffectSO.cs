using UnityEngine;

[CreateAssetMenu(menuName = "Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] clips;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float minVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float maxVolume = 1f;

    [Header("Pitch")]
    [SerializeField] private bool randomizePitch;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.1f;

    public AudioClip GetClip() => (clips == null || clips.Length == 0) ? null : clips[Random.Range(0, clips.Length)];
    public float GetVolume() => Random.Range(minVolume, maxVolume);
    public float GetPitch() => randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
}
