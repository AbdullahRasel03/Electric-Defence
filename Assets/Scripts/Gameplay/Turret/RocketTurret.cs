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

    private RocketProjectile rocketProjectile;

    protected override void OnNewTargetSelected()
    {
        base.OnNewTargetSelected();
        
        rocketProjectile.ChangeTarget(currentTarget);
    }

    protected override void Fire()
    {
        base.Fire();
        if (currentTarget != null && currentTarget.IsActive)
        {
            fireTime += Time.deltaTime;
            SetFireRateSlider(fireTime);
            if (fireTime < fireRate) return;

            fireTime = 0f;

            Quaternion currentRotation = turretBody.transform.rotation;

            fireSequence = DOTween.Sequence();

            fireSequence.Append(turretBody.transform.DOLocalMoveZ(-0.35f, 0.15f))
            .Join(turretBody.transform.DORotateQuaternion(currentRotation * Quaternion.Euler(-10f, 0, 0), 0.15f))
            .AppendInterval(0.1f)
            .Append(turretBody.transform.DOLocalMoveZ(0f, 0.15f))
            .Join(turretBody.transform.DORotateQuaternion(currentRotation, 0.15f));

            AudioManager.CallPlaySFX(Sound.RocketTurretShot);

            fireMuzzle.Play();

            // Create a fire projectile towards the target
            GameObject projectile = ObjectPool.instance.GetObject(fireProjectilePrefab, true, shootPoint.position, Quaternion.identity);
            projectile.transform.LookAt(currentTarget.transform);
            projectile.GetComponent<Projectile>().Initialize(currentTarget, fireImpact);
            projectile.GetComponent<RocketProjectile>().SetExplosionRadius(explosionDamageRadius);
        }

        // else
        // {
        //     fireTime = 0f;
        //     SetFireRateSlider(0f);
        // }
    }
}
