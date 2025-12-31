using System;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
   
    public enum WeaponType { Melee, Rifle, Shotgun, Sniper}


    [Header("References")]
    public Animator animator;

    public GameObject meleeWeaponMesh;
    public GameObject rifleWeaponMesh;
    public GameObject shotgunWeaponMesh;
    public GameObject sniperWeaponMesh;

    [Header("CrosshairReferences")]
    public GameObject meleWeaponCrosshair;
    public GameObject rifleWeaponCrosshair;
    public GameObject shotgunWeaponCrosshair;
    public GameObject sniperWeaponCrosshair;


    public WeaponStats rifleWeapon;
    public WeaponStats shotgunWeapon;
    public WeaponStats meleeWeapon;
    public WeaponStats sniperWeapon;


    private WeaponStats activeWeaponStats;

    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Melee;

    void Start()
    {
        EquipWeapon(currentWeapon);
        activeWeaponStats = meleeWeapon;
    }

    void Update()
    {
        // Switch weapons with number keys (for testing)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switchToWeapon(WeaponType.Melee, 1, meleeWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            switchToWeapon(WeaponType.Rifle, 2,rifleWeapon);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            switchToWeapon(WeaponType.Shotgun, 3, shotgunWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            switchToWeapon(WeaponType.Sniper, 4, sniperWeapon);
        }
    }

    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
        // Toggle visibility
        meleeWeaponMesh.SetActive(newWeapon == WeaponType.Melee);
        rifleWeaponMesh.SetActive(newWeapon == WeaponType.Rifle);
        shotgunWeaponMesh.SetActive(newWeapon == WeaponType.Shotgun);
        sniperWeaponMesh.SetActive(newWeapon == WeaponType.Sniper);

    }



    public void ChangeCrosshair(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;

        // Toggle visibility
        meleWeaponCrosshair.SetActive(newWeapon == WeaponType.Melee);
        rifleWeaponCrosshair.SetActive(newWeapon == WeaponType.Rifle);
        shotgunWeaponCrosshair.SetActive(newWeapon == WeaponType.Shotgun);
        sniperWeaponCrosshair.SetActive(newWeapon == WeaponType.Sniper);

    }

    public void switchToWeapon(WeaponType type, int slot,WeaponStats weapon)
    {

        // visual for the mesh
        EquipWeapon(type); 

        //visual for the crosshair
        ChangeCrosshair(type);

        // animator for animations 
        animator.SetInteger("WeaponType", slot); // shotgun

        //nastavi state weaponov da bojo in order da bos meu currentWeapon da bo tisti kjrji je
        currentWeapon = type;
        
        //Weapon activity + handle reloading and cancle reloading.
        activeWeaponStats.isActive = false;
        activeWeaponStats.TryReaload();
        activeWeaponStats = weapon;
        weapon.isActive = true;
        weapon.CancelReload();
    }
}