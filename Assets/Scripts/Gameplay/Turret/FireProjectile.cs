using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : Projectile
{
    
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Play impact effect
                GameObject impactObject = ObjectPool.instance.GetObject(impactEffect.gameObject);
                ParticleSystem impact = impactObject.GetComponent<ParticleSystem>();
                impact.transform.position = transform.position;
                impact.gameObject.SetActive(true);
                impact.Play();

                // Apply damage to the enemy
                enemy.TakeDamage(35); // Example damage value

                // Destroy the projectile after impact
                ObjectPool.instance.ReturnToPool(this.gameObject);
            }
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        
        if (transform.position.z >= 200f)
        {
            ObjectPool.instance.ReturnToPool(gameObject);
        }
    }
}
