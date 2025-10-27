using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign your FreeLook camera here

    [Header("Settings")]
    public float turnSmoothTime = 0.1f; // smoothing factor
    private float turnSmoothVelocity;

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate target rotation based on camera's Y rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Smoothly rotate player
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Optional: move player forward
            // transform.Translate(direction * speed * Time.deltaTime, Space.Self);
        }
    }
}
