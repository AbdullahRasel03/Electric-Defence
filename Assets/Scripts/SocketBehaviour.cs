using UnityEngine;

public class SocketBehaviour : MonoBehaviour
{
    [Header("Dragging Settings")]
    public float snapSpeed = 20f;

    [Header("References")]
    [SerializeField] private MeshRenderer socketMesh;
    public LayerMask powerPointLayer;

    public bool isDragging = false;
    public bool JustReleased { get; private set; }

    private Vector3 offset;
    private float fixedYPosition;

    private Material socketMaterial;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool shouldSnap;

    private PowerPoint connectedPowerPoint;
    private PowerPoint ownPowerPoint;
    private SocketManager socketManager;

    void Start()
    {
        fixedYPosition = transform.position.y;

        // Cache MeshRenderer material
        if (socketMesh != null)
        {
            socketMaterial = socketMesh.material;
        }
        else
        {
            Debug.LogWarning("Socket MeshRenderer not assigned!", this);
        }

        // Cache own PowerPoint if present
        ownPowerPoint = GetComponent<PowerPoint>();
        if (ownPowerPoint == null)
        {
            Debug.LogWarning("Socket does not have PowerPoint component!", this);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            transform.position = new Vector3(mousePosition.x - offset.x, fixedYPosition, mousePosition.z - offset.z);
        }

        if (shouldSnap)
        {
            SmoothSnap();
        }
    }

    void OnMouseDown()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = mousePosition - transform.position;
        isDragging = true;
        JustReleased = false;

        // Disconnect from current PowerPoint if any
        if (connectedPowerPoint != null)
        {
            connectedPowerPoint.DisconnectPlug(); // Plug or socket should be handled inside PowerPoint
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        JustReleased = true;

        // You may implement connection logic here or keep it externally controlled
    }

    private void SmoothSnap()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, snapSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, snapSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            shouldSnap = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void SetSocketManager(SocketManager manager)
    {
        socketManager = manager;
    }

    public void ReturnToPool()
    {
        if (socketManager != null)
        {
            socketManager.ReturnSocketToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConnectToPowerPoint(PowerPoint point, Vector3 pos, Quaternion rot, Transform parent, PowerSourceState powerSourceState, float sourceMultiplier)
    {
        if (ownPowerPoint == null)
        {
            Debug.LogWarning("Cannot connect: this Socket does not have a PowerPoint component!", this);
            return;
        }

        connectedPowerPoint = point;
        transform.SetParent(parent);
        targetPosition = pos;
        targetRotation = rot;
        shouldSnap = true;
        JustReleased = false;

        ownPowerPoint.powerState = powerSourceState;

        if (powerSourceState == PowerSourceState.Active)
        {
            if (ownPowerPoint.connectedPlug != null && ownPowerPoint.connectedPlug.connectedTurret != null)
            {
                ownPowerPoint.connectedPlug.connectedTurret.InititateTurret();
                ownPowerPoint.connectedPlug.connectedTurret.UpdateFireRate(sourceMultiplier + ownPowerPoint.sourceMultiplier);
            }
            else
            {
                Debug.LogWarning("PowerPoint or connectedTurret is null during activation!", this);
               
            }

            ownPowerPoint.SetColorActive(); // Optional: method to update visuals
        }
        else
        {
            if (ownPowerPoint.connectedPlug != null && ownPowerPoint.connectedPlug.connectedTurret != null)
            {
                ownPowerPoint.connectedPlug.connectedTurret.DeactivateTurret();
                ownPowerPoint.connectedPlug.connectedTurret.ResetFireRate();
            }
            else
            {
                Debug.LogWarning("PowerPoint or connectedTurret is null during activation!", this);
            }

            ownPowerPoint.SetColorDeactivated(); // Optional: method to update visuals

        }
    }

    public void DisconnectFromPowerPoint()
    {
        connectedPowerPoint = null;
        shouldSnap = false;
    }
}
