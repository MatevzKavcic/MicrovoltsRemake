using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class RifleWeapon : WeaponStats

{

    protected override void Aim() // pow poveci malo

    {
        throw new System.NotImplementedException();
    }



    protected override void ServerShootLogic(Vector3 baseDir) // base dir je Aim direction ze zracunau in vrze noter v to metodo.
    {

        Vector3 origin = firePoint.position;

        Vector3 endPoint = origin + baseDir * maxDistance;

       
        if (Physics.Raycast(origin, baseDir, out RaycastHit hit, maxDistance, layerMask))
        {
            endPoint = hit.point;

            // damage player if they have PlayerStats
            var stats = hit.collider.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
            Debug.Log($"Pellet hit {hit.collider.name} for {damage} damage");
        }

        // tell ALL clients to show tracer
        SpawnTracerClientRpc(origin, endPoint);
    }

   
}