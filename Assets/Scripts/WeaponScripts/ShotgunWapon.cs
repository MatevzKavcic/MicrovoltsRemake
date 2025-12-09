using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ShotgunWapon : WeaponStats

{
    [Header("Hitscan Settings")]
    public LayerMask hitMask;        // What layers you can hit

    [Header("Shotgun Settings")]
    public int pelletCount = 8;
    public float spreadAngle = 5f;      // degrees of random spread
    public float tracerDuration = 0.5f;

   

    protected override void ServerShootLogic(Vector3 baseDir) // base dir je Aim direction ze zracunau in vrze noter v to metodo.
    {

        Vector3 origin = firePoint.position;

        for (int i = 0; i <= pelletCount; i++)
        {
            Vector3 dir = GetSpreadDirection(baseDir);

            Vector3 endPoint = origin + dir * maxDistance;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, hitMask))
            {
                endPoint = hit.point;


                var stats = hit.collider.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage);
                }
                Debug.Log($"Pellet hit {hit.collider.name} for {damage} damage");
            }

            SpawnTracerClientRpc(origin, endPoint);

        }
    }


    Vector3 GetSpreadDirection(Vector3 baseDir)
    {
        // Make a random rotation around base direction
        // random yaw & pitch within spreadAngle
        float yaw = Random.Range(-spreadAngle, spreadAngle);
        float pitch = Random.Range(-spreadAngle, spreadAngle);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 spreadDir = rot * baseDir;
        return spreadDir.normalized;
    }

    protected override void Aim()
    {
        throw new System.NotImplementedException();  // nemores aimat z shotijem
    }
}