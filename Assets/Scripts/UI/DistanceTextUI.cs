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
    [SerializeField] private TMP_Text impactTimeText; // Renamed from distanceCount
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Transform safeZone;

    [SerializeField] private float updateInterval = 0.2f;
    private float nextUpdateTime = 0f;

    private bool canStartTimer = false;
    private Sequence seq;

    public void StartTimer(bool flag)
    {
        canStartTimer = flag;
        nextUpdateTime = Time.time + updateInterval;
    }

    public void UpdateImpactTime(float impactTime)
    {
        if (impactTimeText != null)
        {
            if (impactTime <= 0)
            {
                impactTimeText.color = Color.red;
                impactTimeText.text = "T -0.00s";
                if (seq != null)
                {
                    seq.Kill();
                    impactTimeText.transform.localScale = Vector3.one;
                }

                enemySpawner.OnEnemiesReachedSafeZone();
            }
            else
            {
                impactTimeText.text = "T -" + impactTime.ToString("F2") + "s";

                if (impactTime < 5f) // Warning when less than 5 seconds
                {
                    impactTimeText.color = Color.red;

                    if (seq == null || !seq.IsActive())
                    {
                        SetTooCloseEffect();
                    }
                }
                else if (impactTime < 10f && impactTime > 5f) // Warning when less than 10 seconds
                {
                    impactTimeText.color = Color.yellow;
                }
                else
                {
                    impactTimeText.color = Color.white;

                    if (seq != null)
                    {
                        seq.Kill();
                        impactTimeText.transform.localScale = Vector3.one;
                    }
                }
            }
        }
    }

    private void SetTooCloseEffect()
    {
        if (seq != null)
        {
            seq.Kill();
            impactTimeText.transform.localScale = Vector3.one;
        }

        seq = DOTween.Sequence();
        seq.Append(impactTimeText.transform.DOScale(1.2f, 0.15f))
        .AppendInterval(0.2f)
        .Append(impactTimeText.transform.DOScale(1f, 0.15f))
        .SetLoops(-1);
    }

    void Update()
    {
        if (!canStartTimer) return;

        if (Time.time >= nextUpdateTime && enemySpawner.activeEnemies != null && enemySpawner.activeEnemies.Count > 0)
        {
            nextUpdateTime = Time.time + updateInterval;
            FindClosestEnemyImpactTime();
        }
    }

    private void FindClosestEnemyImpactTime()
    {
        float3 safeZonePosition = safeZone.position;
        int enemyCount = enemySpawner.activeEnemies.Count;

        // Create native arrays for the job system
        NativeArray<float3> enemyPositions = new NativeArray<float3>(enemyCount, Allocator.TempJob);
        NativeArray<float> enemyVelocities = new NativeArray<float>(enemyCount, Allocator.TempJob);
        NativeArray<float> impactTimes = new NativeArray<float>(enemyCount, Allocator.TempJob);

        // Fill the arrays
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemySpawner.activeEnemies[i] != null)
            {
                enemyPositions[i] = enemySpawner.activeEnemies[i].transform.position;
                
                Enemy enemyMovement = enemySpawner.activeEnemies[i].GetComponent<Enemy>();
                float speed = enemyMovement != null ? enemyMovement.Speed : 5f;
                // Assuming enemies move towards safe zone (negative Z direction)
                enemyVelocities[i] = speed;
            }
        }

        // Create and schedule the job
        CalculateImpactTimeJob job = new CalculateImpactTimeJob
        {
            enemyPositions = enemyPositions,
            enemyVelocities = enemyVelocities,
            safeZonePosition = safeZonePosition,
            impactTimes = impactTimes
        };

        JobHandle jobHandle = job.Schedule(enemyCount, 64);
        jobHandle.Complete();

        // Find the minimum impact time
        float minImpactTime = float.MaxValue;
        for (int i = 0; i < impactTimes.Length; i++)
        {
            if (impactTimes[i] < minImpactTime)
            {
                minImpactTime = impactTimes[i];
            }
        }

        // Update the UI with the closest enemy impact time
        UpdateImpactTime(minImpactTime == float.MaxValue ? 0 : minImpactTime);

        // Dispose native arrays
        enemyPositions.Dispose();
        enemyVelocities.Dispose();
        impactTimes.Dispose();
    }
}
[BurstCompile]
struct CalculateImpactTimeJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> enemyPositions;
    [ReadOnly] public NativeArray<float> enemyVelocities;
    [ReadOnly] public float3 safeZonePosition;
    [WriteOnly] public NativeArray<float> impactTimes;

    public void Execute(int index)
    {
        float3 position = enemyPositions[index];
        float speed = enemyVelocities[index];

        if (speed > 0.001f) // Avoid division by zero
        {
            // Create a local copy for calculation
            float3 targetPosition = safeZonePosition;
            targetPosition.x = position.x; // Ensure X is the same for distance calculation
            targetPosition.y = position.y; // Ensure Y is the same for distance calculation

            // Calculate the direction from enemy to safe zone
            float3 directionToSafeZone = targetPosition - position;
            float distanceToSafeZone = math.length(directionToSafeZone);

            // Calculate how long it takes to reach the safe zone
            float timeToImpact = distanceToSafeZone / speed;

            // Check if enemy is moving towards safe zone (negative Z direction)
            // Enemy velocity is in negative Z direction when moving towards safe zone
            float3 enemyVelocityDirection = new float3(0, 0, -1); // Enemies move towards safe zone
            float3 normalizedDirectionToSafeZone = math.normalize(directionToSafeZone);

            // Dot product: 1 means moving directly towards, -1 means moving away
            float dotProduct = math.dot(normalizedDirectionToSafeZone, enemyVelocityDirection);

            // If moving away from safe zone (dot product < 0), set time to a large value
            if (dotProduct < 0)
            {
                timeToImpact = -1;
            }

            impactTimes[index] = timeToImpact;
        }
        else
        {
            impactTimes[index] = float.MaxValue;
        }
    }
}

