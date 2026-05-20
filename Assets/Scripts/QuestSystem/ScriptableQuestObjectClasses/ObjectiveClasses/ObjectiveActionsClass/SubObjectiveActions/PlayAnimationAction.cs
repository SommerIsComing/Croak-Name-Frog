using JetBrains.Annotations;
using System;
using UnityEngine;

//objective action som spiller starter en animation
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Objective Actions/PlayAnimAction")]
public class PlayAnimationAction : ObjectiveAction
{
    public string animTriggerName;
    public override void ExecuteAction()
    {
        GameEvent.OnAnimNeeded?.Invoke(animTriggerName);
    }
}
