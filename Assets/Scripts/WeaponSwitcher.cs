using System;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
   
    public enum WeaponType { Melee, Rifle, Shotgun}


    [Header("References")]
    public Animator animator;

    public GameObject meleeWeapon;
    public GameObject rifleWeapon;
    public GameObject shotgunWeapon;

    [Header("CrosshairReferences")]
    public GameObject meleWeaponCrosshair;
    public GameObject rifleWeaponCrosshair;
    public GameObject shotgunWeaponCrosshair;

    //[Header("firePoint References for every weapon")]
    public Transform meeleWeaponfirePoint;
    public Transform rifleWeaponfirePoint;
    public Transform shotgunWeaponfirePoint;



    [Header("Current Weapon")]
    public WeaponType currentWeapon = WeaponType.Melee;

    void Start()
    {
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        // Switch weapons with number keys (for testing)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switchToWeapon(WeaponType.Melee, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            switchToWeapon(WeaponType.Rifle, 2);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            switchToWeapon(WeaponType.Shotgun, 3);
        }
    }

    public void EquipWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
        // Toggle visibility
        meleeWeapon.SetActive(newWeapon == WeaponType.Melee);
        rifleWeapon.SetActive(newWeapon == WeaponType.Rifle);
        shotgunWeapon.SetActive(newWeapon == WeaponType.Shotgun);

    }

  

    public void ChangeCrosshair(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;

        // Toggle visibility
        meleWeaponCrosshair.SetActive(newWeapon == WeaponType.Melee);
        rifleWeaponCrosshair.SetActive(newWeapon == WeaponType.Rifle);
        shotgunWeaponCrosshair.SetActive(newWeapon == WeaponType.Shotgun);

    }

    public void switchToWeapon(WeaponType type, int slot)
    {
        EquipWeapon(type);
        ChangeCrosshair(type);
        animator.SetInteger("WeaponType", slot); // shotgun
        currentWeapon = type;
    }


}