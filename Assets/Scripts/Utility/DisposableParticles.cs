using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableParticles : MonoBehaviour
{
    private ParticleSystem particle;

    void OnEnable()
    {
        if(particle == null)
        {
            particle = GetComponent<ParticleSystem>();
        }
        particle.Play();

        StartCoroutine(WaitToDestroy());
    }

    void OnDisable()
    {
        particle.Stop();
    }



    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(particle.main.duration);
        this.transform.SetParent(null);
        ObjectPool.instance.ReturnToPool(this.gameObject);
    }
}
