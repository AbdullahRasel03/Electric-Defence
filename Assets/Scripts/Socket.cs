using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

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
    public Transform initialParent;
    private bool isDisconnecting = false;

    private void Start()
    {
        socketRb = GetComponent<Rigidbody>();
        nodeType = NodeType.Socket;
        initialParent = transform.parent;
    }

    public override void ConnectTo(PowerNode other)
    {
       // if (!CanConnectWith(other)) return;
        base.ConnectTo(other);


        if (other.nodeType == NodeType.PowerSource || other.nodeType == NodeType.Socket)
        {
            ConnectToAnchor(other.plugParent);
        }
    }

    public void ConnectToAnchor(Transform anchor)
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
        transform.SetParent(null);
        if (isDisconnecting) return;
        if (other.nodeType == NodeType.PowerSource)
        {
            // Start cascading disconnection
            StartCoroutine(CascadeDisconnect());
        }
        else if (other.nodeType == NodeType.Plug)
        {
            Plug plug = other as Plug;
            plug.DisconnectFromAnchor();
        }
        base.DisconnectFrom(other);

    }

    private IEnumerator CascadeDisconnect()
    {
        isDisconnecting = true;

        // First disconnect visually
        DisconnectFromAnchor();

        // Wait for disconnection animation to complete
        yield return new WaitForSeconds(0.1f);

        // Now disconnect all downstream nodes
        DisconnectAllDownstream();

        isDisconnecting = false;
    }

    public void DisconnectFromAnchor()
    {
        connectSequence?.Kill();


        transform.SetParent(initialParent);
        if (socketRb != null) socketRb.isKinematic = true;

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
                if (socketRb != null) socketRb.isKinematic = false;
            });
    }

    protected override void UpdatePowerState(HashSet<PowerNode> visited)
    {
        if (visited.Contains(this)) return;
        visited.Add(this);

        PowerState previousState = powerState;

        bool isPowered = IsConnectedToPowerSource();
        powerState = connectedNodes.Count > 0
            ? (isPowered ? PowerState.Powered : PowerState.Connected)
            : PowerState.Disconnected;

        if (powerState != previousState)
        {
            UpdateVisuals();
            PropagatePowerState(visited);
        }
    }


    private void OnDestroy()
    {
        connectSequence?.Kill();
    }
}