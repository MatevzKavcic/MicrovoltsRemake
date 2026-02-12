using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SniperWeaponScript : WeaponStats
{

    [SerializeField] private float zoomFOV = 5f;

    [SerializeField] private GameObject scopeOverlayUI; // full screen scope
    [SerializeField] private GameObject normalCrosshair;

    protected override void Aim() // pow poveci malo

    {
        if (!IsOwner) return;
        Debug.Log(" is zoomed variable :" + isZoomed);

        if (!isZoomed && !isReloading) // zoomej ce nisi zoomed
        {
            EnableZoom();
        }
        else
        {   
            DisableZoom();
        }
    }

    

    public void EnableZoom()
    {
        isZoomed = true;

        virtualCamera.Lens.FieldOfView = zoomFOV;

        Debug.Log("zoom is " + virtualCamera.Lens.FieldOfView);

        scopeOverlayUI.SetActive(true);
       //    virtualCamera.Lens.FieldOfView = Mathf.Lerp(
       //    virtualCamera.Lens.FieldOfView,
       //    zoomFOV,
       //    Time.deltaTime * zoomSpeed
       //);
    }


    public void DisableZoom()
    {
        if (!IsOwner) return;
        virtualCamera.Lens.FieldOfView = defaultFOV;
        scopeOverlayUI.SetActive(false);
        isZoomed = false;
        Debug.Log("zoom is " + virtualCamera.Lens.FieldOfView);

    }


    protected override void ServerShootLogic(Vector3 baseDir) // base dir je Aim direction ze zracunau in vrze noter v to metodo.
    {

        Vector3 origin = firePoint.position;

        Vector3 endPoint = origin + baseDir * maxDistance;

        // server raycast (authoritative)
        if (Physics.Raycast(origin, baseDir, out RaycastHit hit, maxDistance, layerMask))
        {
            endPoint = hit.point;

            // damage player if they have PlayerStats
            var stats = hit.collider.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
            Debug.Log($"Pellet hit {hit.collider.name} for {damage} damage");
        }

        // tell ALL clients to show tracer
        SpawnTracerClientRpc(origin, endPoint);


        DisableZoom();
    }


    public override void OnUnequip()
    {
        if (isZoomed)
            DisableZoom();
    }

}