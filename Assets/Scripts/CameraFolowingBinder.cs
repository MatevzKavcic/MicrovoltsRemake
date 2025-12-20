using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraBinder : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        CinemachineCamera vcam =FindObjectOfType<CinemachineCamera>();

        if (vcam == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found!");
            return;
        }

        vcam.Follow = cameraFollowTarget;
    }
}