using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class WeaponSwitcher : NetworkBehaviour
{
   
    public enum WeaponType { Melee, Rifle, Shotgun, Sniper, Zooka, Granader}


    [Header("References")]
    public Animator animator;

    public GameObject meleeWeaponMesh;
    public GameObject rifleWeaponMesh;
    public GameObject shotgunWeaponMesh;
    public GameObject sniperWeaponMesh;
    public GameObject zookaWeaponMesh;
    public GameObject granaderWeaponMesh;


    [Header("CrosshairReferences")]
    public GameObject meleWeaponCrosshair;
    public GameObject rifleWeaponCrosshair;
    public GameObject shotgunWeaponCrosshair;
    public GameObject sniperWeaponCrosshair;
    public GameObject sniperWeaponCrosshairZoomed;
    public GameObject zookaWeaponCrosshair;
    public GameObject granaderWeaponCrosshair;


    public WeaponStats rifleWeapon;
    public WeaponStats shotgunWeapon;
    public WeaponStats meleeWeapon;
    public WeaponStats sniperWeapon;
    public WeaponStats zookaWeapon;
    public WeaponStats granaderWeapon;


    public NetworkVariable<WeaponType> NetworkWeapon =
    new NetworkVariable<WeaponType>(
        WeaponType.Melee,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    protected CinemachineCamera virtualCam;

    private WeaponStats activeWeaponStats;

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Melee;


    private void Start()
    {
        virtualCam = FindFirstObjectByType<CinemachineCamera>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            RequestWeaponChangeServerRpc(WeaponType.Melee);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            RequestWeaponChangeServerRpc(WeaponType.Rifle);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            RequestWeaponChangeServerRpc(WeaponType.Shotgun);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            RequestWeaponChangeServerRpc(WeaponType.Sniper);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            RequestWeaponChangeServerRpc(WeaponType.Zooka);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            RequestWeaponChangeServerRpc(WeaponType.Granader);
    }

    public override void OnNetworkSpawn()
    {
        NetworkWeapon.OnValueChanged += OnWeaponChanged;

        // Apply current weapon when spawning
        OnWeaponChanged(NetworkWeapon.Value, NetworkWeapon.Value);
    }

    public override void OnNetworkDespawn()
    {
        NetworkWeapon.OnValueChanged -= OnWeaponChanged;
    }

    /*
     * Equipi visuale In animator spremeni da bo correct
     * 
     * Pol gres LOCAL in menjas crosshair
     */
    private void OnWeaponChanged(WeaponType oldWeapon, WeaponType newWeapon) // core logic
    {

        if (activeWeaponStats != null)
        {
            activeWeaponStats.OnUnequip();
        }


        EquipWeapon(newWeapon); // to je visual
        animator.SetInteger("WeaponType", (int)newWeapon); // animator da ve kaj se dogaja

        if (!IsOwner) return;

        //sniperWeaponCrosshairZoomed.SetActive(false); // quickfix da vedno ko menjas da se ti crosshair zbrise... in pol se zoom nekko moram...

        ChangeCrosshair(newWeapon);

        if (activeWeaponStats != null) // to nima smisla... poglej da ko spremenis weapon da ga reloadas... 
        {
            activeWeaponStats.isActive = false;
            activeWeaponStats.TryReaload();
        }

        activeWeaponStats = GetWeaponStats(newWeapon);
        activeWeaponStats.isActive = true;
        activeWeaponStats.CancelReload();
    }

    private WeaponStats GetWeaponStats(WeaponType type)
    {
        return type switch
        {
            WeaponType.Melee => meleeWeapon,
            WeaponType.Rifle => rifleWeapon,
            WeaponType.Shotgun => shotgunWeapon,
            WeaponType.Sniper => sniperWeapon,
            WeaponType.Zooka => zookaWeapon,
            WeaponType.Granader => granaderWeapon,
            _ => meleeWeapon
        };
    }

    [ServerRpc]
    private void RequestWeaponChangeServerRpc(WeaponType newWeapon)
    {
        NetworkWeapon.Value = newWeapon;
    }


    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
        // Toggle visibility
        meleeWeaponMesh.SetActive(newWeapon == WeaponType.Melee);
        rifleWeaponMesh.SetActive(newWeapon == WeaponType.Rifle);
        shotgunWeaponMesh.SetActive(newWeapon == WeaponType.Shotgun);
        sniperWeaponMesh.SetActive(newWeapon == WeaponType.Sniper);
        zookaWeaponMesh.SetActive(newWeapon == WeaponType.Zooka);
        granaderWeaponMesh.SetActive(newWeapon == WeaponType.Granader);


    }

    public void ChangeCrosshair(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;

        // Toggle visibility
        meleWeaponCrosshair.SetActive(newWeapon == WeaponType.Melee);
        rifleWeaponCrosshair.SetActive(newWeapon == WeaponType.Rifle);
        shotgunWeaponCrosshair.SetActive(newWeapon == WeaponType.Shotgun);
        sniperWeaponCrosshair.SetActive(newWeapon == WeaponType.Sniper);
        zookaWeaponCrosshair.SetActive(newWeapon == WeaponType.Zooka);
        granaderWeaponCrosshair.SetActive(newWeapon == WeaponType.Granader);


    }

} 