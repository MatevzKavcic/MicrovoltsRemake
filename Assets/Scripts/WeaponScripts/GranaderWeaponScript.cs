using Unity.Netcode;
using UnityEngine;

public class GranaderWeaponScript : WeaponStats
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject granaderBulletPrefab;
    public LayerMask hitMask;        // What layers you can hit
    protected Collider ownerCollider;

    protected override void Aim() // pow poveci malo

    {
        throw new System.NotImplementedException();
    }

    protected virtual void Awake()
    {
        ownerCollider = GetComponentInParent<Collider>(); // dobi parent collider muhahahah //copypaste od bazoke
    }

    protected override void ServerShootLogic(Vector3 baseDir) // base dir je Aim direction ze zracunau in vrze noter v to metodo.
    {

        GameObject projectile = Instantiate(
        granaderBulletPrefab,
        firePoint.position,
        Quaternion.LookRotation(baseDir)
    );

        projectile.GetComponent<NetworkObject>().Spawn(true);

        GranaderBullet bullet = projectile.GetComponent<GranaderBullet>();

        Debug.Log(ownerCollider.name);

        bullet.IgnoreOwner(ownerCollider);

    }
}
