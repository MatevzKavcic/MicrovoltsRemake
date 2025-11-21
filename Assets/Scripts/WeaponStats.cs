using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponStats : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Default Weapon";
    public float damage = 10f;
    public float fireRate = 0.25f;
    [Header("Firing")]  
    public Transform firePoint;
    protected float nextFireTime;
    public float maxDistance = 1000f;
    protected Camera cam;

    [Header("ammo")]
    public int ammo;
    public float reloadTime;
    public int ammoSize;
    public bool isReloading = false;


    protected virtual void Awake()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogWarning($"{name}: No main camera found!");
    }

    public virtual void TryShoot()
    {
        if (isReloading) return;          // no shooting while reloading
        if (ammo <= 0) return;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            ammo--;
        }
    }

    public virtual void TryReaload()
    {
        if (isReloading)
        {
            Debug.Log("Reloading a weapon");
            return;
        }



        if (ammo == ammoSize)
        {
            Debug.Log("to much ammo ");
            return;
        }

     

        StartCoroutine(ReloadRoutine());

    }


    protected Vector3 GetAimDirection(out Vector3 targetPoint)
    {
        targetPoint = Vector3.zero;

        if (cam == null || firePoint == null)
        {
            Debug.LogWarning($"{name}: Missing camera or firePoint!");
            return Vector3.forward;
        }

        // Ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * maxDistance;
        }

        return (targetPoint - firePoint.position).normalized;
    }


    protected abstract void Shoot();

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        // TODO: play reload animation / sound here

        // wait for reload time
        Debug.Log("Reloading a weapon");

        yield return new WaitForSeconds(reloadTime);
        Debug.Log("reloaded a weapon");
        ammo = ammoSize;
        isReloading = false;
    }


    private void OnDrawGizmos()
    {
        if (firePoint == null || cam == null) return;
        // Ray from camera center
        Ray camRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 hitPoint;

        if (Physics.Raycast(camRay, out RaycastHit hit, maxDistance, ~0))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = camRay.origin + camRay.direction * maxDistance;
        }

        // Draw camera ray (green)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(camRay.origin, hitPoint);

        // Draw hand ray (red)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, hitPoint);
    }
}