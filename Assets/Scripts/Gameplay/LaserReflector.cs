using MasterFX;
using UnityEngine;

public class LaserReflector : MonoBehaviour
{
    [SerializeField] int glowMatIndex;
    public MLaser laser;
    public Transform reflectDirection;
    public bool useCustomWorldDirection = false;
    public Vector3 direction1;
    public Vector3 direction2;

    public bool isSplitter = false;
    public MLaser splitLaserLeft;
    public MLaser splitLaserRight;

    public float maxDistance = 15f;
    public int maxReflectionCount = 5;
    public float castOffset = 0.01f;

    private bool isHitThisFrame = false;

    [SerializeField] LayerMask reflectionLayer;
    [SerializeField] LayerMask towerLayer;
    [SerializeField] LayerMask socketLayer;
    [SerializeField] LayerMask stopLayer; // ✅ NEW

    Socket socket;

    private void Start()
    {
        socket = GetComponent<Socket>();
    }

    public void Reflect(Vector3 hitPoint, int depth = 0, float accumulatedMultiplier = 0)
    {
        if (laser == null || depth > maxReflectionCount)
            return;

        isHitThisFrame = true;

        float total = accumulatedMultiplier;

        if (socket != null)
        {
            socket.PowerUp(glowMatIndex);
            total -= socket.ownMultiplier / 50;
        }

        if (isSplitter)
        {
            CastLaserDirection(direction1.normalized, hitPoint, depth, total, splitLaserLeft);
            CastLaserDirection(-direction1.normalized, hitPoint, depth, total, splitLaserRight);
        }
        else
        {
            Vector3 incomingDir = (hitPoint - transform.position).normalized;
            Vector3 chosenDir;

            if (Vector3.Dot(incomingDir, direction1.normalized) > 0.5f)
                chosenDir = direction2.normalized;
            else if (Vector3.Dot(incomingDir, direction2.normalized) > 0.5f)
                chosenDir = direction1.normalized;
            else
                chosenDir = transform.forward;

            CastLaserDirection(chosenDir, hitPoint, depth, total, laser);
        }
    }

    private void CastLaserDirection(Vector3 dir, Vector3 startPoint, int depth, float multiplier, MLaser laserRef)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 currentOrigin = origin;
        Vector3 finalEndPoint = origin + dir * maxDistance;
        float remainingDistance = maxDistance;

        int combinedMask = reflectionLayer | towerLayer | socketLayer | stopLayer;

        while (remainingDistance > 0f)
        {
            Ray ray = new Ray(currentOrigin + dir * castOffset, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, remainingDistance, combinedMask))
            {
                int hitLayer = hit.collider.gameObject.layer;

                if (((1 << hitLayer) & socketLayer) != 0)
                {
                    Socket socketHit = hit.collider.GetComponent<Socket>();
                    if (socketHit != null)
                    {
                        socketHit.PowerUp();
                        multiplier -= socketHit.ownMultiplier / 50;
                    }

                    float distUsed = Vector3.Distance(currentOrigin, hit.point) + 0.01f;
                    currentOrigin = hit.point + dir * 0.01f;
                    remainingDistance -= distUsed;
                    continue;
                }

                if (((1 << hitLayer) & reflectionLayer) != 0)
                {
                    finalEndPoint = hit.point + dir.normalized * 1f;

                    LaserReflector nextReflector = hit.collider.GetComponent<LaserReflector>();
                    if (nextReflector != null && nextReflector != this)
                    {
                        nextReflector.Reflect(hit.point, depth + 1, multiplier);
                    }
                    break;
                }

                if (((1 << hitLayer) & towerLayer) != 0)
                {
                    finalEndPoint = hit.point;

                    Turret turret = hit.collider.transform.parent.GetComponentInChildren<Turret>();
                    if (turret != null)
                    {
                        turret.ReceivePower(multiplier);
                    }
                    break;
                }

                // ✅ Stop layer check
                if (((1 << hitLayer) & stopLayer) != 0)
                {
                    finalEndPoint = hit.point;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        if (laserRef != null)
        {
            laserRef.SetLaser(origin, finalEndPoint);
            laserRef.gameObject.SetActive(true);
        }
    }

    void LateUpdate()
    {
        if (!isHitThisFrame)
        {
            if (laser != null) laser.gameObject.SetActive(false);
            if (splitLaserLeft != null) splitLaserLeft.gameObject.SetActive(false);
            if (splitLaserRight != null) splitLaserRight.gameObject.SetActive(false);

            if (socket != null)
                socket.PowerDown(glowMatIndex);
        }

        isHitThisFrame = false;
    }
}
