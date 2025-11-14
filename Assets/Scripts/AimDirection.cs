using UnityEngine;

public class AimDirection : MonoBehaviour
{
    public Transform hand;         // the hand or weapon muzzle
    public Camera playerCamera;    // your main camera
    public LayerMask aimLayerMask = ~0;
    public float maxDistance = 1000f;

    [HideInInspector] public Vector3 targetPoint; // Save the hit point for others

    /// <summary>
    /// Returns a world-space direction from the hand to the crosshair hit point.
    /// </summary>
    public Vector3 GetAimDirection()
    {
        // Ray from the center of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Raycast to detect what the crosshair hits
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If nothing hit, aim straight ahead to maxDistance
            targetPoint = ray.origin + ray.direction * maxDistance;
        }

        // Get direction from hand to that point
        Vector3 direction = (targetPoint - hand.position).normalized;
        return direction;
    }

    // Optional: visualize in Scene view
    private void OnDrawGizmos()
    {
        if (hand == null || playerCamera == null) return;
        // Ray from camera center
        Ray camRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 hitPoint;

        if (Physics.Raycast(camRay, out RaycastHit hit, maxDistance, aimLayerMask))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = camRay.origin + camRay.direction * maxDistance;
        }

        // Draw camera ray (green)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(camRay.origin, hitPoint);

        // Draw hand ray (red)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(hand.position, hitPoint);
    }
}