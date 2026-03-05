using Unity.Netcode;
using UnityEngine;

public class BodyRotation : NetworkBehaviour
{
    [SerializeField] private Transform spineBone;
    [SerializeField] private float maxUpAngle = 40f;
    [SerializeField] private float maxDownAngle = -30f;
    [SerializeField] private float spineRotationSpeed = 10f;

    private float currentSpineAngle;
    private Quaternion spineInitialRotation;


    private Camera playerCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        playerCamera = Camera.main;
    }

    void Start()
    {
        spineInitialRotation = spineBone.localRotation;
    }

    void LateUpdate()
    {
        if (!IsOwner) return;

        if (playerCamera == null) return;


        float cameraPitch = playerCamera.transform.eulerAngles.x;

        if (cameraPitch > 180f)
            cameraPitch -= 360f;

        float targetAngle = Mathf.Clamp(cameraPitch, maxDownAngle, maxUpAngle);

        currentSpineAngle = Mathf.Lerp(
            currentSpineAngle,
            targetAngle,
            spineRotationSpeed * Time.deltaTime
        );

        spineBone.localRotation =
            spineInitialRotation *
            Quaternion.Euler(currentSpineAngle, 0f, 0f);
    }
}