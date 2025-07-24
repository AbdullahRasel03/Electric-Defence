using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    private Camera cam;
    public bool canLookContanstly = true;

    void Start()
    {
        cam = Camera.main;
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
    void LateUpdate()
    {
        if (!canLookContanstly) return;

        Vector3 direction = (transform.position - cam.transform.position).normalized;
        direction.x = 0; // Keep the Y axis fixed
        transform.rotation = Quaternion.LookRotation(direction);

        
    }
}
