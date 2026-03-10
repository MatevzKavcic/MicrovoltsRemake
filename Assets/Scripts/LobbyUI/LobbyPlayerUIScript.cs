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


    public void joinTeam()
    {
        ChangeTeamServerRpc(1); //joinej ga u prvi team automatsko ko ga sisa....
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerName.Value = $"Player {OwnerClientId}";
        }
        joinTeam();
    }

    public void RequestTeamChange(int team)
    {
        if (IsOwner)
            ChangeTeamServerRpc(team);
    }

    [ServerRpc]
    void ChangeTeamServerRpc(int team)
    {
        Team.Value = team;
    }
}