using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = -20f;

    [Header("Jump Settings")]
    public int maxJumps = 2;       // 1 = normal jump, 2 = double jump
    private int jumpCount = 0;
    private bool isGrounded;

    [SerializeField, Range(0f, 1f)]
    private float airControlPercent = 0f; // 0 = no control, 1 = full control

    private CharacterController controller;

    private Vector3 velocity;


    private Vector3 lastMoveDir;

    public float respawnDelay = 4f;

    private Animator animator;

    Vector3 knockbackVelocity;

    float knockbackDecay = 5f;
    public bool CanMove { get; private set; } = true;
    public bool IsRespawning { get; private set; } = false;

    void Start()
    {
        Debug.Log($"{name} | Owner = {IsOwner} | Server = {IsServer} | LocalID = {NetworkManager.Singleton.LocalClientId} | OwnerID = {OwnerClientId}");
        controller = GetComponent<CharacterController>();
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!IsOwner || !CanMove) return;
        HandleMovement();
        HandleJump();
        ApplyGravity();



        Vector3 finalVelocity = velocity + knockbackVelocity;

        controller.Move(finalVelocity * Time.deltaTime);// samo tle premikej mozicka


        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
    }


    public void BeginRespawn(Vector3 spawnPos) // to poklices u player stats da zacnes respawn i gues ? ;
    {
        CanMove = false; // se nemore vec premikat zs to skripto dokler mu ne das to na true
        IsRespawning = true;

        velocity = Vector3.zero;
        controller.enabled = false;

        transform.position = spawnPos;
        controller.enabled = true;
    }
    public IEnumerator FinishRespawn() // nevem zakaj rabi bit enumerator ce ja ampak pustimo ker dela za zdej
    {
        yield return new WaitForSeconds(respawnDelay);

        IsRespawning = false;
        CanMove = true; //se lahko premika nazaj lepo
    }

    void HandleMovement()
    {

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCount = 0;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float animX = Mathf.Approximately(moveX, 0f) ? 0f : Mathf.Sign(moveX);
        float animZ = Mathf.Approximately(moveZ, 0f) ? 0f : Mathf.Sign(moveZ);

        animator.SetFloat("MoveX", animX);
        animator.SetFloat("MoveZ", animZ);
        animator.SetBool("isGrounded", isGrounded);

        // Movement direction relative to the player's facing direction

        Transform cam = Camera.main.transform;

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;    
        Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

        Vector3 moveDir = (camForward * moveZ + camRight * moveX).normalized;

        if (isGrounded)
        {
            lastMoveDir = moveDir;
        }
        else
        {
            moveDir = Vector3.Lerp(lastMoveDir, moveDir, airControlPercent);
        }

        Vector3 horizontalVelocity = moveDir * moveSpeed;

        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    public void ApplyKnockback(Vector3 force)
    {
        if(isGrounded)
        {
            knockbackVelocity += force;

        }
    }


    void HandleJump()
{
    if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
    {
            velocity.y = jumpForce;
            jumpCount++;

            animator.ResetTrigger("jumpKey");
            animator.SetTrigger("jumpKey");
            //Debug.Log("jumpKey triggered");

        }
    }

    // ================= GRAVITY =================
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    //}

   

}