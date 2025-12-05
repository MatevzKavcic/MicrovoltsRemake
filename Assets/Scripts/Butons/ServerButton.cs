using Unity.Netcode;
using UnityEngine;

public class ServerButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
