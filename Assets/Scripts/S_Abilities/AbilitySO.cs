using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AbilitySO : ScriptableObject
{
    public new string name;
    public float cooldown;
    public float activeTime;

    public virtual void Activate(GameObject parent)
    {
        // Implement the ability's effect here
    }

    public virtual void FixedActiveUpdate(GameObject parent)
    {
        // Called every FixedUpdate while the ability is active
    }

    public virtual void Deactivate(GameObject parent)
    {
        // Called when active time expires
    }

    public virtual bool IsActiveComplete(GameObject parent)
    {
        return false; // Default: rely on activeTime timer only
    }
}  
