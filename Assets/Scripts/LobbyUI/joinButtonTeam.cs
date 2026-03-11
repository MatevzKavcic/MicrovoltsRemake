using UnityEngine;

public class JoinTeamButton : MonoBehaviour
{
    public int teamNumber;

    public void JoinTeam()
    {
        LobbyPlayer localPlayer = GetLocalLobbyPlayer();

        if (localPlayer != null)
        {
            localPlayer.RequestTeamChange(teamNumber);
        }
        else
        {
            Debug.LogWarning("Local LobbyPlayer not found");
        }
    }

    LobbyPlayer GetLocalLobbyPlayer()
    {
        LobbyPlayer[] players = FindObjectsOfType<LobbyPlayer>();

        foreach (var player in players)
        {
            if (player.IsOwner)
            {
                return player;
            }
        }

        return null;
    }
}