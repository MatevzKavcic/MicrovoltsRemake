using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GranaderBullet : NetworkBehaviour
{
    public float speed = 30f;
    public float lifeTime = 5f;

    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private LayerMask hitMask;

    private Rigidbody rb;
    public float TimeToExplode = 1.2f;

    private Boolean TimerStarted = false;

    private Collider bulletCollider;

    [SerializeField] private AudioClip bounceSound;

    public override void OnNetworkSpawn()
    {
        bulletCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {

            rb.isKinematic = false; // <-- FORCE IT (important)
            rb.linearVelocity = transform.forward * speed;
            rb.angularVelocity = Vector3.zero;
            Invoke(nameof(Despawn), lifeTime); // ce gre vec ku tolko casa ga despawnej.... da ne gre u neskončnost in wasta power

        }
    }


    public void IgnoreOwner(Collider ownerCollider)
    {
        if (ownerCollider == null) return;
        Physics.IgnoreCollision(bulletCollider, ownerCollider, true);
    }


    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (TimerStarted) return;

        if (bounceSound != null)
        {
            AudioSource.PlayClipAtPoint(bounceSound,transform.position);
        }
        if (!IsServer) return;


        if (collision.collider.GetComponent<PlayerStats>())
        {
            SpawnExplosion();
            Despawn();
            return;
        }

        //if the colision is a player then instantly despawn... and spawn explosion... othervise letit bounce make the logic work but figure out layers and so on...

        TimerStarted = true;

        StartCoroutine(FuseCoroutine());
    }
    
    IEnumerator FuseCoroutine()
    {
        yield return new WaitForSeconds(TimeToExplode);
        SpawnExplosion();
        Despawn();
    }

    void SpawnExplosion()
    {
        GameObject explosion = Instantiate(
            explosionPrefab,
            transform.position,
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