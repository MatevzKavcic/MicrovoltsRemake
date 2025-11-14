using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Values")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("UI")]
    public Image healthBarFill;   // Assign your fill image here
    public Transform respawnPoint;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateHealthUI(); // ce te kej jebe healthbar mas tle updejt drugace ne rabis updejtat konstantno ampak samo ko tejkas dmg lp lahko noc

        if (currentHealth<=0)
        {
            Die();
        }
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
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

        // Respawn
        transform.position = respawnPoint.position;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
}

