using MasterFX;
using UnityEngine;

public class LaserEmitor : MonoBehaviour
{
    public LayerMask reflectableLayers, towerLayer;
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
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, reflectableLayers))
        {
            laser.SetLaser(origin, hit.collider.transform.position);

            LaserReflector reflector = hit.collider.GetComponent<LaserReflector>();
            if (reflector != null)
            {
                // 👇 Pass depth and accumulatedMultiplier
                reflector.Reflect(hit.point, 0, 0f);
            }
        }
       else if (Physics.Raycast(ray, out RaycastHit hit2, maxDistance, towerLayer))
        {
            laser.SetLaser(origin, hit2.point);

            Turret turret = hit2.collider.transform.parent.GetComponentInChildren<Turret>();
            if (turret != null)
            {
              turret.ReceivePower(1);
            }
        }
        else
        {
            Vector3 endPoint = origin + direction * maxDistance;
            laser.SetLaser(origin, endPoint);
        }
    }
}
