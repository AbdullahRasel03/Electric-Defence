using MasterFX;
using UnityEngine;

public class LaserEmitor : MonoBehaviour
{
    public LayerMask reflectableLayers, towerLayer, socketLayer;
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
        Vector3 currentOrigin = origin;
        Vector3 finalEndPoint = origin + direction * maxDistance;
        float remainingDistance = maxDistance;
        float totalMultiplier = 1f;

        int combinedLayerMask = reflectableLayers | towerLayer | socketLayer;

        while (remainingDistance > 0f)
        {
            Ray ray = new Ray(currentOrigin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, remainingDistance, combinedLayerMask))
            {
                int hitLayer = hit.collider.gameObject.layer;

                if (((1 << hitLayer) & socketLayer) != 0)
                {
                    Socket socketTarget = hit.collider.GetComponent<Socket>();
                    if (socketTarget != null)
                    {
                        socketTarget.PowerUp();
                        totalMultiplier -= socketTarget.ownMultiplier/50; // 👈 accumulate
                    }

                    float distanceUsed = Vector3.Distance(currentOrigin, hit.point) + 0.01f;
                    currentOrigin = hit.point + direction * 0.01f;
                    remainingDistance -= distanceUsed;
                    continue;
                }

                if (((1 << hitLayer) & reflectableLayers) != 0)
                {
                    finalEndPoint = hit.point;
                    laser.SetLaser(origin, finalEndPoint);

                    LaserReflector reflector = hit.collider.GetComponent<LaserReflector>();
                    if (reflector != null)
                    {
                        reflector.Reflect(hit.point, 0, totalMultiplier); // 👈 pass accumulated multiplier
                    }
                    return;
                }

                if (((1 << hitLayer) & towerLayer) != 0)
                {
                    finalEndPoint = hit.point;
                    laser.SetLaser(origin, finalEndPoint);

                    Turret turret = hit.collider.transform.parent.GetComponentInChildren<Turret>();
                    if (turret != null)
                    {
                        turret.ReceivePower(totalMultiplier); // 👈 use total multiplier
                    }
                    return;
                }
            }
            else
            {
                break;
            }
        }

        laser.SetLaser(origin, finalEndPoint);
    }

}
