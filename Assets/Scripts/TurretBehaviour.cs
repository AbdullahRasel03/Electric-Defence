using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Obi;

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
    [SerializeField] private float fireRate = 0;
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

    [Header("Rope Config")]
    public ObiRope rope;
    public GameObject electricityVisualPrefab;
    public float electricitySpeed = 0.3f;

    private List<GameObject> activeElectricityVisuals = new List<GameObject>();
    private bool isFiringSequenceActive = false;

    private void Awake()
    {
        InitializeComponents();
        ValidateDependencies();

        initialBarrelRotation = Quaternion.Euler(barrelTiltAngle, 0f, 0f);
        targetBarrelRotation = Quaternion.Euler(0f, 0f, 0f);

        if (turretMeshes != null && turretMeshes.Length > 0)
        {
            turretMaterials = new Material[turretMeshes.Length];
            for (int i = 0; i < turretMeshes.Length; i++)
            {
                if (turretMeshes[i] != null)
                {
                    turretMaterials[i] = turretMeshes[i].material;
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
        if (isActivating) UpdateActivationProgress();
        else if (isDeactivating) UpdateDeactivationProgress();

        if (!isConnectedToGrid || isDeactivating) return;

        UpdateTargetStatus();
        UpdateCooldown();

        if (HasValidTarget())
        {
            RotateTowardsTarget();
            if(fireRate > 0) AttemptFire();
        }
    }

    private void UpdateActivationProgress()
    {
        activationProgress += Time.deltaTime / activationDuration;
        activationProgress = Mathf.Clamp01(activationProgress);

        float saturation = Mathf.Lerp(0f, 1f, activationProgress);
        UpdateMaterialSaturation(saturation);
        barrel.localRotation = Quaternion.Slerp(initialBarrelRotation, targetBarrelRotation, activationProgress);

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

        float saturation = Mathf.Lerp(0f, 1f, activationProgress);
        UpdateMaterialSaturation(saturation);
        barrel.localRotation = Quaternion.Slerp(initialBarrelRotation, targetBarrelRotation, activationProgress);

        if (activationProgress <= 0f)
        {
            isDeactivating = false;
            isConnectedToGrid = false;
        }
    }

    private void UpdateMaterialSaturation(float saturation)
    {
        foreach (var mat in turretMaterials)
            if (mat != null) mat.SetFloat("_Saturation", saturation);
    }

    public void InititateTurret()
    {
        if (!isActivating && !isConnectedToGrid && !isDeactivating)
            StartCoroutine(ActivateTurretWithDelay());
    }

    public void DeactivateTurret()
    {
        if (isConnectedToGrid && !isDeactivating && !isActivating)
            StartDeactivation();
    }

    private IEnumerator ActivateTurretWithDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        isActivating = true;
        activationProgress = 0f;

        if (activationParticles != null) activationParticles.Play();
        if (activationSound != null) activationSound.Play();
    }

    private void StartDeactivation()
    {
        isDeactivating = true;
        activationProgress = 1f;

        if (deactivationParticles != null) deactivationParticles.Play();
        if (deactivationSound != null) deactivationSound.Play();

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
        Debug.Assert(straightBulletPrefab.GetComponent<Rigidbody>() != null, "Straight bullet needs Rigidbody!", this);
    }

    private void InitializeBulletPool()
    {
        _bulletPool = ObjectPool.instance;
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
        if (!hadTarget && _currentTarget != null) _fireCooldown = 0;
    }

    private bool HasValidTarget()
    {
        return _currentTarget != null && _currentTarget.IsActive && IsTargetInRange();
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = _currentTarget.transform.position - barrel.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            barrel.rotation = Quaternion.Slerp(barrel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        _isAimedAtTarget = Vector3.Angle(barrel.forward, direction) <= aimThreshold;
    }

    private bool IsTargetInRange()
    {
        if (_currentTarget == null) return false;

        float zDiff = _currentTarget.transform.position.z - transform.position.z;
        return zDiff >= 0 && Mathf.Abs(zDiff) <= targetingSystem.targetRangeZ;
    }

    private void UpdateCooldown()
    {
        if (_fireCooldown > 0)
            _fireCooldown -= Time.deltaTime;
    }

    private void AttemptFire()
    {
        if (_fireCooldown > 0) return;
        if (projectileType == ProjectileType.Straight && !_isAimedAtTarget) return;

        StartCoroutine(FireWithElectricity());
        _fireCooldown = 1f / fireRate;
    }

    private void Fire()
    {
        var projectileObj = _bulletPool.GetObject(straightBulletPrefab, true, shootingPosition.position, barrel.rotation);
        if (projectileType == ProjectileType.Straight)
            SetupStraightProjectile(projectileObj);
    }

    private void SetupStraightProjectile(GameObject projectileObj)
    {
        var rb = projectileObj.GetComponent<Rigidbody>();
        rb.velocity = barrel.forward * projectileSpeed;
    }

    public void ConnectToGrid() => isConnectedToGrid = true;
    public void DisconnectFromGrid() => isConnectedToGrid = false;
    public void UpdateFireRate(float toAdd) => fireRate += toAdd;
    public void ResetFireRate() => fireRate = 0;

    private IEnumerator FireWithElectricity()
    {
        if (isFiringSequenceActive) yield break;
        isFiringSequenceActive = true;

        if (rope == null || rope.solver == null || rope.activeParticleCount == 0)
        {
            Fire();
            yield break;
        }

        // Calculate the time between shots based on fire rate
        float timeBetweenShots = 1f / fireRate;

        // Start the first shot immediately
        StartCoroutine(AnimateElectricityAndFire(0f));

        // If fireRate is high enough (more than 1), queue up additional shots
        if (fireRate > 1f)
        {
            // Calculate how many additional shots we need based on the fractional part
            int additionalShots = Mathf.FloorToInt(fireRate);

            // Schedule additional shots at appropriate intervals
            for (int i = 1; i < additionalShots; i++)
            {
                StartCoroutine(AnimateElectricityAndFire(i * timeBetweenShots));
            }
        }

        // Wait for all shots to complete
        yield return new WaitForSeconds((Mathf.Ceil(fireRate) * timeBetweenShots) + electricitySpeed);
        isFiringSequenceActive = false;
    }

    private IEnumerator AnimateElectricityAndFire(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rope == null || rope.solver == null || rope.activeParticleCount == 0)
        {
            Fire(); // Fallback to regular fire if no rope
            yield break;
        }

        // Create electricity visual
        GameObject visual = ObjectPool.instance.GetObject(
            electricityVisualPrefab,
            true,
            shootingPosition.position,
            Quaternion.identity
        );
        activeElectricityVisuals.Add(visual);

        var solver = rope.solver;
        var solverIndices = rope.solverIndices;
        float t = 0f;

        // Animate the electricity along the rope
        while (t < 1f)
        {
            t += Time.deltaTime / electricitySpeed;

            float fIndex = Mathf.Lerp(0, rope.activeParticleCount - 1, t);
            int iIndex = Mathf.FloorToInt(fIndex);
            int nextIndex = Mathf.Min(iIndex + 1, rope.activeParticleCount - 1);

            int solverIndexA = solverIndices[iIndex];
            int solverIndexB = solverIndices[nextIndex];

            // Convert solver-local to world-space positions
            Vector3 posA = solver.transform.TransformPoint(solver.positions[solverIndexA]);
            Vector3 posB = solver.transform.TransformPoint(solver.positions[solverIndexB]);

            float lerpT = fIndex - iIndex;
            visual.transform.position = Vector3.Lerp(posA, posB, lerpT);

            yield return null;
        }

        // Clean up the visual effect
        ObjectPool.instance.ReturnToPool(visual, 0f, () =>
        {
            var trail = visual.GetComponent<TrailRenderer>();
            if (trail != null) trail.Clear();
        });
        activeElectricityVisuals.Remove(visual);

        // Fire the actual projectile
        Fire();
    }
}
