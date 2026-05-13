using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BodyRotation : NetworkBehaviour
{
    [SerializeField] private Transform spineBone;

    [SerializeField] private float maxUpAngle = 40f;
    [SerializeField] private float maxDownAngle = -30f;
    [SerializeField] private float spineRotationSpeed = 10f;

    private Quaternion spineInitialRotation;

    private Camera playerCamera;

    private float currentSpineAngle;

    private NetworkVariable<float> networkSpineAngle =
        new NetworkVariable<float>(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartCoroutine(WaitForCamera());
    }


    private IEnumerator WaitForCamera()
    {
        while (playerCamera == null)
        {
            playerCamera = Camera.main;

            yield return null;
        }

        Debug.Log("Camera acquired for client");
    }


    void Start()
    {
        spineInitialRotation = spineBone.localRotation;
    }

    void LateUpdate()
    {

        Debug.Log($"IsOwner={IsOwner} | CameraNull={playerCamera == null}");
        if (IsOwner && playerCamera != null)
        {

            Debug.Log($"IsOwner: {IsOwner} | Local: {NetworkManager.Singleton.LocalClientId} | Owner: {OwnerClientId}");

            float cameraPitch = playerCamera.transform.eulerAngles.x;

            if (cameraPitch > 180f)
                cameraPitch -= 360f;

            float targetAngle =
                Mathf.Clamp(cameraPitch, maxDownAngle, maxUpAngle);

            networkSpineAngle.Value = targetAngle;
        }

        currentSpineAngle = Mathf.Lerp(
            currentSpineAngle,
            networkSpineAngle.Value,
            spineRotationSpeed * Time.deltaTime
        );

        spineBone.localRotation =
            spineInitialRotation *
            Quaternion.Euler(currentSpineAngle, 0f, 0f);
    }
}