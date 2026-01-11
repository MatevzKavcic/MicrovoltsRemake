using Unity.Netcode;
using UnityEngine;

public class ZokkaWeaponScript : WeaponStats
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Hitscan Settings")]
    public LayerMask hitMask;        // What layers you can hit

    [SerializeField] private GameObject zookaProjectilePrefab;

    protected override void Aim() // pow poveci malo

    {
        throw new System.NotImplementedException();
    }

    protected override void ServerShootLogic(Vector3 baseDir) // base dir je Aim direction ze zracunau in vrze noter v to metodo.
    {

        GameObject projectile = Instantiate(
        zookaProjectilePrefab,
        firePoint.position,
        Quaternion.LookRotation(baseDir)
    );

        projectile.GetComponent<NetworkObject>().Spawn(true);

    }
}
