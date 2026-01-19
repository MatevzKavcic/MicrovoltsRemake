using System;
using Unity.Netcode;
using UnityEngine;

public class ZookaBullet : NetworkBehaviour
{
    public float speed = 30f;
    public float lifeTime = 5f;

    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody rb;

    private Collider bulletCollider;



    public override void OnNetworkSpawn()
    {
        bulletCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {
            
            rb.linearVelocity = transform.forward * speed;
            rb.angularVelocity = Vector3.zero;
            Invoke(nameof(Despawn), lifeTime); // ce gre vec ku tolko casa ga despawnej.... da ne gre u neskonènost in wasta power

        }
    }


    public void IgnoreOwner(Collider ownerCollider)
    {
        if (ownerCollider == null) return;
        Physics.IgnoreCollision(bulletCollider, ownerCollider, true);
    }


    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (!IsServer) return;

        SpawnExplosion(collision.contacts[0].point);
        Despawn();
    }
    void SpawnExplosion(Vector3 pos)
    {
        GameObject explosion = Instantiate(
            explosionPrefab,
            pos,
            Quaternion.identity
        );

        explosion.GetComponent<NetworkObject>().Spawn(true);
    }

    void Despawn()
    {
        if (IsServer && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }

}