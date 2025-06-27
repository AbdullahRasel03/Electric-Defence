using Obi;
using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TowerHeroController : MonoBehaviour
{
    public bool isFiring;
    [SerializeField] private TowerController towerController;
    [SerializeField] private float fireRate = 1f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Animator animator;


    [SerializeField] private ObiRope rope;
    [SerializeField] private GameObject electricityVisualPrefab;
    [SerializeField] private float electricitySpeed = 0.3f;
    [SerializeField] private Transform shootPoint;

    private float fireCooldown;
    private bool isFiringSequenceActive;

    private Enemy currentTarget;

    private void Start()
    {
        towerController = GetComponentInParent<TowerController>();

        // Pre-pool 20 bullets
        for (int i = 0; i < 20; i++)
        {
            GameObject pooledBullet = ObjectPool.instance.GetObject(bulletPrefab, false);
            ObjectPool.instance.ReturnToPool(pooledBullet);
        }
    }

    public void TryShoot(Enemy target)
    {
        currentTarget = target;
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f && !isFiringSequenceActive)
        {
            fireCooldown = 1f / fireRate;
            StartCoroutine(FireWithElectricity());
        }
    }

    public void SetFireRate(float newRate)
    {
        fireRate = Mathf.Max(newRate, 0.01f);
        fireCooldown = 0f;

        animator.SetFloat("FireSpeed", Mathf.Max(1f, fireRate));

    }

    private IEnumerator FireWithElectricity()
    {
        isFiringSequenceActive = true;
     
        if (rope == null || rope.solver == null || rope.activeParticleCount == 0 || electricityVisualPrefab == null)
        {
            PlayFireAnimation();
            isFiringSequenceActive = false;
            yield break;
        }

        float t = 0f;
        var visual = Instantiate(electricityVisualPrefab, towerController.plug.transform.position, Quaternion.identity);
        var solver = rope.solver;
        var indices = rope.solverIndices;

        while (t < 1f)
        {
            t += Time.deltaTime / electricitySpeed;

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

        PlayFireAnimation();

        isFiringSequenceActive = false;
    }

  
    public void Fire()
    {
        if (bulletPrefab == null || currentTarget == null) return;

        isFiring = true;

        GameObject bullet = ObjectPool.instance.GetObject(bulletPrefab, true, shootPoint.position, shootPoint.rotation);

        Vector3 targetPosition = currentTarget.transform.position;
        targetPosition.y = shootPoint.position.y;

        float distance = Vector3.Distance(shootPoint.position, targetPosition);
        float travelTime = distance / speed;

        // Calculate dynamic jump height (min 0.5, max 2)
        float minDistance = 1f;
        float maxDistance = 10f;
        float normalized = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float jumpHeight = Mathf.Lerp(0.5f, 2f, normalized);

        bullet.transform.DOJump(targetPosition, jumpHeight, 1, travelTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isFiring = false;
                ObjectPool.instance.ReturnToPool(bullet);
            });
    }

    public void PlayFireAnimation()
    {
        animator.Play("Attack1");
    }
}
