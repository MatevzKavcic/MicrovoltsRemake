using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class PlayerOverHead : NetworkBehaviour
{
    public static PlayerStats LocalPlayer;

    public override void OnNetworkSpawn()
    {

        Invoke(nameof(SetupMarker), 2f); // small delay = ensure all spawned

    }

    void SetupMarker()
    {
        PlayerStats myPlayer = PlayerStats.LocalPlayer;
        PlayerStats target = GetComponentInParent<PlayerStats>();

        if (myPlayer == null)
        {
            Debug.Log("  I CANT FIND MY PLAYEEA  ");
            return;
        }

        Debug.Log("Seting target "+ target.GetHashCode());

        Debug.Log("Seting target haskoda my player" + myPlayer.GetHashCode());


        if (target == null)
        {
            Debug.Log("i didnt find shit");
            return;

        } 

        if (target == myPlayer)
        {
            // Hide your own marker
            myPlayer.markerImageEnemy.SetActive(false);
            myPlayer.markerImageFriendly.SetActive(false);
            Debug.Log("this is me i will defect myself");


            return;
        }

        if (target.Team.Value == myPlayer.Team.Value)
        {
            target.markerImageFriendly.SetActive(true);
            target.markerImageEnemy.SetActive(false);
            Debug.Log("this is my teammate i give him green");

        }
        else
        {
            target.markerImageFriendly.SetActive(false);
            target.markerImageEnemy.SetActive(true);
            Debug.Log("this is my enemyy i give him red");

        }
    }
}