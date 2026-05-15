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
}  
