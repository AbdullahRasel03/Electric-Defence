using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float _damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}