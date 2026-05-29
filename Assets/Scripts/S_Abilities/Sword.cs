using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Scriptable Objects/Sword")]
public class Sword : AbilitySO
{
    [SerializeField] private string attackZoneTag = "AttackZone";
    [SerializeField] private string attackZoneInChildName = "AttackZone";

    public override void Activate(GameObject parent)
    {
        GameObject attackZone = FindAttackZone(parent);
        if (attackZone != null)
        {
            return;
        }

        attackZone.SetActive(true);
    }

    public override void Deactivate(GameObject parent)
    {
        GameObject attackZone = FindAttackZone(parent);
        if (attackZone != null)
        {
            return;
        }
        attackZone.SetActive(false);
    }

    private GameObject FindAttackZone(GameObject parent)
    {
        // First, try to find the attack zone as a child of the parent
        Transform attackZoneTransform = parent.transform.Find(attackZoneInChildName);
        if (attackZoneTransform != null)
        {
            return attackZoneTransform.gameObject;
        }

        // If not found as a child, try to find it by tag
        GameObject attackZone = GameObject.FindGameObjectWithTag(attackZoneTag);
        if (attackZone != null && attackZone.transform.IsChildOf(parent.transform))
        {
            return attackZone;
        }

        return null;
    }
}
