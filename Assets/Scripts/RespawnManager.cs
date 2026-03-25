using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Scene Spawn Points")]
    public List<Transform> spawnPointsTeam1 = new List<Transform>();

    public List<Transform> spawnPointsTeam2 = new List<Transform>();


    private int nextSpawnIndexTeam1 = 0;
    private int nextSpawnIndexTeam2 = 0;


    private void Awake()
    {
        Instance = this;
    }

    // Returns a RANDOM spawn point
    //public Transform GetRandomSpawnPoint()
    //{
    //    if (spawnPoints.Count == 0)
    //    {
    //        Debug.LogError("No spawn points assigned in RespawnManager!");
    //        return null;
    //    }

    //    return spawnPoints[Random.Range(0, spawnPoints.Count)];
    //}

    // Returns next spawn point in order (optional)
    public Transform GetNextSpawnPoint(int Team)
    {

        if (Team == 1)
        {
            if (spawnPointsTeam1.Count == 0) return null;

            Transform t = spawnPointsTeam1[nextSpawnIndexTeam1];
            nextSpawnIndexTeam1 = (nextSpawnIndexTeam1 + 1) % spawnPointsTeam1.Count;
            return t;
        }
        else if (Team == 2)
        {
            if (spawnPointsTeam2.Count == 0) return null;

            Transform t = spawnPointsTeam2[nextSpawnIndexTeam2];
            nextSpawnIndexTeam2 = (nextSpawnIndexTeam2 + 1) % spawnPointsTeam2.Count;
            return t;
        }
        else
        {
            return null; // nimas assignanega teama si nullan MUAHAHAHHA
        }

        
    }
}