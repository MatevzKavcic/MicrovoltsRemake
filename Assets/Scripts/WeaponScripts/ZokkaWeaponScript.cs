using UnityEngine;

public class ZokkaWeaponScript : WeaponStats
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Hitscan Settings")]
    public LayerMask hitMask;        // What layers you can hit

    protected override void Aim() // pow poveci malo

    {
        throw new System.NotImplementedException();
    }



    protected override void ServerShootLogic(Vector3 baseDir) // base dir je Aim direction ze zracunau in vrze noter v to metodo.
    {

        Vector3 origin = firePoint.position;

        Vector3 endPoint = origin + baseDir * maxDistance;

        // server raycast (authoritative)
        if (Physics.Raycast(origin, baseDir, out RaycastHit hit, maxDistance, hitMask))
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
