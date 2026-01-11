using System;
using Unity.Netcode;
using UnityEngine;

public class ZookaBullet : NetworkBehaviour
{
    public float speed = 30f;
    public float lifeTime = 5f;

    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private LayerMask hitMask;

    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {
            rb.linearVelocity = transform.forward * speed;
            rb.angularVelocity = Vector3.zero;
            Invoke(nameof(Despawn), lifeTime); // ce gre vec ku tolko casa ga despawnej.... da ne gre u neskonènost in wasta power

        }
    }

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (!IsServer) return;

        if (((1 << collision.gameObject.layer) & hitMask) == 0)
            return;

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