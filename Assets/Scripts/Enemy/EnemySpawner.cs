using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int concurrentEnemies = 15; 
    public float respawnDelay = 3f;    

    void Start()
    {
        
        for (int i = 0; i < concurrentEnemies; i++)
        {
            SpawnSingleEnemy();
        }

        StartCoroutine(RespawnMonitor());
    }
    private void SpawnSingleEnemy()
    {
        GameObject enemy = EnemyPool.Instance.RequestEnemy();
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        enemy.transform.position = transform.position + randomOffset;
    }
    private IEnumerator RespawnMonitor()
    {
        while (true)
        {
            if (EnemyPool.Instance.ActiveEnemiesCount() < concurrentEnemies)
            {
                SpawnSingleEnemy();
            }
            yield return new WaitForSeconds(respawnDelay);
        }
    }
}