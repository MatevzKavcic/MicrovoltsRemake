using UnityEngine;
using UnityEngine.UIElements;

public class RifleWeapon : WeaponStats

{
    public LineRenderer tracerLine;
    [Header("Hitscan Settings")]
    public Transform firePoint;      // from where it will shoot
    public LayerMask hitMask;        // What layers you can hit
    public float maxDistance=1000f;

    protected override void Shoot()
    {
        if (aimDirection == null) return;

        Vector3 origin = firePoint.position;
        Vector3 dir = aimDirection.GetAimDirection();

        Vector3 endPoint = origin + dir * maxDistance;


        if (Physics.Raycast(origin, dir, out RaycastHit hit, 1000f))
        {

            endPoint = hit.point;


            Debug.Log($"Hit {hit.collider.name} for {damage} damage");
            Debug.Log("i shot something out of my weapon... kinda ");

            if (hit.collider.GetComponent<PlayerStats>() != null) // poglej da si zadeu playera. Èe nisi pol ðabe.
            { 
                hit.collider.GetComponent<PlayerStats>().TakeDamage(100f); // naredi logiko za odvisno kaj zadanes... glava noge roke...

            }
        }
        else
        {
            //do nothing
            Debug.Log("Shot into nothing");
        }

        if (tracerLine)
        {
            StartCoroutine(ShowTracer(endPoint));
        }
        
    }

    private System.Collections.IEnumerator ShowTracer(Vector3 hitPoint)
    {
        tracerLine.SetPosition(0, firePoint.position);
        tracerLine.SetPosition(1, hitPoint);
        tracerLine.enabled = true;
        yield return new WaitForSeconds(0.05f);
        tracerLine.enabled = false;
    }
}