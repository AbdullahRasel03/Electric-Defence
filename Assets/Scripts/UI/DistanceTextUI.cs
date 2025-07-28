using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using DG.Tweening;

public class DistanceTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceCount;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Transform safeZone;

    [SerializeField] private float updateInterval = 0.2f; // Update every 0.2 seconds to avoid performance issues
    private float nextUpdateTime = 0f;

    private bool canStartTimer = false;
    private Sequence seq;

    public void StartTimer()
    {
        canStartTimer = true;
        nextUpdateTime = Time.time + updateInterval; // Initialize the first update time
    }

    public void UpdateDistance(float distance)
    {
        if (distanceCount != null)
        {
            distanceCount.text = distance.ToString("F2") + "m";

            if (distance < 10f)
            {
                distanceCount.color = Color.red;
                SetTooCloseEffect();
            }
            else
            {
                distanceCount.color = Color.white;

                if (seq != null)
                {
                    seq.Kill();
                    distanceCount.transform.localScale = Vector3.one;
                }
            }
        }
    }

    private void SetTooCloseEffect()
    {
        if (seq != null)
        {
            seq.Kill();
            distanceCount.transform.localScale = Vector3.one;
        }

        seq = DOTween.Sequence();
        seq.Append(distanceCount.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBounce))
        .AppendInterval(0.2f)
        .Append(distanceCount.transform.DOScale(1f, 0.15f).SetEase(Ease.InBounce))
        .SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        if (!canStartTimer) return;

        if (Time.time >= nextUpdateTime && enemySpawner.activeEnemies != null && enemySpawner.activeEnemies.Count > 0)
        {
            nextUpdateTime = Time.time + updateInterval;
            FindClosestEnemyDistance();
        }
    }

    private void FindClosestEnemyDistance()
    {
        Vector3 safeZonePosition = safeZone.position;
        int enemyCount = enemySpawner.activeEnemies.Count;

        // Create native arrays for the job system
        NativeArray<float3> enemyPositions = new NativeArray<float3>(enemyCount, Allocator.TempJob);
        NativeArray<float> distances = new NativeArray<float>(enemyCount, Allocator.TempJob);

        // Fill the positions array
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemySpawner.activeEnemies[i] != null)
                enemyPositions[i] = enemySpawner.activeEnemies[i].transform.position;
        }

        // Create and schedule the job
        FindClosestEnemyJob job = new FindClosestEnemyJob
        {
            enemyPositions = enemyPositions,
            safeZonePosition = safeZonePosition,
            distances = distances
        };

        JobHandle jobHandle = job.Schedule(enemyCount, 64);
        jobHandle.Complete();

        // Find the minimum distance
        float minDistance = float.MaxValue;
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] < minDistance)
            {
                minDistance = distances[i];
            }
        }

        // Update the UI with the closest enemy distance
        UpdateDistance(minDistance);

        // Dispose native arrays
        enemyPositions.Dispose();
        distances.Dispose();
    }
}

[BurstCompile]
struct FindClosestEnemyJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> enemyPositions;
    [ReadOnly] public float3 safeZonePosition;
    [WriteOnly] public NativeArray<float> distances;

    public void Execute(int index)
    {
        distances[index] = math.distance(enemyPositions[index], safeZonePosition);
    }
}
