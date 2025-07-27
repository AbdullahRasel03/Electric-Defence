using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLaserSparkle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<ParticleSystem>().Stop();
    }

    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<ParticleSystem>().Play();
    }
}
