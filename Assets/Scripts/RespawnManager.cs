using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Scene Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();

    private int nextSpawnIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Returns a RANDOM spawn point
    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned in RespawnManager!");
            return null;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    // Returns next spawn point in order (optional)
    public Transform GetNextSpawnPoint()
    {
        if (spawnPoints.Count == 0) return null;

        Transform t = spawnPoints[nextSpawnIndex];
        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPoints.Count;
        return t;
    }
}