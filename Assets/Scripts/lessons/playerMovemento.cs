using UnityEngine;

public class playerMovemento : MonoBehaviour

{
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        Vector3 movement = new Vector3 (horizontal, 0.0f , vertical);

         movement.Normalize();

        transform.position = movement * Time.deltaTime * speed;


        if(movement != Vector3.zero)
        {
            transform.forward = movement;
        }

        Quaternion toRotate = Quaternion.LookRotation(movement, Vector3.up);

        Quaternion.RotateTowards ( transform.rotation, toRotate, speed*Time.deltaTime);


       // transform.Translate (movement * Time.deltaTime);  // time delta time pomeni da je enak time cez use kompe ne pa odvisno od komp speeda.
    }
}
