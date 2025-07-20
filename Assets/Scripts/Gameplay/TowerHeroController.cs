using Obi;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TowerHeroController : MonoBehaviour
{

    public bool isFiring;
    [SerializeField] private TowerController towerController;
    [SerializeField] private float fireRate = 0f;
    [SerializeField] private float damagePerShot = 10f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Animator animator;

    [SerializeField] private ObiRope rope;
    [SerializeField] private GameObject electricityVisualPrefab;
    [SerializeField] private float timeToMoveElectrcity = 0.3f;
    [SerializeField] private Transform shootPoint;

    private float fireCooldown;
    private bool isFiringSequenceActive;
    private Enemy currentTarget;
    private bool canFire = true; // New flag to prevent overlapping fires

    private void Start()
    {
        // Pre-pool 20 bullets
        for (int i = 0; i < 20; i++)
        {
            GameObject pooledBullet = ObjectPool.instance.GetObject(bulletPrefab, false);
            ObjectPool.instance.ReturnToPool(pooledBullet);
        }
    }

    public void TryShoot(Enemy target)
    {
        if (!canFire || isFiringSequenceActive) return;

        currentTarget = target;
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            StartCoroutine(FireSequence());
        }
    }

    private IEnumerator FireSequence()
    {
        canFire = false;
        isFiringSequenceActive = true;

        // Electricity effect if available
        if (rope != null && rope.solver != null && rope.activeParticleCount > 0 && electricityVisualPrefab != null)
        {
            yield return StartCoroutine(PlayElectricityEffect());
        }

        PlayFireAnimation();

        isFiringSequenceActive = false;
        canFire = true;
    }

    private IEnumerator PlayElectricityEffect()
    {
        float t = 0f;
        var visual = Instantiate(electricityVisualPrefab, towerController.plug.transform.position, Quaternion.identity);
        var solver = rope.solver;
        var indices = rope.solverIndices;

        while (t < 1f)
        {
            t += Time.deltaTime / timeToMoveElectrcity;
            float fIndex = Mathf.Lerp(0, rope.activeParticleCount - 1, t);
            int i = Mathf.FloorToInt(fIndex);
            int next = Mathf.Min(i + 1, rope.activeParticleCount - 1);

            int solverIndexA = indices[i];
            int solverIndexB = indices[next];

            Vector3 posA = solver.transform.TransformPoint(solver.positions[solverIndexA]);
            Vector3 posB = solver.transform.TransformPoint(solver.positions[solverIndexB]);
            visual.transform.position = Vector3.Lerp(posA, posB, fIndex - i);

            yield return null;
        }

        var trail = visual.GetComponent<TrailRenderer>();
        if (trail != null) trail.Clear();
        Destroy(visual);
    }

    public void Fire()
    {
        if (bulletPrefab == null || currentTarget == null || !currentTarget.IsActive) return;
        isFiring = true;
        GameObject bullet = ObjectPool.instance.GetObject(bulletPrefab, true, shootPoint.position, shootPoint.rotation);

        Vector3 targetPosition = currentTarget.transform.position;
        targetPosition.y = shootPoint.position.y;
        float distance = Vector3.Distance(shootPoint.position, targetPosition);
        float travelTime = distance / speed;

        // Calculate dynamic jump height
        float jumpHeight = Mathf.Lerp(0.5f, 2f,
            Mathf.InverseLerp(1f, 10f, distance));

        bullet.transform.DOJump(currentTarget.transform.position, jumpHeight, 1, travelTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (currentTarget != null && currentTarget.IsActive)
                {
                    currentTarget.TakeDamage(damagePerShot);
                }
                isFiring = false;
                ObjectPool.instance.ReturnToPool(bullet);
            });
    }

    public void PlayFireAnimation()
    {
        animator.Play("Attack1");
    }

    public void SetFireRate(float newRate)
    {
       // fireRate = Mathf.Max(newRate, 0.01f);
        fireRate = newRate;
        fireCooldown = 0f;
        animator.SetFloat("FireSpeed", Mathf.Max(1f, fireRate));
    }
}