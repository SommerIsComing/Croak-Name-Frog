using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public AbilitySO ability;
    float cooldownTime;
    float activeTime;
    bool activateRequested;

    enum AbilityState { ready, active, cooldown }
    AbilityState state = AbilityState.ready;

    public void TriggerAbility()
    {
        activateRequested = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
            if (activateRequested)
            {
                state = AbilityState.active;
                activeTime = ability.activeTime;
                activateRequested = false;
            }
            break;
            case AbilityState.active:
            if (activeTime > 0)
            {
                activeTime -= Time.deltaTime;
            }
            else
            {
                state = AbilityState.cooldown;
                cooldownTime = ability.cooldown;
            }
            break;
            case AbilityState.cooldown:
            if (cooldownTime > 0)
            {
                cooldownTime -= Time.deltaTime;
            }
            else
            {
                state = AbilityState.ready;
            }
            break;
        }
    }
}
