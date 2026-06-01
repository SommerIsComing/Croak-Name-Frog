using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/Objective Actions/UnlockAbilityAction")]

public class UnlockAbilityAction : ObjectiveAction
{
    public string abilityName;

    public override void ExecuteAction()
    {
        GameEvent.OnAbilityUnlockNeeded?.Invoke(abilityName);
    }
}
