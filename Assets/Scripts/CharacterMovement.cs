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

    [Header("Jump Settings")]
    public int maxJumps = 2;       // 1 = normal jump, 2 = double jump
    private int jumpCount = 0;

    private Rigidbody rb;
    private float turnSmoothVelocity;
    private bool isGrounded;

    [SerializeField, Range(0f, 1f)]
    private float airControlPercent = 0f; // 0 = no control, 1 = full control

    private Vector3 lastMoveDir;

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

        // Calculate movement direction relative to camera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

        if (isGrounded)
        {
            // Full movement control on ground
            lastMoveDir = moveDir;
            Vector3 targetVelocity = moveDir * moveSpeed;
            targetVelocity.y = rb.velocity.y;
            rb.velocity = targetVelocity;
        }
        else
        {
            // In the air — reduced or zero control
            Vector3 airDir = Vector3.Lerp(lastMoveDir, moveDir, airControlPercent);
            Vector3 targetVelocity = airDir * moveSpeed;
            targetVelocity.y = rb.velocity.y; // keep gravity/jump
            rb.velocity = targetVelocity;
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

 
   void HandleJump()
{
    if (Input.GetButtonDown("Jump") && jumpCount <= maxJumps)
    {
        // Reset vertical speed before applying new force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpCount++;

        // If we’re on second jump, give some midair control
       
    }
}

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.05f, groundMask);

        // If we just landed, reset jump count and air control
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            airControlPercent = 0f;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }

}