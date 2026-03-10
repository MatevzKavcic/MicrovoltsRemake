using Unity.Netcode;
using UnityEngine;

public class Buttonsss : MonoBehaviour


{

    


    public GameObject hideMe;

    public GameObject[] showMe;
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        hideMe.SetActive(false);

        for (int i = 0; i < showMe.Length; i++)
        {
            showMe[i].SetActive(true);
        }
    }

}
