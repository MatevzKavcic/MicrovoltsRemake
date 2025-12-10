using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    [Header("Player")]
    public GameObject playerObject; // mora bit mesh... ne dat objekta drugace tudi corutine umre oz skripta.
    [Header("Death Handling")]
    public Collider[] collidersToDisable;    // main collider, etc.  // lahko das za kolko te hurta ce te zadane.
    public MonoBehaviour[] scriptsToDisable; // movement, attack, etc.

    [Header("Health Values")]
    public float maxHealth = 1000f;
    //public float currentHealth;


    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(
    0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);

    [Header("UI")]
    public Image healthBarFill;   // Assign your fill image here
    [Header("Respawn")]
    public float respawnDelay = 4f;   // 4 sekunde respawn

    private bool isDead = false;




    private void Start()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        // Update UI when health changes
        currentHealth.OnValueChanged += OnHealthChanged; // naredi server callback  v to noter ker ga pac ima na ValueCahnge
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return; // solv da ga ne teleporta okoli

        UpdateHealthUI(); // ce te kej jebe healthbar mas tle updejt drugace ne rabis updejtat konstantno ampak samo ko tejkas dmg lahko zs insoectorjem.

        if (currentHealth.Value <=0)
        {
            Die();
        }
    }
    private void OnHealthChanged(float oldValue, float newValue)
    {
        // Only update UI for LOCAL player
        if (IsOwner)
        {
            UpdateHealthUI();
        }
    }
    public void TakeDamage(float amount)
    {
        if (!IsServer) return; // only server updates health
        if (isDead) { return; }

        currentHealth.Value -= amount;

        if (currentHealth.Value <= 0)
        {
                currentHealth.Value = 0;
                Die();
        }
    }
    public void UpdateHealth(float amount)
    {
        if (!IsServer) return; // only server updates health

        currentHealth.Value += amount;
        UpdateHealthUI();
    }
    private void UpdateHealthUI()
    {
        if (!IsOwner) return;

        if (healthBarFill != null)
        {
            float fill = currentHealth .Value/ maxHealth;
            healthBarFill.fillAmount = fill;
        }
    }

    private void Die()
    {
        Debug.Log("PLAYER DIED");
        isDead = true;

        Debug.Log("respawn delay function");

        SetAliveState(false);
        StartCoroutine(RespawnAfterDelay());

        // Respawn

    }
    IEnumerator RespawnAfterDelay()
    {
        // Wait 4 seconds
        playerObject.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        // Reset health
        


        // Reset velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
         // transform to a random spawn point
        Transform spawn = RespawnManager.Instance.GetRandomSpawnPoint();

        if (spawn != null)
        {
            transform.position = spawn.position;
        }
        else
        {
            Debug.LogWarning("No spawn points found in RespawnManager!");
        }

        isDead = false;
        currentHealth.Value = maxHealth;
        SetAliveState(true);

    }

    private void SetAliveState(bool alive)
    {
        // Show/hide character mesh
        if (playerObject != null)
            playerObject.SetActive(alive);

        // Enable/disable colliders
        if (collidersToDisable != null)
        {
            foreach (var col in collidersToDisable)
            {
                if (col != null)
                    col.enabled = alive;
            }
        }

        // Enable/disable movement/attack scripts
        if (scriptsToDisable != null)
        {
            foreach (var s in scriptsToDisable)
            {
                if (s != null)
                    s.enabled = alive;
            }
        }
    }
}

