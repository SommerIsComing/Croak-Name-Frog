using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayFootstep : MonoBehaviour
{
    public void PlaySound()
    {
        SoundManager.PlaySound(SoundType.FOOTSTEP);
    }
}
