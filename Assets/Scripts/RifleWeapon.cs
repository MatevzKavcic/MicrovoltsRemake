using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class RifleWeapon : WeaponStats

{
    public LineRenderer tracerLinePrefab;
    [Header("Hitscan Settings")]
    //public Transform firePoint;      // from where it will shoot
    public LayerMask hitMask;        // What layers you can hit
    //public float maxDistance=1000f;

    protected override void Shoot()
    {
        if (firePoint == null) return;
        Vector3 targetPoint;
        Vector3 dir = GetAimDirection(out targetPoint);

        Vector3 origin = firePoint.position;
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

        if (tracerLinePrefab != null)
        {
            LineRenderer lr = Instantiate(tracerLinePrefab, Vector3.zero, Quaternion.identity);
            lr.useWorldSpace = true;
            StartCoroutine(ShowTracer(lr, origin, endPoint));
        }

    }

    

    private IEnumerator ShowTracer(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.enabled = true;

        yield return new WaitForSeconds(0.05f);

        Destroy(line.gameObject);
    }



}