using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AbilityHolder : MonoBehaviour
{
    public enum AbilityState { ready, active, cooldown }

    [Serializable]
    public class AbilitySlot
    {
        public AbilitySO ability;
        public bool isUnlocked = false;

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

        if (!slot.isUnlocked)
        {
            return;
        }

        if (slot.state != AbilityState.ready)
        {
            return;
        }
        slot.activateRequested = true;
    }

    public void UnlockAbilityByName(string abilityName)
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            if (abilitySlots[i].ability != null && abilitySlots[i].ability.name == abilityName)
            {
                abilitySlots[i].isUnlocked = true;
                return;
            }
        }
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

    void FixedUpdate()
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            AbilitySlot slot = abilitySlots[i];
            if (slot == null || slot.ability == null)
            {
                continue;
            }

            if (slot.state == AbilityState.active)
            {
                slot.ability.FixedActiveUpdate(gameObject);
            }
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

                    slot.activeTime = 0f;

                    if (slot.ability.activeTime > 0f)
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
                slot.activeTime += Time.deltaTime;

                if (slot.activeTime >= slot.ability.activeTime || slot.ability.IsActiveComplete(gameObject))
                {
                    slot.ability.Deactivate(gameObject);
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
