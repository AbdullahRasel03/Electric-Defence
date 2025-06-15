using UnityEngine;

public class PowerPoint : MonoBehaviour
{
    [Header("Settings")]
    public Transform plugPoint;
    public LayerMask plugLayer;
    public float connectionThreshold = 0.05f;
    public Color connectedColor = Color.green;
    public Color disconnectedColor = Color.gray;

    [Header("References")]
    [SerializeField] private MeshRenderer socketMesh;

    [Header("State")]
    public PlugBehaviour connectedPlug;
    public bool HasConnection => connectedPlug != null;

    private Material socketMaterial;
    private Color targetColor;
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
            if (plug != null && !plug.IsBeingDragged && plug.JustReleased)
            {
                ConnectPlug(plug);
            }
        }
    }

    private void ConnectPlug(PlugBehaviour plug)
    {
        connectedPlug = plug;
        plug.ConnectToSocket(this, plugPoint.position, plugPoint.rotation);
        targetColor = connectedColor; // Change to connected color
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
}