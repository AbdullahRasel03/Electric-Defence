using UnityEngine;

public class TurretTargeting : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float targetRefreshRate = 0.5f;

    private Transform _currentTarget;
    private float _targetSearchCooldown;

    public Transform CurrentTarget => _currentTarget;

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

        SearchForTargets();
        _targetSearchCooldown = targetRefreshRate;
    }

    private void SearchForTargets()
    {
        Collider[] potentialTargets = Physics.OverlapSphere(transform.position, 50f, targetMask);

        if (potentialTargets.Length > 0)
        {
            // Simple implementation - just pick the first target
            // In a real game you might want to find the closest or most dangerous target
            _currentTarget = potentialTargets[0].transform;
        }
        else
        {
            _currentTarget = null;
        }
    }

    public bool HasActiveTarget(out Transform target)
    {
        target = _currentTarget;
        return target != null;
    }
}