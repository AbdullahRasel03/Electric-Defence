using UnityEngine;

public class TowerHeroController : MonoBehaviour
{
    [SerializeField] private float fireRate = 1f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Animator animator;
    private float fireCooldown;

    public void TryShoot(Enemy target, Transform firePoint)
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            Fire(target, firePoint);
        }
    }

    public void SetFireRate(float newRate)
    {
        fireRate = newRate;
        fireCooldown = 0f;
    }

    public void Fire(Enemy target, Transform firePoint)
    {
        if (bulletPrefab == null || target == null) return;
        animator.SetTrigger("Fire");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            Vector3 dir = (target.transform.position - firePoint.position).normalized;
            rb.velocity = dir * speed;
        }
    }
}
