using UnityEngine;

[CreateAssetMenu(fileName = "Shooter", menuName = "Abilities/Shooter")]
public class Shooter : AbilitySO
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Vector3 bulletSpawnOffset = new Vector3(0, 0, 1f);
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 2f;

    public override void Activate(GameObject parent)
    {
        if (parent == null || bulletPrefab == null)
        {
            return;
        }

        Transform t = parent.transform;
        Vector3 spawnPos = t.TransformPoint(bulletSpawnOffset);
        Quaternion spawnRot = Quaternion.LookRotation(t.forward, Vector3.up) * bulletPrefab.transform.rotation;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, spawnRot);
        MagicBullet magicBullet = bullet.GetComponent<MagicBullet>();
        if (magicBullet != null)
        {
            magicBullet.Initialize(t.forward, bulletSpeed, bulletLifetime);
        }
        else
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = t.forward * bulletSpeed;
                
            }
            Destroy(bullet, bulletLifetime);
        }
    }
}
