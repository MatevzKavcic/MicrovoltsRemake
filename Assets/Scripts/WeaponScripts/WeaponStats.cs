using System;
using System.Collections;
using Unity.Cinemachine;
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

    public float defaultFOV= 50;
    public bool isZoomed = false;


    [SerializeField] public float zoomSpeed = 10f;


    protected Camera cam;

    [SerializeField] protected CinemachineCamera virtualCamera;

    [SerializeField] protected CinemachineCamera virtualCameraFPS;

    protected CinemachinePanTilt fpsPanTilt;
    protected CinemachinePanTilt thirdPanTilt;

    public LineRenderer tracerLinePrefab;

    [Header("ammo")]
    public int ammo;
    public float reloadTime;
    public int ammoSize;
    public int totalAmo;
    public bool isReloading = false;

    private Coroutine reloadCoroutine;

    [Header("Hitscan Settings")]
    public LayerMask layerMask; //To masko mora nastimat usak weapon posebej ampak basicly je to samo kam aimas... in kaj zadanes in kaj ne.


    public CinemachineInputAxisController inputAxisController;

    public CinemachineInputAxisController inputAxisControllerFPS;

    //protected virtual IEnumerator AssignCameraWhenReady()
    //{
    //    // Wait until Camera.main exists
    //    while (cam == null)
    //    {
    //        cam = Camera.main;

    //        yield return null; // wait a frame
    //    }


    //}// samo za camera da se bo pravilno inniciarizirala

    void Awake()
    {
       
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartCoroutine(AssignCamera());
    }

    private IEnumerator AssignCamera()
    {
        while (Camera.main == null)
            yield return null;

        cam = Camera.main;

        Debug.Log("Camera assigned: " + cam);

        inputAxisController = virtualCamera
            .GetComponentInChildren<CinemachineInputAxisController>();


        fpsPanTilt = virtualCamera.GetComponent<CinemachinePanTilt>();
        thirdPanTilt = virtualCameraFPS.GetComponent<CinemachinePanTilt>();



    }


    //protected virtual void Start()
    //{
    //    if (!IsOwner) return;
    //    if (cam == null) return;
    //    if (virtualCamera == null) return;


    //} // samo za camera da se bo pravilno inniciarizirala

    public virtual bool TryShoot() // to je dejanski gate keep ki se vedno pogleda preden streljas tko da lahko tle delas checke za network ne rabis u weaponih.
    {
        if (!IsOwner) return false;  // network blocka da si to res samo ti ce nisi automatko ne nardi
        if (isReloading) return false ;          // no shooting while reloading
        if (ammo == -10) // melee weapon da bo udaru
        {
            Shoot();
            return true;
        }
        if (ammo <= 0) return false;
        if (Time.time < nextFireTime) return false;

        nextFireTime = Time.time + fireRate;
        ammo--;
        Shoot();
        return true;


    }
    protected virtual void Shoot()
    {
        if (!IsOwner) return; // << only the owner uses the camera
        Vector3 targetPoint;
        Vector3 baseDir = GetAimDirection(out targetPoint);

        OnBeforeShoot(); // to rabi samo sniper.... ampak rabi 

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

    public virtual void OnUnequip()
    {
        // default: do nothing
    }

    protected virtual void OnBeforeShoot()
    {
        // default = do nothing
    }



    protected abstract void Aim(); // usi clasi ga morajo met ampak aimali bojo rifle in snipe... mele weapon hita

    public virtual void TryAim()
    {
        if (!IsOwner) return;  // <–– only local player shoots
        if (cam == null) return;
        if (isReloading) return ;
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

        if (totalAmo == 0)
        {
            Debug.Log("OutOfAmmo");
            return;
        }


        reloadCoroutine = StartCoroutine(ReloadRoutine());

    }


    private IEnumerator ReloadRoutine()
    {
        if (totalAmo <= 0) {  // nimas vec nic u rezervi in u chamberju
            Debug.Log("no more amo nowhere");

            yield return 0;
        }
        isReloading = true;
        // wait for reload time
        Debug.Log("Reloading a weapon");

        bool fullReload = (totalAmo + (ammoSize - ammo)) > ammoSize; // ko je to true je mozen full reload in ga naredi

        Debug.Log("full reload posible ? --> " + fullReload);

        yield return new WaitForSeconds(reloadTime);
        Debug.Log("reloaded a weapon");
       

        if (fullReload)
        {
            totalAmo = totalAmo - (ammoSize - ammo);    // total ammo is a substraction of the amoMagSize - ammoLeftIn the chamber
            ammo = ammoSize; // napolni mag in total ammo zmanjsej za razliko.
            Debug.Log("full full reloading");

            Debug.Log(totalAmo +" - " +ammoSize +" - " +ammo );


        }
        else // full reload ni mogoc
        {
            ammo = ammo + totalAmo; // uzemi kar mas u ammotu in dodaj se total ammo.
            totalAmo = 0;
            Debug.Log("half reloading");

        }



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

        yield return new WaitForSeconds(1f);

        Destroy(line.gameObject);
    } // kar dejansko dela line u igri in jih unicuje.

    [ClientRpc]
    protected void SpawnTracerClientRpc(Vector3 start, Vector3 end) // narisi crte in bodi fency
    {
        if (tracerLinePrefab == null) return;
        if (cam == null) return;

        
        LineRenderer lr = Instantiate(tracerLinePrefab, Vector3.zero, Quaternion.identity);
        StartCoroutine(ShowTracer(lr, start, end));
    }

    protected Vector3 GetAimDirection(out Vector3 targetPoint)
    {

        targetPoint = Vector3.zero;

        if (cam == null||!IsOwner )
        {
            Debug.LogWarning($"{name}: Missing camera" + cam + "   na zacetku je setana ? "+ IsOwner);
            return Vector3.forward;
        }
        if ( firePoint == null)
        {
            Debug.LogWarning($"{name}: missing fire point" + cam);

        }

        // Ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
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