using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    // No fixed sound field - each caller passes the SoundEffectSO it wants played.
    public void Play(SoundEffectSO sound)
    {
        SoundFXManager.instance.PlaySound(sound, transform);
    }
}
