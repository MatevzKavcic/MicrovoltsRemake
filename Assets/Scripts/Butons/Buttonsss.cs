using Unity.Netcode;
using UnityEngine;

public class Buttonsss : MonoBehaviour

    
{

    public GameObject hideMe;
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        hideMe.SetActive(false);
    }

}
