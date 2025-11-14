using UnityEngine;

public class KillZone : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb) rb.linearVelocity = Vector3.zero;

            PlayerStats stats = other.GetComponent<PlayerStats>();

            stats.TakeDamage(1000f);


            other.transform.position = respawnPoint.position;

            Debug.Log("Player respawned from kill zone.");



        }
    }
}
