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
    private bool isActive = false;
    public Plug plug;
    private void Update()
    {
        // Fire whenever Space is pressed, ignoring aim or target
        if (Input.GetKey(KeyCode.Space))
        {
            // If no current target, you might want to pass null or just shoot forward
            shooter.TryShoot(currentTarget);
        }
        if (!isActive) return;

        currentTarget = targetingSystem.GetCurrentTarget();

        // Rotate and shoot if there is a valid target and aimed
        if (currentTarget != null && currentTarget.IsActive)
        {
            RotateToTarget(currentTarget.transform.position);

            if (IsAimedAt(currentTarget.transform.position))
            {
                shooter.TryShoot(currentTarget);
            }
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

        // Keep target direction as is (no offset here)
        Vector3 targetDir = dir.normalized;

        // Remove rotation offset from turret forward (rotate it -offset)
        Quaternion inverseOffset = Quaternion.Euler(0f, -rotationOffsetY, 0f);
        Vector3 correctedTurretForward = inverseOffset * turretHead.forward;

        float angle = Vector3.Angle(correctedTurretForward, targetDir);

        Debug.Log($"Aim Debug: Current Angle = {angle:F2}°, Aim Threshold = {aimThreshold}°, Aimed = {angle <= aimThreshold}");

        return angle <= aimThreshold;
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

    private void OnDrawGizmos()
    {
        if (turretHead == null) return;

        Gizmos.color = Color.yellow;

        Vector3 origin = turretHead.position;
        Vector3 forward = turretHead.forward * 2f;
        Gizmos.DrawRay(origin, forward);

        if (currentTarget != null)
        {
            Vector3 targetDir = currentTarget.transform.position - origin;
            targetDir.y = 0;

            Quaternion inverseOffset = Quaternion.Euler(0f, -rotationOffsetY, 0f);
            Vector3 correctedDir = inverseOffset * targetDir;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, correctedDir.normalized * 2f);

            float angle = Vector3.Angle(turretHead.forward, correctedDir);
            UnityEditor.Handles.Label(origin + Vector3.up * 1.5f, $"Angle: {angle:F2}");
        }
    }

}
