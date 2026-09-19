using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }  
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize = 15;
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();


    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        AddEnemyToPool(poolSize);
    }
    private void AddEnemyToPool(int count) 
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyList.Add(enemy);
            enemy.transform.parent = transform;
        }
    }
    public GameObject RequestEnemy() 
    {
        for (int i = 0; i < enemyList.Count; i++) 
        {
            if (!enemyList[i].activeSelf) 
            {
                enemyList[i].SetActive(true);
                return enemyList[i];
            }
        }
        AddEnemyToPool(1);
        enemyList[enemyList.Count - 1].SetActive(true);
        return enemyList[enemyList.Count - 1];

    }
    public int ActiveEnemiesCount()
    {
        int activeCount = 0;
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].activeInHierarchy)
            {
                activeCount++;
            }
        }
        return activeCount;
    }

}
