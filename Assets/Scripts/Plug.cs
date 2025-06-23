using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plug : PowerNode
{
    [Header("DOTween Settings")]
    public float connectDuration = 0.5f;
    public Ease connectEase = Ease.OutBack;
    public float disconnectDuration = 0.3f;
    public Ease disconnectEase = Ease.InOutQuad;

    private Sequence connectSequence;
    private Rigidbody plugRb;
    private Transform initialParent; // Stores the original parent

    public TurretBehaviour connectedTurret;
    private void Awake()
    {
        plugRb = GetComponent<Rigidbody>();
        nodeType = NodeType.Plug;
        initialParent = transform.parent; // Store initial parent
    }

    public override void ConnectTo(PowerNode other)
    {
        if (!CanConnectWith(other)) return;

        base.ConnectTo(other);

        if (other.nodeType == NodeType.PowerSource || other.nodeType == NodeType.Socket)
        {
            MoveToPosition(other.plugParent);
        }

        if (other.powerState == PowerState.Powered)
        {
            connectedTurret.UpdateFireRate(CalculatePowerOutput());
        }
    }
    // In Plug.cs
    protected override void UpdatePowerState(HashSet<PowerNode> visited)
    {
        // Skip if already visited
        if (visited.Contains(this)) return;
        visited.Add(this);

        // Store previous state
        PowerState previousState = powerState;

        // Calculate new state
        bool isPowered = IsConnectedToPowerSource(); // Use the direct check method
        powerState = connectedNodes.Count > 0
            ? (isPowered ? PowerState.Powered : PowerState.Connected)
            : PowerState.Disconnected;

        // Only update if state changed
        if (powerState != previousState)
        {
            UpdateVisuals();

            // Handle turret power
            if (connectedTurret != null)
            {
                if (powerState == PowerState.Powered)
                {
                    float totalPower = CalculateEffectivePower();
                    connectedTurret.InititateTurret();
                    connectedTurret.UpdateFireRate(totalPower);
                }
                else
                {
                    connectedTurret.DeactivateTurret();
                    connectedTurret.ResetFireRate();
                }
            }

            PropagatePowerState(visited);
        }
    }
    public void MoveToPosition(Transform anchor)
    {
        // Kill any existing tweens
        connectSequence?.Kill();

        // Disable physics during connection
        if (plugRb != null) plugRb.isKinematic = true;

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
        base.DisconnectFrom(other);
        if (other.nodeType == NodeType.PowerSource || other.nodeType == NodeType.Socket)
        {
            DisconnectFromAnchor();
        }
        transform.SetParent(initialParent);
    }

    public void DisconnectFromAnchor()
    {
        // Kill any existing tweens
        connectSequence?.Kill();
        transform.SetParent(initialParent);

        // Re-enable physics
        if (plugRb != null)
        {
            plugRb.isKinematic = true;
        }
        // Create smooth disconnection sequence
       
    }

  
    private void OnDestroy()
    {
        // Clean up tweens when destroyed
        connectSequence?.Kill();
    }
}