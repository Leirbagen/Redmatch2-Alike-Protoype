using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance { get; private set; }
    [SerializeField] private Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy() 
    { 
        if (Instance == this)
        {
            Instance = null;
        }
        
    }
    
    public Transform GetRandomSpawnPoint()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return spawnPoint;
    }
}
