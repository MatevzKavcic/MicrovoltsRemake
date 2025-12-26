using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public float groundCheckDistance;
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

    public float respawnDelay = 4f;

    private Animator animator;

    public bool CanMove { get; private set; } = true;
    public bool IsRespawning { get; private set; } = false;

    void Start()
    {
        Debug.Log($"{name} | Owner = {IsOwner} | Server = {IsServer} | LocalID = {NetworkManager.Singleton.LocalClientId} | OwnerID = {OwnerClientId}");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleJump();
        //Debug.Log("Grounded: " + isGrounded);
    }

    void FixedUpdate()
    {
        if (!IsOwner || !CanMove) return; // poglej ce s elahko premika.... ce se nemore
        HandleMovement();
        CheckGround();
    }

    public void BeginRespawn(Vector3 spawnPos) // to poklices u player stats
    {
        CanMove = false; // se nemore vec premikat zs to skripto dokler mu ne das to na true
        IsRespawning = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        transform.position = spawnPos; // premakni ga 
    }
    public IEnumerator FinishRespawn() // nevem zakaj rabi bit enumerator ce ja ampak pustimo ker dela za zdej
    {
        yield return new WaitForSeconds(respawnDelay);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.WakeUp();

        IsRespawning = false;
        CanMove = true; //se lahko premika nazaj lepo
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

        Transform cam = Camera.main.transform;

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;    
        Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

        if (isGrounded)
        {
            lastMoveDir = moveDir;

            Vector3 targetVelocity = moveDir * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            Vector3 airDir = Vector3.Lerp(lastMoveDir, moveDir, airControlPercent);

            Vector3 targetVelocity = airDir * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }
    }


    void HandleJump()
{
    if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
    {
        // Reset vertical speed before applying new force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpCount++;

            animator.ResetTrigger("jumpKey");
            animator.SetTrigger("jumpKey");
            Debug.Log("jumpKey triggered");

        }
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        float radius = col.radius * 0.9f; // slight shrink to avoid edge issues

        Vector3 origin = transform.position + Vector3.up * (col.height / 2f - radius);

        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.05f, groundMask);

        // If we just landed, reset jump count and air control
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;

            animator.SetBool("isGrounded", isGrounded);
            //Debug.Log("Is grounded true");


        }
        else if(isGrounded==false){
            animator.SetBool("isGrounded", isGrounded);
            //Debug.Log("Is grounded false");
        }



    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }

   

}