using Unity.Netcode;
using UnityEngine;

public class Buttonsss : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

}
