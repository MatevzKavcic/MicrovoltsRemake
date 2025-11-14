using UnityEngine;
using UnityEngine.UIElements;

public class RifleWeapon : WeaponStats

{
    public LineRenderer tracerLine;
    [Header("Hitscan Settings")]
    public Transform firePoint;      // from where it will shoot
    public LayerMask hitMask;        // What layers you can hit

    protected override void Shoot()
    {
        if (aimDirection == null) return;

        Vector3 origin = firePoint.position;
        Vector3 dir = aimDirection.GetAimDirection();

        if (Physics.Raycast(origin, dir, out RaycastHit hit, 1000f))
        {
            Debug.Log($"Hit {hit.collider.name} for {damage} damage");
            Debug.Log("i shot something out of my weapon... kinda ");

            // Optional: apply damage if target has health
            // hit.collider.GetComponent<Health>()?.TakeDamage(damage);

            if (tracerLine)
            {
                StartCoroutine(ShowTracer(hit.point));
            }
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