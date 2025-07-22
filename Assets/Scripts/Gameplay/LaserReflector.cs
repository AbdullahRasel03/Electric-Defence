using MasterFX;
using UnityEngine;

public class LaserReflector : MonoBehaviour
{
    public MLaser laser;
    public Transform reflectDirection;
    public bool useCustomWorldDirection = false;
    public Vector3 direction1;
    public Vector3 direction2;

    public float maxDistance = 15f;
    public int maxReflectionCount = 5;
    public float castOffset = 0.01f;

    private bool isHitThisFrame = false;
    [SerializeField] LayerMask reflectionLayer;

    [SerializeField] LayerMask towerLayer;
    public void Reflect(Vector3 hitPoint, int depth = 0)
    {
        if (laser == null || depth > maxReflectionCount)
            return;

        Vector3 incomingDir = (hitPoint - transform.position).normalized;
        Vector3 chosenDir;

        if (Vector3.Dot(incomingDir, direction1.normalized) > 0.5f)
        {
            chosenDir = direction2.normalized;
        }
        else if (Vector3.Dot(incomingDir, direction2.normalized) > 0.5f)
        {
            chosenDir = direction1.normalized;
        }
        else
        {
            // Fallback direction if incoming direction is ambiguous
            chosenDir = transform.forward;
        }

        Vector3 origin = transform.position;
        Vector3 endPoint = origin + chosenDir * maxDistance;

        Ray ray = new Ray(origin + chosenDir * castOffset, chosenDir);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, reflectionLayer))
        {
            endPoint = hit.point;

            LaserReflector nextReflector = hit.collider.GetComponent<LaserReflector>();
            if (nextReflector != null && nextReflector != this)
            {
                nextReflector.Reflect(hit.point, depth + 1);
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit hit2, maxDistance, towerLayer))
        {
            endPoint = hit2.point;

            if (hit2.collider != null)
            {
                hit2.collider.gameObject.GetComponent<TowerController>().ActivateTower();
            }
        }

            laser.SetLaser(origin, endPoint);
        laser.gameObject.SetActive(true);
        isHitThisFrame = true;
    }


    void LateUpdate()
    {
        if (!isHitThisFrame && laser != null)
        {
            laser.gameObject.SetActive(false);
        }

        isHitThisFrame = false;
    }
}
