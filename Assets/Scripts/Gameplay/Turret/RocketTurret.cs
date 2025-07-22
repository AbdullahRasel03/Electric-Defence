using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocketTurret : Turret
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem fireMuzzle;
    [SerializeField] private ParticleSystem fireImpact;
    [SerializeField] private GameObject fireProjectilePrefab;
    [SerializeField] private float explosionDamageRadius = 5f;
    private float fireTime = 0f;

    protected override void Fire()
    {
        base.Fire();
        if (currentTarget != null)
        {
            fireTime += Time.deltaTime;
            if (fireTime < fireDelay) return;

            fireTime = 0f;

            turretBody.transform.DOLocalMoveZ(-0.25f, 0.15f).OnComplete(() =>
            {
                turretBody.transform.DOLocalMoveZ(0f, 0.1f);
            });

            fireMuzzle.Play();

            // Create a fire projectile towards the target
            GameObject projectile = ObjectPool.instance.GetObject(fireProjectilePrefab, true, shootPoint.position, Quaternion.identity);
            projectile.transform.LookAt(currentTarget.transform);
            projectile.GetComponent<Projectile>().Initialize(currentTarget, fireImpact);
            projectile.GetComponent<RocketProjectile>().SetExplosionRadius(explosionDamageRadius);
        }
    }
}
