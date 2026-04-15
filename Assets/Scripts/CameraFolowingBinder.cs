using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class CameraFolowingBinder : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;

    [SerializeField] private Transform cameraFollowTargetFPS;


    [SerializeField] private CinemachineCamera vcam;

    [SerializeField] private CinemachineCamera FPSvCam;

    public Camera PlayerCamera { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            vcam.gameObject.SetActive(false);
            FPSvCam.gameObject.SetActive(false);
            return;
        }

        vcam.gameObject.SetActive(true);
        vcam.Follow = cameraFollowTarget;

        FPSvCam.gameObject.SetActive(true);
        FPSvCam.Follow = cameraFollowTargetFPS;

        if (vcam == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found!");
            return;
        }

        if (FPSvCam == null)
        {
            Debug.LogError("No Cinemachine FPS Virtual Camera found!");
            return;
        }

    }


}
