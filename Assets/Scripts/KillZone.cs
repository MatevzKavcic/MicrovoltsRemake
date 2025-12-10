using Unity.Netcode;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;   // Only server handles kill zone

        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats == null) return;

            // Instantly kill the player
            stats.TakeDamage(9999f);

            Debug.Log("Player entered kill zone.");
        }
    }
}