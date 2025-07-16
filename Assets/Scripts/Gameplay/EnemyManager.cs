using UnityEngine;
using System.Collections.Generic;

public enum EnemyType
{
    Skeleton, Gazer, Pig, Goblin, Troll, Dragon, Golem, Phoenix, Zombie, Vampire, Werewolf
}
public class EnemyManager : MonoBehaviour
{

    public static EnemyManager Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private EnemySpawner spawner;

    [Header("Debug")]
    [SerializeField] private List<Enemy> _activeEnemies = new List<Enemy>();

    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

   
    public void RegisterEnemy(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Add(enemy);
            enemy.OnDeath += UnregisterEnemy;
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            enemy.OnDeath -= UnregisterEnemy;
            _activeEnemies.Remove(enemy);
        }
    }

    public void DamageAllActiveEnemies(float damage)
    {
        // Create a copy to avoid modification during iteration
        var enemiesToDamage = new List<Enemy>(_activeEnemies);

        foreach (var enemy in enemiesToDamage)
        {
            if (enemy != null && enemy.IsActive)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public Enemy GetClosestEnemy(Vector3 position, float maxRange = Mathf.Infinity)
    {
        Enemy closestEnemy = null;
        float closestDistance = maxRange;

        foreach (var enemy in _activeEnemies)
        {
            if (!enemy.IsActive) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void OnDestroy()
    {
        // Cleanup all event subscriptions
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                enemy.OnDeath -= UnregisterEnemy;
            }
        }

        spawner.CleanupAllEnemies();
    }
}