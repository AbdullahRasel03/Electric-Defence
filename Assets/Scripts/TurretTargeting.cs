using UnityEngine;

public class TurretTargeting : MonoBehaviour
{
    [Header("Targeting Settings")]
    public float targetRangeZ = 12f; // Distance along Z-axis
    [SerializeField] private float targetRefreshRate = 0.5f;

    [Header("Debug")]
    [SerializeField] private Enemy _currentTarget;
    private float _targetSearchCooldown;

    public Enemy CurrentTarget => _currentTarget;

    private void Start()
    {
        if (EnemyManager.Instance == null)
        {
            Debug.LogError("EnemyManager instance not found in scene!");
            enabled = false;
        }
    }

    private void Update()
    {
        UpdateTargetSearch();
    }

    private void UpdateTargetSearch()
    {
        if (_targetSearchCooldown > 0)
        {
            _targetSearchCooldown -= Time.deltaTime;
            return;
        }

        // Only search for new target if we don't have a valid current target
        if (!IsCurrentTargetValid())
        {
            FindNewTarget();
        }

        _targetSearchCooldown = targetRefreshRate;
    }

    private bool IsCurrentTargetValid()
    {
        if (_currentTarget == null) return false;

        // Check if target is active and in range
        float zDistance = Mathf.Abs(_currentTarget.transform.position.z - transform.position.z);
        return _currentTarget.IsActive && zDistance <= targetRangeZ;
    }

    private void FindNewTarget()
    {
        Enemy closestEnemy = null;
        float closestDistance = targetRangeZ;

        foreach (Enemy enemy in EnemyManager.Instance.ActiveEnemies)
        {
            if (!enemy.IsActive) continue;

            float zDistance = Mathf.Abs(enemy.transform.position.z - transform.position.z);

            if (zDistance <= targetRangeZ && zDistance < closestDistance)
            {
                closestDistance = zDistance;
                closestEnemy = enemy;
            }
        }

        _currentTarget = closestEnemy;
    }

    public bool HasActiveTarget()
    {
        return IsCurrentTargetValid();
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        // Z-axis range markers
        Gizmos.color = Color.cyan;
        Vector3 frontMarker = transform.position + Vector3.forward * targetRangeZ;
        Vector3 backMarker = transform.position - Vector3.forward * targetRangeZ;
        Gizmos.DrawSphere(frontMarker, 0.3f);
        Gizmos.DrawSphere(backMarker, 0.3f);

        // Range line
        Gizmos.DrawLine(frontMarker, backMarker);

        // Current target visualization
        if (_currentTarget != null)
        {
            Gizmos.color = _currentTarget.IsActive ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);

            // Draw a sphere around current target
            Gizmos.color = _currentTarget.IsActive ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_currentTarget.transform.position, 0.5f);
        }
    }
}