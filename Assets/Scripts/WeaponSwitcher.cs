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
            EquipWeapon(WeaponType.Melee);
            ChangeCrosshair(WeaponType.Melee);
            animator.SetInteger("WeaponType", 1); // Melee
            currentWeapon = WeaponType.Melee;

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(WeaponType.Rifle);
            ChangeCrosshair(WeaponType.Rifle);
            animator.SetInteger("WeaponType", 2); //rifle
            currentWeapon = WeaponType.Rifle;

        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(WeaponType.Shotgun);
            ChangeCrosshair(WeaponType.Shotgun);
            animator.SetInteger("WeaponType", 3); // shotgun
            currentWeapon = WeaponType.Shotgun;
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
}