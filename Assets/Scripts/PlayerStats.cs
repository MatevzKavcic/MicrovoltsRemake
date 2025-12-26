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

    private CharacterMovement movement;


    void Awake()
    {
        movement = GetComponent<CharacterMovement>(); // dobi CharacterMovement script
    }
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
        if (isDead)  return; 

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
            float fill = currentHealth.Value/ maxHealth;
            healthBarFill.fillAmount = fill;
        }
    }

    private void Die()
    {
        Debug.Log("PLAYER DIED");
        isDead = true;

        StartCoroutine(RespawnRoutine());

    }
    /*
     disabli coliderje,
    corutine da waitas en cajt,
    u CharacterMovement poklici API da ga transforma na pravo mesto
    dej mu helth nazaj,
    dej mu da je ziv
    enabli skripte
     */
    IEnumerator RespawnRoutine() 
    {
        EnableDisableColidersAndScripts(false);

        yield return new WaitForSeconds(respawnDelay);

        Transform spawn = RespawnManager.Instance.GetRandomSpawnPoint();

        RespawnClientRpc(spawn.position); // dej mu spawn point da ga poklicu u skripti

        currentHealth.Value = maxHealth;
        ResetHealthServerRpc(); // i gues da bi moglo bit to prav zato da resetas health na serverju?

        isDead = false; // dej mu da je ziv nazaj

        EnableDisableColidersAndScripts(true);
    }


    [ClientRpc] // v movement scriptu poklici da se spawna na random spawn point
    void RespawnClientRpc(Vector3 spawnPos, ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;
        movement.BeginRespawn(spawnPos);
        StartCoroutine(movement.FinishRespawn());
    }

    private void EnableDisableColidersAndScripts(bool alive) // colidersi in scripte disabli enabli da nemore streljat itd...
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

    [ServerRpc]
    private void ResetHealthServerRpc()
    {
        currentHealth.Value = maxHealth;
    }
}

