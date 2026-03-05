using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class CameraFolowingBinder : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;

    [SerializeField] private CinemachineCamera vcam;

    public Camera PlayerCamera { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            vcam.gameObject.SetActive(false);
            return;
        }

        vcam.gameObject.SetActive(true);
        vcam.Follow = cameraFollowTarget;

        if (vcam == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found!");
            return;
        }

    }


}
