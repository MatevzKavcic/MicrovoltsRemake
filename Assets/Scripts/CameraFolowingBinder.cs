using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using static UnityEditor.VersionControl.Message;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System;

public class PlayerCameraBinder : NetworkBehaviour
{
    [SerializeField] private Transform cameraFollowTarget;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();

        if (vcam == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found!");
            return;
        }
        vcam.Follow = cameraFollowTarget;
    }
}
