using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    [Header("TEAM Boxes")]
    public GameObject markerImageEnemy;
    public GameObject markerImageFriendly;
    public static PlayerStats LocalPlayer;

    [Header("Player")]
    public GameObject playerObject; // mora bit mesh... ne dat objekta drugace tudi corutine umre oz skripta.

    [Header("TEAM + name")]
    public NetworkVariable<int> Team = new NetworkVariable<int>();
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();


    [Header("Death Handling")]
    public Collider[] collidersToDisable;    // main collider, etc.  // lahko das za kolko te hurta ce te zadane.
    public MonoBehaviour[] scriptsToDisable; // movement, attack, etc.

    [Header("Health Values")]
    public float maxHealth = 1000f;
    //public float currentHealth;

    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(
    0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server     );

    [Header("UI")]
    public Image healthBarFill;   // Assign your fill image here
    [Header("Respawn")]
    public float respawnDelay = 4f;   // 4 sekunde respawn

    private bool isDead = false;

    private CharacterMovement movement;

    public WeaponStats[] allWeaponsToReaload;

    public GameObject healthUIRoot;

    private ScoreManager scoreManager;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>(); // dobi CharacterMovement script
    }
    private void Start() // to ne stekam al se klice usakic al ne...
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        // Update UI when health changes
    }

    public void TakeDamage(float amount) // server to mora klicat vedno ko tejkas damage ampak ti moras to poklicat !!!! oz moras poslat serverju ? 
    {
        if (!IsServer) return; // only server updates health
        if (isDead) return;

        currentHealth.Value -= amount;

        if (IsOwner)
        {
            UpdateHealthUI();
            

        }

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            Die();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(DelayedSpawn());
        }

        if (!IsOwner)
        {
            healthUIRoot.SetActive(false); // hide others
        }

        // EVERYONE listens
        currentHealth.OnValueChanged += OnHealthChanged;

        // Only local player sets up local UI reference
        if (IsOwner)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager == null)
            {
                Debug.LogError("ScoreManager not found!");
            }
            LocalPlayer = this;
        }

        UpdateHealthUI(); // initial sync
    }

    // sepravi rabim 




    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            currentHealth.OnValueChanged -= OnHealthChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) return;
        if (isDead) return; // solv da ga ne teleporta okoli

        //UpdateHealthUI(); // ce te kej jebe healthbar mas tle updejt drugace ne rabis updejtat konstantno ampak samo ko tejkas dmg lahko zs insoectorjem.

    }

    private void OnHealthChanged(float oldValue, float newValue) // client... server ti spremeni health... ti zaznas in si spremenis UI health.
    {
        // Only update UI for LOCAL player
        if (IsOwner)
        {
            Debug.Log("My health changed... i updated my UI!!");
            UpdateHealthUI();
        }
    }
    private void UpdateHealthUI() // client
    {
        if (!IsOwner) return;

            Debug.Log(" i am owner of this health bar can you please LET  ME UPDATE IT FOR FUCK SAKE");

        if (healthBarFill != null)
        {
            float fill = (float) currentHealth.Value / maxHealth;
            healthBarFill.fillAmount = fill;
            Debug.Log(healthBarFill.fillAmount  +  " i update it to this much ..");

        }
    }

    private void Die() // client ig
    {
        Debug.Log("PLAYER DIED");
        isDead = true;

        ScoreManager.Instance.AddKillServerRpc(Team.Value);

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
    IEnumerator RespawnRoutine() // reloadej use weapone also   
    {
        isDead = true;

        EnableDisableColidersAndScripts(false);

        Transform spawn = RespawnManager.Instance.GetNextSpawnPoint(Team.Value); // poves value team u katerem si in pol te manager vrze na pravo mesto ....

        RespawnClientRpc(spawn.position);

        yield return new WaitForSeconds(respawnDelay);

        for (int i = 0; i < allWeaponsToReaload.Length; i++)
        {
            allWeaponsToReaload[i].deathReload();
        }


        currentHealth.Value = maxHealth;
        
        isDead = false;

        EnableDisableColidersAndScripts(true);
    } // server


    private IEnumerator DelayedSpawn()
    {
        // wait 1 frame so everything initializes
        yield return null;

        // wait until RespawnManager exists
        while (RespawnManager.Instance == null)
            yield return null;

        Transform spawn = RespawnManager.Instance.GetNextSpawnPoint(Team.Value);

        // teleport ONLY the owner client
        RespawnClientRpc(spawn.position, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { OwnerClientId }
            }
        });

        currentHealth.Value = maxHealth;
    }

    //public void StartSpawn()
    //{
    //    Transform spawn = RespawnManager.Instance.GetNextSpawnPoint(Team.Value); // poves value team u katerem si in pol te manager vrze na pravo mesto ....

    //    RespawnClientRpc(spawn.position);
    //}

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


    [ClientRpc] // v movement scriptu poklici da se spawna na random spawn point
    void RespawnClientRpc(Vector3 spawnPos, ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        //teleport him back to where he will spawn...
        movement.BeginRespawn(spawnPos);
        StartCoroutine(movement.FinishRespawn());
    }


}

