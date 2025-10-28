using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAimRotation : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign your main camera here in the Inspector

    [Header("Rotation Settings")]
    [Tooltip("How quickly the player rotates to face the camera direction.")]
    public float rotationSpeed = 8f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Ensure physics doesn't interfere with rotation

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        RotateTowardCamera();
    }

    private void RotateTowardCamera()
    {
        if (!cameraTransform) return;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        if (camForward.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(camForward);

        // Rotate at a fixed angular speed (degrees per second)
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}