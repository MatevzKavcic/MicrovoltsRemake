using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SniperWeaponScript : WeaponStats
{

    [SerializeField] private float zoomFOV = 5f;

    [SerializeField] private GameObject scopeOverlayUI; // full screen scope
    [SerializeField] private GameObject normalCrosshair;

    [SerializeField] private float inZoomMouseSpeed;

    [SerializeField] private GameObject CameraMover;
    
    [SerializeField] private Transform DefaultPosition;

    [SerializeField] private Transform ZoomPosition;


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

        //CinemachinePOV.m_HorizontalAxis.m_MaxSpeed =3;

        //pri kameri ne premikat kamere ampak premakni samo položaj folow targeta

        CameraMover.transform.position = ZoomPosition.position;

        Debug.Log(cam.transform.position  + " položaj kamere  ENABLE");


        setCameraSpeedtoSlow();

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
        Debug.Log(cam.transform.position + " položaj kamere  DISABLE");
        CameraMover.transform.position = DefaultPosition.position;

        setCameraSpeedtoNormal();
        Debug.Log("zoom is " + virtualCamera.Lens.FieldOfView);

    }

    public void setCameraSpeedtoSlow()
    {
        foreach (var c in inputAxisController.Controllers)
        {


            if (c.Name == "Look X (Pan)")
            {

                c.Input.Gain = inZoomMouseSpeed;
            }

            if (c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = -inZoomMouseSpeed;

            }

        }
    }
    public void setCameraSpeedtoNormal()
    {
        foreach (var c in inputAxisController.Controllers)
        {


            if (c.Name == "Look X (Pan)")
            {

                c.Input.Gain = 1; //constant... to je konstanta za normal playtrough
            }

            if (c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = -1; //constant... to je konstanta za normal playtrough

            }

        }
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