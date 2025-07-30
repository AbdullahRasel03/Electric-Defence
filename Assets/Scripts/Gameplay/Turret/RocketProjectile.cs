using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RocketProjectile : Projectile
{
    [Header("Movement Settings")]
    public float frequency = 5f;        // How fast it oscillates
    public float amplitude = 1f;        // How wide the zigzag is
    public float hitThreshold = 0.5f;   // Distance to consider a hit

    private float timeElapsed = 0f;

    private float explosionRadius;

    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }

    void Update()
    {
        if (target == null)
        {
            ObjectPool.instance.ReturnToPool(gameObject);
            return;
        }

        else if (!target.IsActive)
        {
            ObjectPool.instance.ReturnToPool(gameObject);
            return;
        }

        timeElapsed += Time.deltaTime;

        Vector3 direction = (target.transform.position - transform.position).normalized;

        // Calculate right vector for perpendicular zigzag
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;


        float zigzag = Mathf.Sin(timeElapsed * frequency) * amplitude;
        Vector3 offset = perpendicular * zigzag;


        Vector3 targetPosition = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition + offset, speed * Time.deltaTime);


        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);

        if (Vector3.Distance(transform.position, target.transform.position) <= hitThreshold)
        {
            OnExplosionRequired();

            ObjectPool.instance.ReturnToPool(gameObject);
        }
    }

    private void OnExplosionRequired()
    {
        AudioManager.CallPlaySFX(Sound.RocketExplosion);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(40); // Example damage value
            }
        }

        GameObject impactObject = ObjectPool.instance.GetObject(impactEffect.gameObject, true, transform.position, Quaternion.identity);
        ParticleSystem impact = impactObject.GetComponent<ParticleSystem>();
        impact.Play();

        Camera.main.transform.DOShakePosition(0.5f, 0.3f, 10, 0.5f);
        Camera.main.transform.DOPunchRotation(Vector3.forward * 0.35f, 0.5f, 10, 1f);
    }
    

}
