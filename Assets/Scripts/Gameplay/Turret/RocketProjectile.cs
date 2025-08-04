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

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            OnExplosionRequired();
        }
    }

    void Update()
    {
        // if (target == null)
        // {
        //     ObjectPool.instance.ReturnToPool(gameObject);
        //     return;
        // }
        //
        // else if (!target.IsActive)
        // {
        //     ObjectPool.instance.ReturnToPool(gameObject);
        //     return;
        // }

        timeElapsed += Time.deltaTime;

     
        float zigzag = Mathf.Sin(timeElapsed * frequency) * amplitude;
        
        Vector3 forwardMove = transform.forward * speed * Time.deltaTime;
        
        Vector3 offset = new Vector3(zigzag, 0f, 0f);
        
        transform.position += forwardMove + transform.TransformDirection(offset - new Vector3(0f, 0f, 0f));

        // // Optional: Explosion or lifetime-based destruction
        // lifeTime -= Time.deltaTime;
        // if (lifeTime <= 0f)
        // {
        //     OnExplosionRequired(); // Your explosion method
        //     ObjectPool.instance.ReturnToPool(gameObject); // Or Destroy(gameObject);
        // }

        if (transform.position.z >= 200f)
        {
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
                enemy.TakeDamage(80); // Example damage value
            }
        }

        GameObject impactObject = ObjectPool.instance.GetObject(impactEffect.gameObject, true, transform.position, Quaternion.identity);
        ParticleSystem impact = impactObject.GetComponent<ParticleSystem>();
        impact.Play();

        Camera.main.transform.DOShakePosition(0.5f, 0.3f, 10, 0.5f);
        Camera.main.transform.DOPunchRotation(Vector3.forward * 0.35f, 0.5f, 10, 1f);
        
        ObjectPool.instance.ReturnToPool(gameObject);
    }
    

}
