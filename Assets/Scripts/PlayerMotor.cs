using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMotor : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private PlayerInputHandler input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputHandler>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Vector2 move = input.MoveInput;
        Vector3 moveDir = transform.forward * move.y + transform.right * move.x;
        Vector3 targetVel = moveDir.normalized * moveSpeed;
        targetVel.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVel;
    }
}