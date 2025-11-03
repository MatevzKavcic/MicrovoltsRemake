using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component from the same GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if left mouse button was pressed
        if (Input.GetMouseButtonDown(0))
        {
            animator.ResetTrigger("attackKey"); // optional — helps prevent stuck triggers
            animator.SetTrigger("attackKey");
            Debug.Log("Attack triggered!");
        }
    }
}