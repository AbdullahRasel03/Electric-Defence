using UnityEngine;

public class TowerTargetingSystem : MonoBehaviour
{
    [SerializeField] private float range = 12f;
    private Enemy currentTarget;
    private float refreshCooldown = 0.2f;
    private float timer;

    public Enemy GetCurrentTarget() => currentTarget;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            currentTarget = FindClosestEnemy();
            timer = refreshCooldown;
        }
    }

    private Enemy FindClosestEnemy()
    {
        float closestDist = range;
        Enemy best = null;

        foreach (var enemy in EnemyManager.Instance.ActiveEnemies)
        {
            if (!enemy.IsActive) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= range && dist < closestDist)
            {
                closestDist = dist;
                best = enemy;
            }
        }

        return best;
    }
}
