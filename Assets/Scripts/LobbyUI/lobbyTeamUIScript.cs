using Unity.Netcode;
using UnityEngine;

public class lobbyTeamUIScript : NetworkBehaviour
{

    public int teamNumber;

    public LobbyPlayer[] GetPlayersInTeam()
    {
        var players = FindObjectsOfType<LobbyPlayer>(); // optimiziraj

        System.Collections.Generic.List<LobbyPlayer> teamPlayers = new();

        foreach (var player in players)
        {
            if (player.Team.Value == teamNumber)
            {
                teamPlayers.Add(player);
            }
        }

        return teamPlayers.ToArray();
    }


    public void showPlayersOnTeamUI()
    {
        var players = GetPlayersInTeam();
       
        foreach (var player in players)
        {
            Debug.Log("player ; " + player.name + "   team  : " + player.Team.Value);   
        }
    }


    private void Start()
    {
        InvokeRepeating(nameof(showPlayersOnTeamUI), 1f, 2f);
    }
}
