using MasterFX;
using UnityEngine;

public class LaserReflector : MonoBehaviour
{
    public MLaser laser;
    public Transform reflectDirection;
    public bool useCustomWorldDirection = false;
    public Vector3 direction1;
    public Vector3 direction2;

    public float multiplier = 1f;
    public float maxDistance = 15f;
    public int maxReflectionCount = 5;
    public float castOffset = 0.01f;

    private bool isHitThisFrame = false;

    [SerializeField] LayerMask reflectionLayer;
    [SerializeField] LayerMask towerLayer;
    [SerializeField] LayerMask socketLayer;
    Socket socket;

    private void Start()
    {
        socket = GetComponent<Socket>();
    }
    public void Reflect(Vector3 hitPoint, int depth = 0, float accumulatedMultiplier = 0f)
    {
        if (laser == null || depth > maxReflectionCount)
            return;

        isHitThisFrame = true;
        socket.PowerUp();
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
            chosenDir = transform.forward;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 endPoint = origin + chosenDir * maxDistance;

        Ray ray = new Ray(origin + chosenDir * castOffset, chosenDir);

        float total = accumulatedMultiplier + multiplier;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, reflectionLayer))
        {
            endPoint = hit.point + (chosenDir.normalized * 1f);

            LaserReflector nextReflector = hit.collider.GetComponent<LaserReflector>();
            if (nextReflector != null && nextReflector != this)
            {
                nextReflector.Reflect(hit.point, depth + 1, total);
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit hit2, maxDistance, towerLayer))
        {
            endPoint = hit2.point;
            Turret tower = hit2.collider.transform.parent.GetComponentInChildren<Turret>();
            if (tower != null)
            {
                tower.ReceivePower(total); // 👈 use ReceivePower instead of ActivateTower
            }
        }

        laser.SetLaser(origin, endPoint);
        laser.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (!isHitThisFrame && laser != null)
        {
            laser.gameObject.SetActive(false);
            socket.PowerDown();
        }

        isHitThisFrame = false;
    }
}
