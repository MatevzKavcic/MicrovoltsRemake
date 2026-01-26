using System;
using Unity.Netcode;
using UnityEngine;

public class ExplosionZooka : NetworkBehaviour
{
    public float damage = 150f;
    public float radius = 5f;
    public LayerMask damageMask;

    public float lifeTime = 0.2f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // server authorative.... samo ce si server handlas use bulete i gues ... oz tle ze poves da bo take damage na serverju

        DealDamage();
        Invoke(nameof(Despawn), lifeTime);
    }

    void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            damageMask
        );

        foreach (Collider hit in hits)
        {
            PlayerStats stats = hit.GetComponent<PlayerStats>(); // ce si player tejkej dmg drugace nic.... alpa naredi nek paintwork .... muahahhaha
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
        }
    }

    void Despawn()
    {
        if (NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
}
