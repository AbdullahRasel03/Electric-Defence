using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyConfig
    {
        public GameObject prefab;
        [Range(0, 1)] public float spawnWeight = 1f;
        public float health;
    }


    [Header("Configuration")]
    [SerializeField] private EnemyConfig[] enemyConfigs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int maxActiveEnemies = 20;

    [Header("Runtime Info")]
    [SerializeField] public List<Enemy> activeEnemies = new List<Enemy>();

    private float nextSpawnTime;
    private bool isSpawning;

    private void Start()
    {
        InitializePool();
       // StartSpawning();
    }

    private void InitializePool()
    {
        if (ObjectPool.instance == null)
        {
            var poolObj = new GameObject("ObjectPool");
            poolObj.AddComponent<ObjectPool>();
        }

        foreach (var config in enemyConfigs)
        {
            for (int i = 0; i < 5; i++)
            {
                var obj = ObjectPool.instance.GetObject(config.prefab, false);
                ObjectPool.instance.ReturnToPool(obj);
            }
        }
    }

    private void Update()
    {
        if (!isSpawning || activeEnemies.Count >= maxActiveEnemies) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        SetNextSpawnTime();
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnEnemy()
    {
        if (activeEnemies.Count >= maxActiveEnemies) return;

        EnemyConfig config = GetRandomEnemyConfig();
        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject enemyObj = ObjectPool.instance.GetObject(
            config.prefab,
            true,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.ActivateEnemy(spawnPoint.position, spawnPoint.rotation, config.health);

        activeEnemies.Add(enemy);
    }

    private EnemyConfig GetRandomEnemyConfig()
    {
        float totalWeight = 0;
        foreach (var config in enemyConfigs)
            totalWeight += config.spawnWeight;

        float randomPoint = Random.value * totalWeight;

        foreach (var config in enemyConfigs)
        {
            if (randomPoint < config.spawnWeight)
                return config;

            randomPoint -= config.spawnWeight;
        }

        return enemyConfigs[0]; // fallback
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public void OnEnemyDefeated(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void CleanupAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            enemy.Die();
        }
        activeEnemies.Clear();
    }

    private void OnDestroy()
    {
        CleanupAllEnemies();
    }

    // ✅ Public trigger method for manual spawn
    public void TriggerSpawn()
    {
        SpawnEnemy();
    }

    public void SpawnSpecificEnemy(EnemySpawnData data)
    {
        var config = System.Array.Find(enemyConfigs, e => e.prefab.GetComponent<Enemy>().EnemyType == data.enemyType);
        if (config == null) return;

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject enemyObj = ObjectPool.instance.GetObject(
            config.prefab,
            true,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        float adjustedHealth = config.health * data.healthMultiplier;

        enemy.ActivateEnemy(spawnPoint.position, spawnPoint.rotation, adjustedHealth);
        activeEnemies.Add(enemy);
    }
}
