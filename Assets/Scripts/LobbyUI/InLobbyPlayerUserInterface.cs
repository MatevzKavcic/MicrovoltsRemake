using TMPro;
using UnityEngine;

public class InLobbyPlayerUserInterface : MonoBehaviour
{
    public TMP_Text playerNameText;

    public void Setup(string playerName)
    {
        playerNameText.text = playerName;
    }
}
