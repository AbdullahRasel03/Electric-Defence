using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
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
    [SerializeField] private Camera topDownCam;
    [SerializeField] private Camera topDownNonPPCam;
    [SerializeField] private Camera tpCam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SocketManager socketManager;
    [SerializeField] private List<Turret> allTurrets;

    [Header("Runtime Info")]
    [SerializeField] public List<Enemy> activeEnemies = new List<Enemy>();

    private float nextSpawnTime;
    private float currentTime;
    private bool isSpawning;

    public static event Action OnSpawnStarted;

    private void Start()
    {
        InitializePool();
        //StartSpawning();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpawning();
        }
        if (!isSpawning || activeEnemies.Count >= maxActiveEnemies) return;

        currentTime += Time.deltaTime;


        if (currentTime >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    public void StartSpawning()
    {
        OnSpawnStarted?.Invoke();
        allTurrets.ForEach(x => x.Activate());

        canvas.SetActive(false);
        socketManager.ResetAllSockets();
        nextSpawnTime = 4f;
        isSpawning = true;
        topDownNonPPCam.orthographic = false;
        topDownCam.orthographic = false;

        allTurrets.ForEach(turret => turret.RotateFireRateText());

        topDownCam.transform.DOMove(tpCam.transform.position, 1.5f);
        topDownCam.transform.DORotate(tpCam.transform.rotation.eulerAngles, 1.5f);

        DOTween.To(() => topDownCam.fieldOfView, x => topDownCam.fieldOfView = x, tpCam.fieldOfView, 1.5f);
        DOTween.To(() => topDownNonPPCam.fieldOfView, x => topDownNonPPCam.fieldOfView = x, tpCam.fieldOfView, 1.5f);


        // SetNextSpawnTime();
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
        Vector3 spawnOffset = new Vector3(0, 0, 35f);

        GameObject enemyObj = ObjectPool.instance.GetObject(
            config.prefab,
            true,
            spawnPoint.position + spawnOffset,
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
