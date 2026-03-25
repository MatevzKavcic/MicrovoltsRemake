using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<int> Team = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );



    public override void OnNetworkSpawn()
    {
        LobbyManagerUI ui = FindFirstObjectByType<LobbyManagerUI>();

        if (IsServer)
        {
            PlayerName.Value = $"Player {OwnerClientId}"; //dej clientu id za ime sam da se ve
        }

        Team.OnValueChanged += OnTeamChanged;

        ChangeTeamServerRpc(1); // automatsko si doloèi team da slucajno nebos crashou
        ui.RefreshUI(); 
    }

    public void RequestTeamChange(int team)  // to bo referancou button in mu das se st zdraven.
    {
        if (IsOwner) {
            ChangeTeamServerRpc(team);
            Debug.Log("requesting to change the team to " + team);
        }

    }

    private void OnTeamChanged(int oldTeam, int newTeam)
    {
        LobbyManagerUI ui = FindFirstObjectByType<LobbyManagerUI>();

        ui.RefreshUI();

        Debug.Log(PlayerName.Value + " switched to team " + newTeam);
    }


    [ServerRpc]
    void ChangeTeamServerRpc(int team)
    {
        Team.Value = team;

    }

}