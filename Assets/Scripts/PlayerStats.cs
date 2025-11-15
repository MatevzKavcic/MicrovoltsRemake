using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerObject;
    [Header("Death Handling")]
    public Collider[] collidersToDisable;    // main collider, etc.
    public MonoBehaviour[] scriptsToDisable; // movement, attack, etc.

    [Header("Health Values")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("UI")]
    public Image healthBarFill;   // Assign your fill image here
    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 4f;   // 4 sekunde respawn

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateHealthUI(); // ce te kej jebe healthbar mas tle updejt drugace ne rabis updejtat konstantno ampak samo ko tejkas dmg lahko zs insoectorjem.

        if (currentHealth<=0)
        {
            Die();
        }
    }
    public void TakeDamage(float amount)
    {
        if (isDead) { return; }

        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();

        }
    }
    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        UpdateHealthUI();
    }
    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            float fill = currentHealth / maxHealth;
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

        // Move to respawn point
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log("PLAYER RESPAWNED");
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

