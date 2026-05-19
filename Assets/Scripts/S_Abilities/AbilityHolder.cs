using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics.Tracing;

public class AbilityHolder : MonoBehaviour
{
    public enum AbilityState { ready, active, cooldown }

    [Serializable]
    public class AbilitySlot
    {
        public AbilitySO ability;

        public AbilityState state = AbilityState.ready;
        public float cooldownTime;
        public float activeTime;
        public bool activateRequested;
    }

    [SerializeField] private List<AbilitySlot> abilitySlots = new List<AbilitySlot>();
    public void TriggerAbility()
    {
        TriggerAbility(0);
    }

    public void TriggerAbility(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return;
        }

        AbilitySlot slot = abilitySlots[slotIndex];
        if (slot == null || slot.ability == null )
        {
            return;
        }

        if (slot.state != AbilityState.ready)
        {
            return;
        }
        slot.activateRequested = true;
    }

    public void TriggerAbilityByName(string abilityName)
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            if (abilitySlots[i].ability != null && abilitySlots[i].ability.name == abilityName)
            {
                TriggerAbility(i);
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            UpdateSlot(abilitySlots[i]);
        }
    }

    private void UpdateSlot(AbilitySlot slot)
    {
        if (slot == null || slot.ability == null)
        {
            return;
        }

        switch (slot.state)
        {
            case AbilityState.ready:
                if (slot.activateRequested)
                {
                    slot.activateRequested = false;

                    slot.ability.Activate(gameObject);

                    slot.activeTime = slot.ability.activeTime;

                    if (slot.activeTime > 0f)
                    {
                        slot.state = AbilityState.active;
                    }
                    else
                    {
                        slot.state = AbilityState.cooldown;
                        slot.cooldownTime = slot.ability.cooldown;
                    }
                }
                break;

            case AbilityState.active:
                if (slot.activeTime > 0f)
                {
                    slot.activeTime -= Time.deltaTime;
                }
                else
                {
                    slot.state = AbilityState.cooldown;
                    slot.cooldownTime = slot.ability.cooldown;
                }
                break;

            case AbilityState.cooldown:
                if (slot.cooldownTime > 0f)
                {
                    slot.cooldownTime -= Time.deltaTime;
                }
                else
                {
                    slot.state = AbilityState.ready;
                }
                break;
        }
    }

    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < abilitySlots.Count;
    }
}
