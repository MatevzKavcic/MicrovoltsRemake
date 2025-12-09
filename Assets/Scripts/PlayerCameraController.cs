using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    private CinemachineCamera virtualCam; // KOMPONENTA KI GLEDA SAMO MULTIPLAYER CAMERO KER JE CINEMACHINE IN DRUGAVCE NOCES SPREMINJAT SKRIPTE

    void Awake()
    {
        virtualCam = GetComponentInChildren<CinemachineCamera>();
    }

    public override void OnNetworkSpawn()
    {
        // Only enable the camera if this is MY player
        if (!IsOwner)
        {
            virtualCam.enabled = false;
        }
        else
        {
            virtualCam.enabled = true;
        }
    }
}