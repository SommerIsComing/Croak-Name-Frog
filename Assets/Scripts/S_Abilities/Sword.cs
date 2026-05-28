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
    {}
}
