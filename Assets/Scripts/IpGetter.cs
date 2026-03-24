using UnityEngine;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class IpGetter : MonoBehaviour
{
    public TMP_Text hintText;

    void Start()
    {
        hintText.text = GetLocalIPAddress();
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "No IPv4 found";
    }
}