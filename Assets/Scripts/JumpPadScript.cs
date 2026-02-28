using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 launchDirection = new Vector3(0, 1, 1);
    public float launchSpeed = 20f;


    private void OnTriggerEnter(Collider other)
    {
        CharacterMovement movement = other.GetComponent<CharacterMovement>();

        if (movement != null)
        {
            //Vector3 launchDir = transform.forward + Vector3.up;
            launchDirection.Normalize();

            movement.ApplyLaunch(launchDirection* launchSpeed);

        }
    }
}
