using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Animator animator;

    public WeaponSwitcher weaponSwitcher;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (weaponSwitcher == null) return;

        // Get the currently active weapon
        WeaponStats currentWeaponStats = GetActiveWeaponStats();
        if (currentWeaponStats == null) return;

        if (currentWeaponStats.ammo == 0) // vedno ko mas 0 ammota probas reloadad sepravi ce canclam reload bo u naslednjm frejmu ze reloadou nazaj
        {
            currentWeaponStats.TryReaload();
        }

        // Left click -> primary fire
        if (Input.GetMouseButton(0)&& currentWeaponStats.isReloading!=true)
        {
           
            currentWeaponStats.TryShoot(); // delegate the actual attack
            animator.SetTrigger("leftClick"); // optional, if you have weapon attack animation
        }

        // Right click -> secondary fire
        if (Input.GetMouseButtonDown(1))
        {
            // If melee, heavy attack or alt-fire
            animator.SetTrigger("rightClick");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeaponStats.TryReaload();
        }
    }

    private WeaponStats GetActiveWeaponStats()
    {
        switch (weaponSwitcher.currentWeapon)
        {
            case WeaponSwitcher.WeaponType.Melee:
                return weaponSwitcher.meleeWeapon;
            case WeaponSwitcher.WeaponType.Rifle:
                return weaponSwitcher.rifleWeapon;
            case WeaponSwitcher.WeaponType.Shotgun:
                return weaponSwitcher.shotgunWeapon;
            default:
                return null;
        }
    }
}