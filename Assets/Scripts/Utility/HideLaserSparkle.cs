using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLaserSparkle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
    }
}
