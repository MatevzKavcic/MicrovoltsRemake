using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public GameObject playerPrefab;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }
  

    public void StartGame()
    {
        if (!IsServer) return;

        NetworkManager.SceneManager.LoadScene(
            "GameScene",
            LoadSceneMode.Single
        );
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("Client connected: " + clientId);

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            SpawnPlayer(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsServer) return;

        if (scene.name == "GameScene")
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnPlayer(clientId);
                Debug.Log("Spawning Client  " + clientId);

            }
        }
    }
    //private void SpawnPlayer(ulong clientId)
    //{
    //    GameObject player = Instantiate(playerPrefab);
    //    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

    //}

    void SpawnPlayer(ulong clientId) // beter version od above... se transfera podatke o player teamu da pol se nemores streljat

    {
        var client = NetworkManager.Singleton.ConnectedClients[clientId];

        LobbyPlayer lobbyPlayer = client.PlayerObject.GetComponent<LobbyPlayer>();

        GameObject player = Instantiate(playerPrefab);

        PlayerStats gamePlayer = player.GetComponent<PlayerStats>();

        // data premakni u state
        gamePlayer.Team.Value = lobbyPlayer.Team.Value;
        gamePlayer.PlayerName.Value = lobbyPlayer.PlayerName.Value;

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}