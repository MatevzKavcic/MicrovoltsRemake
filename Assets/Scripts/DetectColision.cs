using UnityEngine;


public class Collision : MonoBehaviour
{
    private int points = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        Debug.Log("enter collision" + other.gameObject.name);



        if (other.gameObject.CompareTag("Coin"))
        {
            points += 50;
            Debug.Log("Pobrau si kovancek brao ");

            other.gameObject.SetActive(false);

            Debug.Log("points collected : " + points);
        }

       
    }

    void OnCollisionExit(UnityEngine.Collision collision)
    {
        Debug.Log("exited colision with " + collision.gameObject.name);

    }
}
