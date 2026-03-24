using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ClientButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hideMe;

    public TMP_InputField ipInputField;

    public GameObject[] showMe;

    
    public void StartClient()
    {
        string ip = ipInputField.text;

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        Debug.Log(transport + " to je transpport ce dela sploh ' to je  ip : "+ ip );
        transport.SetConnectionData(ip, 7777);

        NetworkManager.Singleton.StartClient();
        hideMe.SetActive(false);

        for (int i = 0; i < showMe.Length; i++)
        {
            showMe[i].SetActive(true);
        }

    }
}
