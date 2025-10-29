using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Jump Feel")]
    public float fallMultiplier = 2f; // slightly faster fall

    [Header("Jump Settings")]
    public int maxJumps = 2;       // 1 = normal jump, 2 = double jump
    private int jumpCount = 0;

    private Rigidbody rb;
    private bool isGrounded;

    [SerializeField, Range(0f, 1f)]
    private float airControlPercent = 0f; // 0 = no control, 1 = full control

    private Vector3 lastMoveDir;


    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponentInChildren<Animator>();
        Debug.Log(animator + "is indeed here");

    }

    void Update()
    {
        HandleJump();
        Debug.Log("Grounded: " + isGrounded);
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

        float animX = Mathf.Approximately(moveX, 0f) ? 0f : Mathf.Sign(moveX);
        float animZ = Mathf.Approximately(moveZ, 0f) ? 0f : Mathf.Sign(moveZ);

        animator.SetFloat("MoveX", animX);
        animator.SetFloat("MoveZ", animZ);

        // Movement direction relative to the player's facing direction
        Vector3 moveDir = (transform.forward * moveZ + transform.right * moveX).normalized;

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
            // In the air � reduced or zero control
            Vector3 airDir = Vector3.Lerp(lastMoveDir, moveDir, airControlPercent);
            Vector3 targetVelocity = airDir * moveSpeed;
            targetVelocity.y = rb.velocity.y; // keep gravity/jump
            rb.velocity = targetVelocity;
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

        // If we�re on second jump, give some midair control
       
    }
}

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        float rayOriginOffset = GetComponent<CapsuleCollider>().height / 2 - 0.05f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayOriginOffset + groundCheckDistance, groundMask);

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