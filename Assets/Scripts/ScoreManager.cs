using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : NetworkBehaviour
{

    public TMP_Text teamScore1UI;
    public TMP_Text teamScore2UI;

    public NetworkVariable<int> Team1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> Team2Score = new NetworkVariable<int>(0);


    public int scoreToWin = 50;


    public static ScoreManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Team1Score.OnValueChanged += OnScoreChanged;
        Team2Score.OnValueChanged += OnScoreChanged;
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        teamScore1UI.text = Team1Score.Value.ToString();
        teamScore2UI.text = Team2Score.Value.ToString();
    }

    [ServerRpc]
    public void AddKillServerRpc(int killerTeam)
    {
        if (killerTeam == 1)
        {
            Team1Score.Value++;
        }
        else
        {
            Team2Score.Value++;
        }

        CheckWin();
    }

    private void CheckWin()
    {
        if (Team1Score.Value >= scoreToWin)
        {
            Debug.Log("TEAM 1 WINS");
        }

        if (Team2Score.Value >= scoreToWin)
        {
            Debug.Log("TEAM 2 WINS");
        }
    }



}
