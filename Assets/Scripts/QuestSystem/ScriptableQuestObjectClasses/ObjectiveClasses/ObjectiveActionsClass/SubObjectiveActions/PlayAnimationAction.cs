using JetBrains.Annotations;
using UnityEngine;

public class PlayAnimationAction : ObjectiveAction
{
    public Animator animator;

    public string paramenterName;
    public override void ExecuteAction()
    {
        animator.SetTrigger(paramenterName);
    }
}
