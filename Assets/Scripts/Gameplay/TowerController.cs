using UnityEngine;

[RequireComponent(typeof(TowerTargetingSystem))]
public class TowerController : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private Transform turretHead;
    [SerializeField] public TowerHeroController shooter;
    [SerializeField] private TowerTargetingSystem targetingSystem;

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float aimThreshold = 5f;

    private Enemy currentTarget;
    public bool isActive = true;
    public Plug plug;
    private bool manualFireInput; // Track manual firing state

    public GridObject[] gridsOnPath;

    private void Start()
    {
        ActivateTower();
    }
    private void Update()
    {
        manualFireInput = Input.GetKey(KeyCode.Space);

        if (!isActive) return;

        currentTarget = targetingSystem.GetCurrentTarget();

        // Only handle automatic firing if not manually firing
        if (!manualFireInput && currentTarget != null && currentTarget.IsActive)
        {
            RotateToTarget(currentTarget.transform.position);

            if (IsAimedAt(currentTarget.transform.position))
            {
                shooter.TryShoot(currentTarget);
            }
        }
        // Handle manual firing
        else if (manualFireInput)
        {
            shooter.TryShoot(currentTarget);
        }
    }

    [SerializeField] private float rotationOffsetY = 45f;

    private void RotateToTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - turretHead.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            targetRot *= Quaternion.Euler(0f, rotationOffsetY, 0f);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    private bool IsAimedAt(Vector3 targetPos)
    {
        Vector3 dir = targetPos - turretHead.position;
        dir.y = 0;
        Vector3 targetDir = dir.normalized;
        Quaternion inverseOffset = Quaternion.Euler(0f, -rotationOffsetY, 0f);
        Vector3 correctedTurretForward = inverseOffset * turretHead.forward;
        return Vector3.Angle(correctedTurretForward, targetDir) <= aimThreshold;
    }

    public void ActivateTower()
    {
        isActive = true;
        shooter.enabled = true;
    }

    public void DeactivateTower()
    {
        isActive = false;
        shooter.enabled = false;
    }

    public void CheckMultisOnPath()
    {
        float fireRate = 1;
        foreach (var item in gridsOnPath)
        {
            if (item.socket)
            {
                fireRate += item.socket.ownMultiplier;
                item.socket.PowerUp();
            }
        }
        shooter.SetFireRate(fireRate);
    }
}