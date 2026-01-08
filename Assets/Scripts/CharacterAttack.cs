using System.Collections;
using Unity.Netcode.Components;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Animator animator;

    public WeaponSwitcher weaponSwitcher;

    [SerializeField] private NetworkAnimator networkAnimator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void LateUpdate()
    {
        Debug.Log(animator.GetCurrentAnimatorStateInfo(1).shortNameHash);   
    }


    void Update()
    {
        if (weaponSwitcher == null) return;

        // Get the currently active weapon
        WeaponStats currentWeaponStats = GetActiveWeaponStats();
        if (currentWeaponStats == null) return;

        if (currentWeaponStats.ammo == 0 && currentWeaponStats.ammo != -10) // vedno ko mas 0 ammota probas reloadad sepravi ce canclam reload bo u naslednjm frejmu ze reloadou nazaj... ce imas -10 ammota si mele weapon
        {
            currentWeaponStats.TryReaload();
             // animator.SetBool("isReloading", true); // to moras se implementirat

        }

        // Left click -> primary fire
        if (Input.GetMouseButton(0)&& currentWeaponStats.isReloading!=true)
        {
            currentWeaponStats.TryShoot(); // delegate the actual attack
            //animator.SetTrigger("leftClick"); // optional, if you have weapon attack animation
            Debug.Log(" triger Left click activated");

            //networkAnimator.SetTrigger("leftClick");

            animator.SetBool("isShooting", true);

            ResetShoot(currentWeaponStats.shootLockTime); // bad practice je to ma ce deluje tle dobis samo kolko casa traja animation od weapona in pol lockej ta animation da shoota tolko casa...... preden skenslas animation
            

        }

        if (Input.GetMouseButton(0) == false )
        {
            animator.SetBool("isShooting", false);

        }

        // Right click -> secondary fire
        if (Input.GetMouseButtonDown(1))
        {
            // If melee, heavy attack or alt-fire
            animator.SetTrigger("rightClick");
            currentWeaponStats.TryAim(); // delegate the actual attack

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeaponStats.TryReaload();
        }
    }


    IEnumerator ResetShoot(float timer)
    {
        yield return new WaitForSeconds(timer);
        animator.SetBool("IsShooting", false);
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
            case WeaponSwitcher.WeaponType.Sniper:
                return weaponSwitcher.sniperWeapon;
            default:
                return null;
        }
    }
}