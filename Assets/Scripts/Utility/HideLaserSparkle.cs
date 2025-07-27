using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLaserSparkle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError(other.gameObject);
        if (other.gameObject.CompareTag("Socket"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
