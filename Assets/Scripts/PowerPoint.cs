using UnityEngine;

public enum PowerSourceState
{
    Ianctive,
    Active
}
public class PowerPoint : MonoBehaviour
{
    public PowerSourceState powerState;
    public float sourceMultiplier = 1;
    [Header("Settings")]
    public Transform plugPoint;
    public LayerMask plugLayer;
    public LayerMask socketLayer;
    public float connectionThreshold = 0.05f;
    public Color connectedColor = Color.green;
    public Color disconnectedColor = Color.gray;

    [Header("References")]
    [SerializeField] private MeshRenderer socketMesh;

    [Header("State")]
    public PlugBehaviour connectedPlug;
    public SocketBehaviour connectedSocket;
    public bool HasConnection => connectedPlug != null;

    private Material socketMaterial;
    public Color targetColor;
    private float colorChangeSpeed = 5f;

    
    private void Awake()
    {
        // Initialize socket material
        if (socketMesh != null)
        {
            socketMaterial = socketMesh.material;
            socketMaterial.color = disconnectedColor;
            targetColor = disconnectedColor;
        }
        else
        {
            Debug.LogWarning("Socket MeshRenderer not assigned!", this);
        }
        if (powerState == PowerSourceState.Active)
        {
            targetColor = connectedColor;
        }
    }

    private void Update()
    {
        // Smooth color transition
        if (socketMaterial != null && socketMaterial.color != targetColor)
        {
            socketMaterial.color = Color.Lerp(
                socketMaterial.color,
                targetColor,
                colorChangeSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!HasConnection && IsPlug(other.gameObject))
        {
            PlugBehaviour plug = other.GetComponent<PlugBehaviour>();
            if (plug != null && !plug.IsBeingDragged && plug.JustReleased && !Input.GetMouseButton(0))
            {
                ConnectPlug(plug);
            }
        }
        else if (!HasConnection && IsSocket(other.gameObject))
        {
            SocketBehaviour socket = other.GetComponent<SocketBehaviour>();
            if (socket != null && !socket.isDragging && socket.JustReleased)
            {
                ConnectSocket(socket);
                print("Is A Socket");
            }
        }
    }

    private void ConnectPlug(PlugBehaviour plug)
    {
        connectedPlug = plug;
        plug.ConnectToSocket(this, plugPoint.position, plugPoint.rotation, plugPoint, powerState, sourceMultiplier);
        if (powerState == PowerSourceState.Active)
        {

            targetColor = connectedColor; // Change to connected color
        }
    }

    private void ConnectSocket(SocketBehaviour socket)
    {
        connectedSocket = socket;
        socket.ConnectToPowerPoint(this, plugPoint.position, plugPoint.rotation, plugPoint, powerState, sourceMultiplier);
        if (powerState == PowerSourceState.Active)
        {

            targetColor = connectedColor; // Change to connected color
        }
    }

    public void DisconnectPlug()
    {
        if (connectedPlug != null)
        {
            connectedPlug.DisconnectFromSocket();
            connectedPlug = null;
            targetColor = disconnectedColor; // Revert to disconnected color
        }
    }

    private bool IsPlug(GameObject obj)
    {
        return plugLayer == (plugLayer | (1 << obj.layer));
    }
    private bool IsSocket(GameObject obj)
    {
        return socketLayer == (socketLayer | (1 << obj.layer));
    }
    public void SetColorActive()
    {
        if (socketMaterial != null)
        {
            targetColor = connectedColor;
        }
    }

    public void SetColorDeactivated()
    {
        if (socketMaterial != null)
        {
            targetColor = disconnectedColor;
        }
    }

}