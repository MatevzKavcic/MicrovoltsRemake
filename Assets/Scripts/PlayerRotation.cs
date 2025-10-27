using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerRotation : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotationSmoothTime = 0.1f;
    public float idleDeadZone = 20f;

    private PlayerInputHandler input;
    private float turnSmoothVelocity;

    void Start()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        HandleRotation();
    }

    void HandleRotation()
    {
        Vector2 move = input.MoveInput;
        Vector3 moveDir = new Vector3(move.x, 0f, move.y).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                float targetAngle = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
                float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

                if (Mathf.Abs(diff) > idleDeadZone)
                {
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }
    }
}
