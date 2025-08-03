using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed = 10f;
    protected Enemy target;
    protected ParticleSystem impactEffect;
    public virtual void Initialize(Enemy target, ParticleSystem impactEffect)
    {
        this.target = target;
        this.impactEffect = impactEffect;
    }
}
