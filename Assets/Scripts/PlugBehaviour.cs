using UnityEngine;

public class PlugBehaviour : MonoBehaviour
{
    [Header("Settings")]
    public float dragSpeed = 15f;
    public float snapSpeed = 20f;
    public float selectionRadius = 0.5f;

    [Header("Visual Feedback")]
    public Color connectedRopeColor = Color.green;
    public Color disconnectedRopeColor = Color.gray;
    public float ropeColorChangeSpeed = 5f;

    [Header("References")]
    [SerializeField] private MeshRenderer ropeMesh;
    public PowerPoint ConnectedSocket { get; private set; }
    public TurretBehaviour connectedTurret;

    public bool IsBeingDragged { get; private set; }
    public bool JustReleased { get; private set; }

    private Vector3 dragOffset;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool shouldSnap;
    private Camera mainCamera;
    private Material ropeMaterial;
    private Color targetRopeColor;

    private void Awake()
    {
        mainCamera = Camera.main;
        GetComponent<Rigidbody>().isKinematic = true;

        // Initialize rope material
        if (ropeMesh != null)
        {
            ropeMaterial = ropeMesh.material;
            ropeMaterial.color = disconnectedRopeColor;
            targetRopeColor = disconnectedRopeColor;
        }
        else
        {
            Debug.LogWarning("Rope MeshRenderer not assigned!", this);
        }
    }

    private void Update()
    {
        HandleInput();

        if (shouldSnap)
        {
            SmoothSnap();
        }

        // Update rope color
        if (ropeMaterial != null && ropeMaterial.color != targetRopeColor)
        {
            ropeMaterial.color = Color.Lerp(
                ropeMaterial.color,
                targetRopeColor,
                ropeColorChangeSpeed * Time.deltaTime
            );
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }

        if (IsBeingDragged)
        {
            UpdateDragPosition();
        }
    }

    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                StartDrag();
            }
        }
    }

    private void StartDrag()
    {
        if (ConnectedSocket != null)
        {
            ConnectedSocket.DisconnectPlug();
        }

        IsBeingDragged = true;
        JustReleased = false;
        shouldSnap = false;
        dragOffset = transform.position - GetMouseWorldPosition();
        targetRopeColor = disconnectedRopeColor;
    }

    private void UpdateDragPosition()
    {
        Vector3 targetPos = GetMouseWorldPosition() + dragOffset;
        transform.position = targetPos;
    }

    private void EndDrag()
    {
        IsBeingDragged = false;
        JustReleased = true;
    }

    public void ConnectToSocket(PowerPoint socket, Vector3 position, Quaternion rotation)
    {
        ConnectedSocket = socket;
        targetPosition = position;
        targetRotation = rotation;
        shouldSnap = true;
        JustReleased = false;
        targetRopeColor = connectedRopeColor;

        if (connectedTurret != null)
        {
            connectedTurret.InititateTurret();
        }
    }

    public void DisconnectFromSocket()
    {
        ConnectedSocket = null;
        shouldSnap = false;
        targetRopeColor = disconnectedRopeColor;
        if (connectedTurret != null)
        {
            connectedTurret.DeactivateTurret();
        }
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
}