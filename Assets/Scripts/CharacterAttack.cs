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
    }    //void LateUpdate()
    //{
    //    //Debug.Log(animator.GetCurrentAnimatorStateInfo(1).shortNameHash);   
    //}



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
           // currentWeaponStats.TryShoot(); // delegate the actual attack
            //animator.SetTrigger("leftClick"); // optional, if you have weapon attack animation
            //Debug.Log(" triger Left click activated");

            if (currentWeaponStats.TryShoot()) // delegate the attack !!
            {
                animator.SetTrigger("Shoot");
            }
            animator.SetBool("isFiring", Input.GetMouseButton(0));
        }

        if (Input.GetMouseButton(0) == false )
        {

            animator.SetBool("isFiring", Input.GetMouseButton(0));
        }


        // Right click -> secondary fire
        if (Input.GetMouseButtonDown(1)&& currentWeaponStats.isReloading==false)
        {
            // If melee, heavy attack or alt-fire
            animator.SetTrigger("rightClick");

            
            currentWeaponStats.TryAim(); // delegate the actual attack or aim 

            

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
            case WeaponSwitcher.WeaponType.Sniper:
                return weaponSwitcher.sniperWeapon;
            case WeaponSwitcher.WeaponType.Zooka:
                return weaponSwitcher.zookaWeapon;
            case WeaponSwitcher.WeaponType.Granader:
                return weaponSwitcher.granaderWeapon;
            default:
                return null;
        }
    }
}