using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using Unity.Mathematics;
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
    [SerializeField] private float initialSpawnDelay = 2f;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private int maxActiveEnemies = 20;
    [SerializeField] private int minEnemyBatchSize = 5;
    [SerializeField] private int maxEnemyBatchSize = 6;
    [SerializeField] private Camera topDownCam;
    [SerializeField] private Camera topDownNonPPCam;
    [SerializeField] private Camera tpCam;
    [SerializeField] private Camera uiCam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SocketManager socketManager;
    [SerializeField] private List<Turret> allTurrets;
    [SerializeField] private EnemyConfig bossConfig;
    [SerializeField] private int spawnBossAfterBatch = 3;
    

    [Header("Runtime Info")]
    [SerializeField] public List<Enemy> activeEnemies = new List<Enemy>();

    [Space(15)]
    [SerializeField] private DistanceTextUI distanceTextUI;
    [SerializeField] private bool isReflectorGameplay = false;

    [Space(20)]
    [SerializeField] private VictoryPopup victoryPopup;
    [SerializeField] private LostPopup lostPopup;
    

    private float nextSpawnTime;
    private float currentTime;
    private int currentSpawnCount = 0;
    private bool isSpawning;
    private int currentSpawnPoint = 0;
    private int currentBatchCount = 0;
    private bool isBossSpawned = false;

    public static event Action OnSpawnStarted;

    private void Start()
    {
        InitializePool();
        Enemy.OnEnemyDead += OnEnemyDefeated;
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
            currentTime = 0f;
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    public void StartSpawning()
    {
        OnSpawnStarted?.Invoke();

        if (!isReflectorGameplay)
            allTurrets.ForEach(x => x.Activate());

        distanceTextUI.gameObject.SetActive(true);

        canvas.SetActive(false);
        socketManager.ResetAllSockets();
        nextSpawnTime = initialSpawnDelay;
        isSpawning = true;
        topDownNonPPCam.orthographic = false;
        uiCam.orthographic = false;
        topDownCam.orthographic = false;
        topDownCam.fieldOfView = 54.5f;
        topDownNonPPCam.fieldOfView = 54.5f;
        uiCam.fieldOfView = 54.5f;

        allTurrets.ForEach(turret => { turret.HideFireRateText(); turret.SetFireRateSlider(); });

        topDownCam.transform.DOMove(tpCam.transform.position, 1.5f);
        topDownCam.transform.DORotate(tpCam.transform.rotation.eulerAngles, 1.5f);

        DOTween.To(() => topDownCam.fieldOfView, x => topDownCam.fieldOfView = x, tpCam.fieldOfView, 1.5f);
        DOTween.To(() => topDownNonPPCam.fieldOfView, x => topDownNonPPCam.fieldOfView = x, tpCam.fieldOfView, 1.5f);
        DOTween.To(() => uiCam.fieldOfView, x => uiCam.fieldOfView = x, tpCam.fieldOfView, 1.5f);

        DOVirtual.DelayedCall(3.5f, () =>
        {
            distanceTextUI.StartTimer(true);
        });


        // SetNextSpawnTime();
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnEnemy()
    {
        if (currentSpawnCount >= maxActiveEnemies) return;

        int batchSize = currentBatchCount > spawnBossAfterBatch && !isBossSpawned? 3 : Random.Range(minEnemyBatchSize, maxEnemyBatchSize + 1);

        for (int i = 0; i < batchSize; i++)
        {
            EnemyConfig config = GetRandomEnemyConfig();

            if (currentBatchCount > spawnBossAfterBatch && !isBossSpawned && i == 1)
            {
                config = bossConfig;
                isBossSpawned = true;
            }
            
            Transform spawnPoint = GetRandomSpawnPoint();
            Vector3 spawnOffset = new Vector3(0, 0, 35f);

            GameObject enemyObj = ObjectPool.instance.GetObject(
                config.prefab,
                true,
                spawnPoint.position + spawnOffset,
                quaternion.identity
            );

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.ActivateEnemy(spawnPoint.position + new Vector3(0, 0, Random.Range(-10, 10)), quaternion.identity, config.health);

            activeEnemies.Add(enemy);

            currentSpawnCount++;
        }

        currentBatchCount++;
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
        int spawnPointIndex = currentSpawnPoint;
        currentSpawnPoint = (spawnPointIndex + 1) % spawnPoints.Length;
        return spawnPoints[spawnPointIndex];
    }

    public void OnEnemyDefeated(Enemy enemy)
    {
        activeEnemies.Remove(enemy);

        if (currentSpawnCount >= maxActiveEnemies && activeEnemies.Count == 0)
        {
            isSpawning = false;
            distanceTextUI.StartTimer(false);
            distanceTextUI.gameObject.SetActive(false);
            victoryPopup.SetView(true);
        }
    }

    public void OnEnemiesReachedSafeZone()
    {
        isSpawning = false;
        distanceTextUI.StartTimer(false);
        distanceTextUI.gameObject.SetActive(false);
        lostPopup.SetView(true);
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
        Enemy.OnEnemyDead -= OnEnemyDefeated;
    }

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
