using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAimRotation : NetworkBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign your main camera here in the Inspector

    [Header("Rotation Settings")]
    [Tooltip("How quickly the player rotates to face the camera direction.")]
    public float rotationSpeed = 1000f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartCoroutine(AssignCamera());
    }

    private IEnumerator AssignCamera()
    {
        while (Camera.main == null)
            yield return null;

        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (!IsOwner) return; 
        RotateTowardCamera();
        //Debug.Log($"ROT: {transform.rotation.eulerAngles}");
    }

    private void RotateTowardCamera()
    {
        if (!cameraTransform) return;

        if (!IsOwner) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(forward),
            10f * Time.deltaTime
        );

        //Debug.Log("rotating " + transform.gameObject);
    }
}