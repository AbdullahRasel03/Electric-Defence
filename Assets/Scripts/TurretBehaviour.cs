using System.Collections;
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
    [SerializeField] private float aimThreshold = 5f;

    [Header("State")]
    [SerializeField] private bool isConnectedToGrid = false;

    private float _fireCooldown;
    private bool _isAimedAtTarget = false;
    private Enemy _currentTarget;
    private ObjectPool _bulletPool;


    [Header("Activation Settings")]
    private float activationDelay = 0.5f;
    private float activationDuration = 0.3f;
    private float deactivationDuration = 0.2f;
    [SerializeField] private ParticleSystem activationParticles;
    [SerializeField] private ParticleSystem deactivationParticles;
    [SerializeField] private AudioSource activationSound;
    [SerializeField] private AudioSource deactivationSound;
    [SerializeField] private MeshRenderer[] turretMeshes;
    [SerializeField] private float barrelTiltAngle = 15f;

    private bool isActivating = false;
    private bool isDeactivating = false;
    private Material[] turretMaterials;
    private Quaternion initialBarrelRotation;
    private Quaternion targetBarrelRotation;
    private float activationProgress;

    private void Awake()
    {
        InitializeComponents();
        ValidateDependencies();

        // Cache initial barrel rotation and calculate target
        initialBarrelRotation = Quaternion.Euler(barrelTiltAngle, 0f, 0f);
        targetBarrelRotation = Quaternion.Euler(0f, 0f, 0f); // Target upright rotation

        // Initialize materials
        if (turretMeshes != null && turretMeshes.Length > 0)
        {
            turretMaterials = new Material[turretMeshes.Length];
            for (int i = 0; i < turretMeshes.Length; i++)
            {
                if (turretMeshes[i] != null)
                {
                    turretMaterials[i] = turretMeshes[i].material;
                    // Set initial saturation to 0
                    turretMaterials[i].SetFloat("_Saturation", 0f);
                }
            }
        }
    }
    private void Start()
    {
        InitializeBulletPool();
    }
    private void Update()
    {
        if (isActivating)
        {
            UpdateActivationProgress();
        }
        else if (isDeactivating)
        {
            UpdateDeactivationProgress();
        }

        if (!isConnectedToGrid || isDeactivating) return;

        UpdateTargetStatus();
        UpdateCooldown();
        if (HasValidTarget())
        {
            RotateTowardsTarget();
            AttemptFire();
        }
    }

    private void UpdateActivationProgress()
    {
        activationProgress += Time.deltaTime / activationDuration;
        activationProgress = Mathf.Clamp01(activationProgress);

        // Animate saturation from 0 to 1
        float saturation = Mathf.Lerp(0f, 1f, activationProgress);
        UpdateMaterialSaturation(saturation);

        // Animate barrel rotation
        barrel.localRotation = Quaternion.Slerp(
            initialBarrelRotation,
            targetBarrelRotation,
            activationProgress
        );

        if (activationProgress >= 1f)
        {
            isActivating = false;
            isConnectedToGrid = true;
        }
    }

    private void UpdateDeactivationProgress()
    {
        activationProgress -= Time.deltaTime / deactivationDuration;
        activationProgress = Mathf.Clamp01(activationProgress);

        // Animate saturation from current value to 0
        float saturation = Mathf.Lerp(0f, 1f, activationProgress);
        UpdateMaterialSaturation(saturation);

        // Animate barrel rotation back to initial position
        barrel.localRotation = Quaternion.Slerp(
            initialBarrelRotation,
            targetBarrelRotation,
            activationProgress
        );

        if (activationProgress <= 0f)
        {
            isDeactivating = false;
            isConnectedToGrid = false;
        }
    }

    private void UpdateMaterialSaturation(float saturation)
    {
        foreach (var mat in turretMaterials)
        {
            if (mat != null) mat.SetFloat("_Saturation", saturation);
        }
    }

    public void InititateTurret()
    {
        if (!isActivating && !isConnectedToGrid && !isDeactivating)
        {
            StartCoroutine(ActivateTurretWithDelay());
        }
    }

    public void DeactivateTurret()
    {
        if (isConnectedToGrid && !isDeactivating && !isActivating)
        {
            StartDeactivation();
        }
    }

    private IEnumerator ActivateTurretWithDelay()
    {
        // Initial delay before activation starts
        yield return new WaitForSeconds(activationDelay);

        // Start activation sequence
        isActivating = true;
        activationProgress = 0f;

        // Play effects
        if (activationParticles != null) activationParticles.Play();
        if (activationSound != null) activationSound.Play();
    }

    private void StartDeactivation()
    {
        // Start deactivation sequence
        isDeactivating = true;
        activationProgress = 1f;

        // Play effects
        if (deactivationParticles != null) deactivationParticles.Play();
        if (deactivationSound != null) deactivationSound.Play();

        // Stop any active targeting/firing
        _currentTarget = null;
    }
    private void InitializeComponents()
    {
        if (targetingSystem == null) targetingSystem = GetComponent<TurretTargeting>();
        if (shootingPosition == null) shootingPosition = barrel;
    }

    private void ValidateDependencies()
    {
        Debug.Assert(barrel != null, "Barrel transform not assigned!", this);
        Debug.Assert(straightBulletPrefab != null, "Straight bullet prefab not assigned!", this);
        Debug.Assert(straightBulletPrefab.GetComponent<Rigidbody>() != null,
                   "Straight bullet needs Rigidbody!", this);
    }

    private void InitializeBulletPool()
    {
        _bulletPool = ObjectPool.instance;
        // Pre-warm the bullet pool
        for (int i = 0; i < 5; i++)
        {
            var bullet = Instantiate(straightBulletPrefab);
            bullet.SetActive(false);
            _bulletPool.ReturnToPool(bullet);
        }
    }

    private void UpdateTargetStatus()
    {
        bool hadTarget = _currentTarget != null;
        _currentTarget = targetingSystem.CurrentTarget;

        if (!hadTarget && _currentTarget != null)
        {
            // New target acquired
            _fireCooldown = 0; // Allow immediate shot at new target
        }
    }

    private bool HasValidTarget()
    {
        return _currentTarget != null &&
               _currentTarget.IsActive &&
               IsTargetInRange();
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = _currentTarget.transform.position - barrel.position;

        // Flatten the direction to only consider Y-axis rotation
        direction.y = 0;

        // Only rotate if there's meaningful direction (not zero vector)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            barrel.rotation = Quaternion.Slerp(barrel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update aim status (using flattened direction for angle calculation)
        _isAimedAtTarget = Vector3.Angle(barrel.forward, direction) <= aimThreshold;
    }

    private bool IsTargetInRange()
    {
        if (_currentTarget == null) return false;

        // Only check if target is in front of turret (Z-axis)
        float zDifference = _currentTarget.transform.position.z - transform.position.z;

        // For negative Z-axis movement (if enemies move toward -Z)
        return zDifference >= 0 && Mathf.Abs(zDifference) <= targetingSystem.targetRangeZ;
    }

    private void UpdateCooldown()
    {
        if (_fireCooldown > 0)
        {
            _fireCooldown -= Time.deltaTime;
        }
    }

    private void AttemptFire()
    {
        if (_fireCooldown > 0) return;
        if (projectileType == ProjectileType.Straight && !_isAimedAtTarget) return;

        //print("Fire");
        Fire();
        _fireCooldown = 1f / fireRate;
    }

    private void Fire()
    {
        var projectileObj = _bulletPool.GetObject(straightBulletPrefab, true, shootingPosition.position, barrel.rotation);

        if (projectileType == ProjectileType.Straight)
        {
            SetupStraightProjectile(projectileObj);
        }
    }

    private void SetupStraightProjectile(GameObject projectileObj)
    {
        var rb = projectileObj.GetComponent<Rigidbody>();
        rb.velocity = barrel.forward * projectileSpeed;

    }

    public void ConnectToGrid() => isConnectedToGrid = true;
    public void DisconnectFromGrid() => isConnectedToGrid = false;


}