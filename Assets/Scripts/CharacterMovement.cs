using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float jumpForce = 7f;
    public float idleRotationDeadZone = 20f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Jump Feel")]
    public float fallMultiplier = 2f; // slightly faster fall

    private Rigidbody rb;
    private float turnSmoothVelocity;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        if (moveDir.magnitude >= 0.1f)
        {
            // Rotate moveDir relative to camera Y rotation
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDirRotated = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            Vector3 targetVelocity = moveDirRotated * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y; // preserve vertical velocity
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // Stop horizontal movement if no input
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void HandleRotation()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 movementDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            // Rotate toward movement direction relative to camera
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            // Idle: rotate toward camera forward if dead zone exceeded
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                float targetAngle = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
                float angleDiff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

                if (Mathf.Abs(angleDiff) > idleRotationDeadZone)
                {
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }
    }

    //void HandleRotation()
    //{
    //    float moveX = Input.GetAxis("Horizontal");
    //    float moveZ = Input.GetAxis("Vertical");
    //    Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

    //    if (direction.magnitude >= 0.1f)
    //    {
    //        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
    //        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
    //        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    //    }
    //}
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // clear old vertical motion
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);      // one consistent impulse
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
            groundCheckDistance + 0.05f, groundMask);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }

}