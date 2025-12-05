using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponStats

{
    [Header("Melee Settings")]
    public Collider hitbox;            // assign your Hitbox collider here
    public float activeHitTime = 0.2f; // how long hitbox is active per swing
    public string targetTag = "Player"; // or "Enemy", depending who you hit

    private HashSet<Collider> alreadyHit = new HashSet<Collider>();

    protected override void Shoot()
    {
        // swing quick
        StartCoroutine(MeleeSwingQuickRoutine());
    }

    protected override void Aim()
    {
        // swing slow
        Debug.Log("i came here ?");
        StartCoroutine(MeleeSwingSlowRoutine());

    }

    private IEnumerator MeleeSwingQuickRoutine()
    {
        if (hitbox == null)
        {
            Debug.LogWarning($"{name}: No hitbox assigned for melee.");
            yield break;
        }

        damage = 350; // kolko dmg naredis ko swingnes quick

        // clear previously hit targets for this swing
        alreadyHit.Clear();

        // enable hitbox
        hitbox.enabled = true;

        // wait while swing is active
        yield return new WaitForSeconds(activeHitTime);

        // disable hitbox
        hitbox.enabled = false;
    }

    private IEnumerator MeleeSwingSlowRoutine()
    {
        if (hitbox == null)
        {
            Debug.LogWarning($"{name}: No hitbox assigned for melee.");
            yield break;
        }

        damage = 1200;  // kolko dmg naredis ko swingnes hard


        // clear previously hit targets for this swing
        alreadyHit.Clear();

        // enable hitbox
        hitbox.enabled = true;

        // wait while swing is active
        yield return new WaitForSeconds(activeHitTime*2); // difrence je samo u active time slotu weapona in u dmg ku ga naredis

        // disable hitbox
        hitbox.enabled = false;
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("im insiiideeee   F YEA");
    //    HandleHit(other);
    //}

    public void HandleHit(Collider other)
    {
        Debug.Log("im insiiideeee0");

        if (!hitbox.enabled) return;
        if (alreadyHit.Contains(other)) return;  // don’t hit same target twice this swing

        alreadyHit.Add(other);

        var stats = other.GetComponent<PlayerStats>(); // or EnemyStats

        Debug.Log("stats od playerja so " + stats.maxHealth);

        if (stats != null)
        {
            stats.TakeDamage(damage);
            Debug.Log($"[Melee] Hit {other.name} for {damage} damage");
        }
    }
}