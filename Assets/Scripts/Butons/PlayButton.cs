using System;
using Unity.Netcode;
using UnityEngine;

public class PlayButton : NetworkBehaviour
{
    public void StartGame()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene(
            "GameScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }
}
