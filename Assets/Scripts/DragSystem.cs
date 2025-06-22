using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragSystem : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 15f;
    public float snapSpeed = 20f;
    public bool lockYAxis = true;
    public float fixedYPosition = 0f;

    public bool IsBeingDragged { get; private set; }
    public bool JustReleased;

    private Vector3 dragOffset;
    private Camera mainCamera;
    private PowerNode powerNode;

    private void Awake()
    {
        mainCamera = Camera.main;
        powerNode = GetComponent<PowerNode>();
    }

    private void OnMouseDown()
    {
        StartDrag();
    }

    private void OnMouseUp()
    {
        EndDrag();
    }

    public void StartDrag()
    {
        IsBeingDragged = true;
        JustReleased = false;

        Vector3 mousePosition = GetMouseWorldPosition();
        dragOffset = mousePosition - transform.position;

        // Disconnect all when starting to drag (except fixed nodes)
        if (powerNode != null && !(powerNode is PowerSource))
        {
            powerNode.DisconnectAll();
        }
    }

    public void EndDrag()
    {
        IsBeingDragged = false;
        JustReleased = true;

        // Check for nearby connections
        if (powerNode != null)
        {
            CheckForConnections();
        }
    }

    private void Update()
    {
        if (IsBeingDragged)
        {
            Vector3 targetPosition = GetMouseWorldPosition() - dragOffset;
            if (lockYAxis)
            {
                targetPosition.y = fixedYPosition;
            }
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
        }
    }

    private void CheckForConnections()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, powerNode.connectionRadius, powerNode.connectableLayers);
        foreach (var hitCollider in hitColliders)
        {
            
           // Debug.Break();
            PowerNode otherNode = hitCollider.GetComponent<PowerNode>();
            if (otherNode != null && powerNode.CanConnectWith(otherNode))
            {
                powerNode.ConnectTo(otherNode);
                print(otherNode.gameObject.name);
                break; // Connect to the first valid node found
            }


        }

       // print(hitColliders.Length);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}