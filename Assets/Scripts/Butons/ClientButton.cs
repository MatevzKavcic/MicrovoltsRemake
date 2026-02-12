using Unity.Netcode;
using UnityEngine;

public class ClientButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hideMe;

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        hideMe.SetActive(false);

    }
}
