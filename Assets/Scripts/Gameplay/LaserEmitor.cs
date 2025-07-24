using MasterFX;
using UnityEngine;

public class LaserEmitor : MonoBehaviour
{
    public LayerMask reflectableLayers;
    public MLaser laser;
    public float maxDistance = 100f;

    void Update()
    {
        EmitLaser();
    }

    void EmitLaser()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, reflectableLayers))
        {
            laser.SetLaser(origin, hit.point);

            LaserReflector reflector = hit.collider.GetComponent<LaserReflector>();
            if (reflector != null)
            {
                reflector.Reflect(hit.point);
            }
        }
        else
        {
            Vector3 endPoint = origin + direction * maxDistance;
            laser.SetLaser(origin, endPoint);
        }
    }
}
