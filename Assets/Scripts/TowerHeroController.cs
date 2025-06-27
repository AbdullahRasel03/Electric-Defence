using Obi;
using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TowerHeroController : MonoBehaviour
{

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

    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
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

        animator.SetFloat("FireSpeed", fireRate); // Adjust speed to match fire rate
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

    bool isFiring;
    public void Fire()
    {
        if (bulletPrefab == null || currentTarget == null) return;

        isFiring = true;
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

        Vector3 targetPosition = currentTarget.transform.position;
        targetPosition.y = shootPoint.position.y; // Keep bullet at shoot point height if needed

        float travelTime = Vector3.Distance(shootPoint.position, targetPosition) / speed;

        bullet.transform.DOJump(currentTarget.transform.position, 2, 1, travelTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            isFiring = false;
            Destroy(bullet);
        });
    }

    public void PlayFireAnimation()
    {
        animator.SetTrigger("Fire");
    }
}
