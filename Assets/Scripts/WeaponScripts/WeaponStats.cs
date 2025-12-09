using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class WeaponStats : NetworkBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Default Weapon";
    public float damage = 10f;
    public float fireRate = 0.25f;
    public bool isActive = false;   // if you have the weapon selected so it knows if you dont it reloads it. if not active TryReload  ?  . if you switch to it start reloading from scratch.
    [Header("Firing")]  
    public Transform firePoint;
    protected float nextFireTime;
    public float maxDistance = 1000f;
    protected Camera cam;

    public LineRenderer tracerLinePrefab;

    [Header("ammo")]
    public int ammo;
    public float reloadTime;
    public int ammoSize;
    public bool isReloading = false;

    private Coroutine reloadCoroutine;


    //protected virtual void Awake()
    //{
    //    Debug.Log("BEFORE isOwner");

    //    if (!IsOwner) return;

    //    cam = Camera.main;
    //    Debug.Log("CAMERA IS SET WHY TF IS NOT WORKING");

    //    if (cam == null)
    //        Debug.LogWarning($"{name}: No main camera found!");
    //}

    protected virtual IEnumerator AssignCameraWhenReady()
    {
        // Wait until Camera.main exists
        while (cam == null)
        {
            cam = Camera.main;
            yield return null; // wait a frame
        }
    }// samo za camera da se bo pravilno inniciarizirala

    protected virtual void Start()
    {
        if (!IsOwner) return;
        StartCoroutine(AssignCameraWhenReady());
    } // samo za camera da se bo pravilno inniciarizirala
    public virtual void TryShoot() // to je dejanski gate keep ki se vedno pogleda preden streljas tko da lahko tle delas checke za network ne rabis u weaponih.
    {
        if (!IsOwner) return;  // network blocka da si to res samo ti ce nisi automatko ne nardi
        if (isReloading) return;          // no shooting while reloading
        if (ammo == -10) // melee weapon da bo udaru
        {
            Shoot();
        }
        if (ammo <= 0) return;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            ammo--;
        }

        
    }
    protected virtual void Shoot()
    {
        if (!IsOwner) return; // << only the owner uses the camera
        Vector3 targetPoint;
        Vector3 baseDir = GetAimDirection(out targetPoint);

        // Ask the server to perform the shot
        PerformShotServerRpc(baseDir);
    }
    //ne streljas vec z shoot ampak z server shoot logic da pokazes usem in bo tejkau damage od serverja
    protected abstract void ServerShootLogic(Vector3 baseDir);

    [ServerRpc]
    protected void PerformShotServerRpc(Vector3 baseDir)
    {
        // v spodnji metodi mors definirat logiko kako napadas v Rifle, shotgun ...
        ServerShootLogic(baseDir);
    }



    protected abstract void Aim(); // usi clasi ga morajo met ampak aimali bojo rifle in snipe... mele weapon hita

    public virtual void TryAim()
    {
        if (!IsOwner) return;  // <–– only local player shoots
        Aim();
    }

    public virtual void TryReaload()
    {
        if (!IsOwner) return; // network
        if (isReloading)
        {
            //Debug.Log("Already reloading a weapon");
            return;
        }

        if (ammo == ammoSize)
        {
            //Debug.Log("Doesnt need Reloading");
            return;
        }

        reloadCoroutine = StartCoroutine(ReloadRoutine());

    }


    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        // wait for reload time
        Debug.Log("Reloading a weapon");

        yield return new WaitForSeconds(reloadTime);
        Debug.Log("reloaded a weapon");
        ammo = ammoSize;
        isReloading = false;
        reloadCoroutine = null;
    }

    public void CancelReload()
    {
        if (!isReloading) return;

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        isReloading = false;
        Debug.Log( "reload cancelled and started back again reloding");

    }


    private IEnumerator ShowTracer(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.enabled = true;

        yield return new WaitForSeconds(0.05f);

        Destroy(line.gameObject);
    } // kar dejansko dela line u igri in jih unicuje.

    [ClientRpc]
    protected void SpawnTracerClientRpc(Vector3 start, Vector3 end) // narisi crte in bodi fency
    {
        if (tracerLinePrefab == null) return;

        LineRenderer lr = Instantiate(tracerLinePrefab, Vector3.zero, Quaternion.identity);
        StartCoroutine(ShowTracer(lr, start, end));
    }

    protected Vector3 GetAimDirection(out Vector3 targetPoint)
    {

        targetPoint = Vector3.zero;

        if (cam == null||!IsOwner )
        {
            Debug.LogWarning($"{name}: Missing camera" + cam + "   na zacetku je setana ? ");
            return Vector3.forward;
        }
        if ( firePoint == null)
        {
            Debug.LogWarning($"{name}: missing fire point" + cam);

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
    } // usi weaponi rabijo to da vejo kam streljajo


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