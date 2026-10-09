using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEditor.EditorTools;


public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    public List<PoolConfig> poolConfigs;
    private Dictionary<string, ObjectPool<GameObject>> pools;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePools();
    }
    private void InitializePools()
    {
        pools = new Dictionary<string, ObjectPool<GameObject>>();

        foreach (var config in poolConfigs)
        {
            if (config.prefab == null || string.IsNullOrEmpty(config.poolID))
            {
                Debug.LogWarning("no ID", this);
                continue;
            }

            if (pools.ContainsKey(config.poolID))
            {
                Debug.LogWarning("SAME ID", this);
                continue;
            }

            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(config.prefab, transform);
                    obj.SetActive(false); 
                    return obj;
                },
                actionOnRelease: (obj) => obj.SetActive(false),  
                actionOnDestroy: (obj) => Destroy(obj),          
                collectionCheck: true,                          
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );

            pools.Add(config.poolID, newPool);
        }
    }
    public GameObject Get(string poolID, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(poolID, out var pool))
        {
            Debug.LogWarning($"NOT EXISTS '{poolID}'.", this);
            return null;
        }
        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true); 

        if (obj.TryGetComponent(out IPooleable pooleable))
        {
            pooleable.OnObjectSpawn();
        }
        return obj;
    }

    public void Release(string poolID, GameObject obj)
    {
        if (pools.TryGetValue(poolID, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"NOT EXISTS '{poolID}'.", this);
        }
    }

    [System.Serializable]
    public class PoolConfig
    {
        public string poolID;
        public GameObject prefab;
        public int defaultCapacity = 20;
        public int maxSize = 100;
    }
}