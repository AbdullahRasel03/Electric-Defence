using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//NOTE: Create Scriptable Object for Wave Data
[CreateAssetMenu(fileName = "WaveData", menuName = "ScriptableObjects/WaveDataSO", order = 1)]
public class WaveDataSO : ScriptableObject
{

    public List<EnemySpawnData> enemyForWaves = new List<EnemySpawnData>();
    
    public bool hasMidWaveBoss;
    public bool isBossWave;
    
    [Header("Wave Timers")]
    [Tooltip("Initial Delay")] public float waveStartDelay;
    public float perWaveDelay;

    [Space] public int waveGemBonus;
    
    public static Action<int> OnWaveStarted;
    public static Action<int> OnWaveEnded;
    
    private int totalEnemies;
    private float totalMinInterval;
    public int TotalEnemies => totalEnemies;
    public float TotalMinInterval => totalMinInterval;
    
    public int GetTotalEnemies()
    {
        int total = 0;
        foreach (var enemy in enemyForWaves)
        {
            if (enemy != null)
                total += enemy.spawnCount;
        }
        return total;
    }
    
    public void UpdateTotalEnemies()
    {
        totalEnemies = 0;
        foreach (var enemy in enemyForWaves)
        {
            if (enemy != null)
                totalEnemies += enemy.spawnCount;
        } 
    }

    public void GetTotalMinInterval()
    {
        totalMinInterval = 0;
        foreach (var enemy in enemyForWaves)
        {
            if (enemy != null)
                totalMinInterval += (Mathf.FloorToInt(enemy.minInterval)*enemy.spawnCount);
        }
    }
}

[Serializable]
public class EnemySpawnData
{
    public EnemyType enemyType;
    public int spawnCount;

    [Space] 
    public float healthMultiplier = 1;
    public float attackMultiplier = 1;
    
    [Space]
    public float minInterval; 
    public float maxInterval;
}