using UnityEngine;
using DG.Tweening;

public class Socket : PowerNode
{
    [Header("DOTween Settings")]
    public float connectDuration = 0.5f;
    public Ease connectEase = Ease.OutBack;
    public float disconnectDuration = 0.3f;
    public Ease disconnectEase = Ease.InOutQuad;
    public float disconnectOffset = 0.2f;

    private Sequence connectSequence;
    private Rigidbody socketRb;
    private Transform initialParent; // Stores original parent

    private void Awake()
    {
        socketRb = GetComponent<Rigidbody>();
        nodeType = NodeType.Socket;
        initialParent = transform.parent; // Store initial parent
    }

    public override void ConnectTo(PowerNode other)
    {
        if (!CanConnectWith(other)) return;

        base.ConnectTo(other);

        if (other.nodeType == NodeType.PowerSource)
        {
            ConnectToAnchor(other.plugParent);
        }
        else if (other.nodeType == NodeType.Socket)
        {
            // For socket-to-socket connections
            ConnectToAnchor(other.plugParent);
        }
    }

    public void ConnectToAnchor(Transform anchor)
    {
        // Kill any existing tweens
        connectSequence?.Kill();

        // Disable physics during connection
        if (socketRb != null) socketRb.isKinematic = true;

        // Create smooth connection sequence
        connectSequence = DOTween.Sequence()
            .Append(transform.DOMove(anchor.position, connectDuration).SetEase(connectEase))
            .Join(transform.DORotateQuaternion(anchor.rotation, connectDuration).SetEase(connectEase))
            .OnComplete(() => {
                // Finalize connection
                transform.SetParent(anchor);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            });
    }

    public override void DisconnectFrom(PowerNode other)
    {
        if (other.nodeType == NodeType.Plug)
        {
            Plug plug = other as Plug;
            plug.DisconnectFromAnchor();
        }
        else if (other.nodeType == NodeType.PowerSource || other.nodeType == NodeType.Socket)
        {
            DisconnectFromAnchor();
        }

        base.DisconnectFrom(other);
    }

    public void DisconnectFromAnchor()
    {
        // Kill any existing tweens
        connectSequence?.Kill();

        // Calculate disconnect position
        Vector3 disconnectPos = transform.position + transform.forward * disconnectOffset;

         transform.SetParent(initialParent);
         if (socketRb != null) socketRb.isKinematic = false;
       
    }

    // Call this when drag is released without connecting
    public void ReturnToInitialParent()
    {
        // Kill any existing tweens
        connectSequence?.Kill();

        // Create return sequence
        connectSequence = DOTween.Sequence()
            .Append(transform.DOMove(initialParent.position, disconnectDuration).SetEase(disconnectEase))
            .Join(transform.DORotateQuaternion(initialParent.rotation, disconnectDuration).SetEase(disconnectEase))
            .OnComplete(() => {
                transform.SetParent(initialParent);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                if (socketRb != null)
                {
                    socketRb.isKinematic = false;
                }
            });
    }

    private void OnDestroy()
    {
        // Clean up tweens when destroyed
        connectSequence?.Kill();
    }
}