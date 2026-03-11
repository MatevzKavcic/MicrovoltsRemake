using UnityEngine;

public class LobbyManagerUI : MonoBehaviour
{
    public Transform team1Container;
    public Transform team2Container;

    public InLobbyPlayerUserInterface playerRowPrefab;

    public void RefreshUI()
    {
        ClearUI();

        LobbyPlayer[] players = FindObjectsOfType<LobbyPlayer>();

        foreach (var player in players)
        {
            Transform parent = player.Team.Value == 1 ? team1Container : team2Container;

            InLobbyPlayerUserInterface row = Instantiate(playerRowPrefab, parent);

            row.Setup(player.PlayerName.Value.ToString()+" banana");
        }
    }

    void ClearUI()
    {
        foreach (Transform child in team1Container)
            Destroy(child.gameObject);

        foreach (Transform child in team2Container)
            Destroy(child.gameObject);
    }
}