using System.Collections;
using UnityEngine;

public class ShotgunWapon : WeaponStats

{
    public LineRenderer tracerLinePrefab; // zakomentirano ker bo tracer prefabDelau
    [Header("Hitscan Settings")]
    public Transform firePoint;      // from where it will shoot
    public LayerMask hitMask;        // What layers you can hit
    public float maxDistance = 1000f;

    [Header("Shotgun Settings")]
    public int pelletCount = 8;
    public float spreadAngle = 5f;      // degrees of random spread
    public float tracerDuration = 0.5f;


    protected override void Shoot()
    {
        if (aimDirection == null || firePoint == null) return;

        Vector3 origin = firePoint.position;
        Vector3 baseDir = aimDirection.GetAimDirection();




        for (int i = 0; i <= pelletCount; i++)
        {
            Vector3 dir = GetSpreadDirection(baseDir);

            Vector3 endPoint = origin + dir * maxDistance;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, hitMask))
            {
                endPoint = hit.point;


                // Optional: apply damage
                if (hit.collider.GetComponent<PlayerStats>() != null)
                {
                    hit.collider.GetComponent<PlayerStats>().TakeDamage(100f); // naredi logiko za odvisno kaj zadanes... glava noge roke...
                }

                Debug.Log($"Pellet {i} hit {hit.collider.name} for {damage} damage");
            }

            else
            {
                //do nothing
                Debug.Log("Shot into nothing");
            }

            if (tracerLinePrefab != null)
            {
                LineRenderer lr = Instantiate(tracerLinePrefab, Vector3.zero, Quaternion.identity);
                StartCoroutine(ShowTracer(lr, origin, endPoint));
            }

        }

    }

    Vector3 GetSpreadDirection(Vector3 baseDir)
    {
        // Make a random rotation around base direction
        // random yaw & pitch within spreadAngle
        float yaw = Random.Range(-spreadAngle, spreadAngle);
        float pitch = Random.Range(-spreadAngle, spreadAngle);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 spreadDir = rot * baseDir;
        return spreadDir.normalized;
    }

    private IEnumerator ShowTracer(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.enabled = true;

        yield return new WaitForSeconds(tracerDuration);

        Destroy(line.gameObject);
    }
}