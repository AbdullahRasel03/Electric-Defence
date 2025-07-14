using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private List<WaveDataSO> waves;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float delayBeforeWaveStart = 1f;

    [Header("UI")]
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private CamController camController;
    [SerializeField] private SocketManager socketManager;

    public int currentWaveIndex = 0;
    private bool waveInProgress = false;

    public void StartFirstWave()
    {
        currentWaveIndex = 0;
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (waveInProgress || currentWaveIndex >= waves.Count) return;

        StartCoroutine(RunWave(waves[currentWaveIndex]));
        currentWaveIndex++;
    }

    private IEnumerator RunWave(WaveDataSO waveData)
    {
        waveInProgress = true;

        waveData.UpdateTotalEnemies();
        waveData.GetTotalMinInterval();

        if (delayBeforeWaveStart > 0)
            yield return new WaitForSeconds(delayBeforeWaveStart);

        WaveDataSO.OnWaveStarted?.Invoke(currentWaveIndex);

        if (waveNumberText != null)
            waveNumberText.text = $"Wave {currentWaveIndex}";

        foreach (var enemyData in waveData.enemyForWaves)
        {
            for (int i = 0; i < enemyData.spawnCount; i++)
            {
                spawner.SpawnSpecificEnemy(enemyData);
                yield return new WaitForSeconds(Random.Range(enemyData.minInterval, enemyData.maxInterval));
            }
        }

        yield return new WaitUntil(() => EnemyManager.Instance.ActiveEnemies.Count == 0);

        WaveDataSO.OnWaveEnded?.Invoke(currentWaveIndex);
        waveInProgress = false;

        ShowShop();
    }

    private void ShowShop()
    {
        if (waveNumberText != null)
            waveNumberText.text = "Shopping Break!";

        camController.SetShopView();
        socketManager.RefreshSockets();

        // ✅ From here, the game waits for player to press a "Start Next Wave" button in UI
    }
}
