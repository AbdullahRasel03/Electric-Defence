using System.Collections;
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
        public float speed;
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount = 10;
        public float spawnInterval = 0.5f;
    }

    [Header("Configuration")]
    [SerializeField] private EnemyConfig[] enemyConfigs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int maxActiveEnemies = 20;

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private float interWaveDelay = 3f;
    public int currentWaveIndex = -1;

    [Header("Runtime Info")]
    [SerializeField] public List<Enemy> activeEnemies = new List<Enemy>();

    private float nextSpawnTime;
    private bool isSpawning;
    private bool isWaveMode = false;

    private void Start()
    {
        InitializePool();
        // StartSpawning(); // Manual or wave-based
        // StartWaves();    // Uncomment to start waves automatically
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

        if (!isWaveMode && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        isWaveMode = false;
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
        enemy.ActivateEnemy(spawnPoint.position, spawnPoint.rotation, config.health, config.speed);

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
        // Auto progress wave if all enemies are dead
        if (isWaveMode && activeEnemies.Count == 0)
        {
            StartCoroutine(SpawnNextWaveWithDelay());
        }
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

    public void TriggerSpawn()
    {
        SpawnEnemy();
    }

    // ✅ WAVE SYSTEM STARTS HERE
    public void StartWaves()
    {
        if (waves.Count == 0) return;

        isSpawning = true;
        isWaveMode = true;
        currentWaveIndex = -1;

        StartCoroutine(SpawnNextWaveWithDelay());
    }

    private IEnumerator SpawnNextWaveWithDelay()
    {
        yield return new WaitForSeconds(interWaveDelay);
        SpawnNextWave();
    }

    private void SpawnNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            isSpawning = false;
            return;
        }

        Wave wave = waves[currentWaveIndex];
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            if (activeEnemies.Count < maxActiveEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }
}
