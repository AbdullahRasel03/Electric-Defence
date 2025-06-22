using UnityEngine;
using DG.Tweening;

public class Socket : PowerNode
{
    [Header("Socket-Specific Settings")]
    public float connectDuration = 0.5f;
    public Ease connectEase = Ease.OutBack;
    public float disconnectDuration = 0.3f;
    public Ease disconnectEase = Ease.InOutQuad;
    public float disconnectOffset = 0.2f;

    private Sequence connectSequence;
    private Rigidbody socketRb;
    private Transform initialParent;

    private void Awake()
    {
        socketRb = GetComponent<Rigidbody>();
        nodeType = NodeType.Socket;
        initialParent = transform.parent;
    }

    public override void ConnectTo(PowerNode other)
    {
        if (!CanConnectWith(other)) return;

        base.ConnectTo(other); // Handles the core connection logic

        if (other.nodeType == NodeType.PowerSource || other.nodeType == NodeType.Socket)
        {
            AnimateConnectionTo(other.plugParent);
        }
    }

    private void AnimateConnectionTo(Transform anchor)
    {
        connectSequence?.Kill();
        if (socketRb != null) socketRb.isKinematic = true;

        connectSequence = DOTween.Sequence()
            .Append(transform.DOMove(anchor.position, connectDuration).SetEase(connectEase))
            .Join(transform.DORotateQuaternion(anchor.rotation, connectDuration).SetEase(connectEase))
            .OnComplete(() => {
                transform.SetParent(anchor);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            });
    }

    public override void DisconnectFrom(PowerNode other)
    {
        if (other.nodeType == NodeType.PowerSource)
        {
            // Handle visual disconnection first
            AnimateDisconnection(() => {
                // Then trigger downstream disconnection through PowerNode
                DisconnectAllDownstream();
            });
        }
        else if (other.nodeType == NodeType.Plug)
        {
            var plug = other as Plug;
            plug.DisconnectFromAnchor();
        }

        base.DisconnectFrom(other); // Handles core disconnection logic
    }

    private void AnimateDisconnection(System.Action onComplete = null)
    {
        connectSequence?.Kill();

        Vector3 disconnectPos = transform.position + transform.forward * disconnectOffset;

        connectSequence = DOTween.Sequence()
            .Append(transform.DOMove(disconnectPos, disconnectDuration).SetEase(disconnectEase))
            .OnComplete(() => {
                transform.SetParent(initialParent);
                if (socketRb != null) socketRb.isKinematic = true;
                onComplete?.Invoke();
            });
    }

    public void ReturnToInitialParent()
    {
        connectSequence?.Kill();

        connectSequence = DOTween.Sequence()
            .Append(transform.DOMove(initialParent.position, disconnectDuration).SetEase(disconnectEase))
            .Join(transform.DORotateQuaternion(initialParent.rotation, disconnectDuration).SetEase(disconnectEase))
            .OnComplete(() => {
                transform.SetParent(initialParent);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                if (socketRb != null) socketRb.isKinematic = true;
            });
    }

    private void OnDestroy()
    {
        connectSequence?.Kill();
    }
}