using UnityEngine;

[RequireComponent(typeof(TurretTargeting))]
public class TurretBehaviour : MonoBehaviour
{
    public enum ProjectileType { Straight, Homing }

    [Header("Dependencies")]
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform shootingPosition;
    [SerializeField] private GameObject straightBulletPrefab;
    [SerializeField] private TurretTargeting targetingSystem;

    [Header("Configuration")]
    [SerializeField] private ProjectileType projectileType = ProjectileType.Straight;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float attackRange = 25f;
    [SerializeField] private float aimThreshold = 5f;

    [Header("State")]
    [SerializeField] private bool isConnectedToGrid = false;

    private float _fireCooldown;
    private bool _hasValidTarget = false;
    private bool _isAimedAtTarget = false;

    private void Awake()
    {
        if (targetingSystem == null) targetingSystem = GetComponent<TurretTargeting>();
        ValidateDependencies();
    }

    private void ValidateDependencies()
    {
        if (barrel == null) Debug.LogError("Barrel transform not assigned!", this);
        if (shootingPosition == null) shootingPosition = barrel;
        if (straightBulletPrefab == null) Debug.LogError("Straight bullet prefab not assigned!", this);
        if (straightBulletPrefab?.GetComponent<Rigidbody>() == null) Debug.LogError("Straight bullet needs Rigidbody!", this);
      
    }

    private void Update()
    {
        if (!isConnectedToGrid) return;

        UpdateTargeting();
        UpdateCooldown();
        AttemptFire();
    }

    private void UpdateTargeting()
    {
        _hasValidTarget = targetingSystem.HasActiveTarget(out var target);
        if (_hasValidTarget)
        {
            RotateTowardsTarget(target);
            CheckAim(target);
        }
        else
        {
            _isAimedAtTarget = false;
        }
    }

    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - barrel.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        barrel.rotation = Quaternion.Slerp(barrel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void CheckAim(Transform target)
    {
        Vector3 targetDirection = (target.position - barrel.position).normalized;
        _isAimedAtTarget = Vector3.Angle(barrel.forward, targetDirection) <= aimThreshold;
    }

    private void UpdateCooldown()
    {
        if (_fireCooldown > 0)
        {
            _fireCooldown -= Time.deltaTime;
        }
    }

    private bool IsTargetInRange()
    {
        if (!targetingSystem.HasActiveTarget(out var target)) return false;
        float distance = Vector3.Distance(shootingPosition.position, target.position);
        return distance <= attackRange;
    }

    private void AttemptFire()
    {
        if (!_hasValidTarget || _fireCooldown > 0 || !IsTargetInRange()) return;
        if (projectileType == ProjectileType.Straight && !_isAimedAtTarget) return;

        Fire();
        _fireCooldown = 1f / fireRate;
    }

    private void Fire()
    {
        var projectileObj = Instantiate(straightBulletPrefab, shootingPosition.position, shootingPosition.rotation);

        if (projectileType == ProjectileType.Straight)
        {
            SetupStraightProjectile(projectileObj);
        }
      
    }

    private void SetupStraightProjectile(GameObject projectileObj)
    {
        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
        rb.velocity = barrel.forward * projectileSpeed;
        rb.isKinematic = false;
        Destroy(projectileObj, 5f);
    }


    public void ConnectToGrid() => isConnectedToGrid = true;
    public void DisconnectFromGrid() => isConnectedToGrid = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(shootingPosition != null ? shootingPosition.position : transform.position, attackRange);

        if (shootingPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(shootingPosition.position, 0.1f);
        }
    }
}

