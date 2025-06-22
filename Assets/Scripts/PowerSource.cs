using UnityEngine;
using System.Collections.Generic;

public class PowerSource : PowerNode
{
    [Header("Power Source Settings")]
    public float basePowerOutput = 1f;

    private void Awake()
    {
        nodeType = NodeType.PowerSource;
        powerMultiplier = basePowerOutput;
        powerState = PowerState.Powered; // Always powered

        // Power sources are always fixed
        DragSystem dragSystem = GetComponent<DragSystem>();
        if (dragSystem != null) dragSystem.enabled = false;
    }

    public override bool CanConnectWith(PowerNode other)
    {
        if (!base.CanConnectWith(other)) return false;

        // Power source can only connect to plugs or sockets
        return other.nodeType == NodeType.Plug || other.nodeType == NodeType.Socket;
    }

    public override void UpdatePowerState()
    {
        // Power source is always powered - use new HashSet for propagation
        UpdatePowerState(new HashSet<PowerNode>());
    }

    protected override void UpdatePowerState(HashSet<PowerNode> visited)
    {
        // Skip if already visited
        if (visited.Contains(this)) return;
        visited.Add(this);

        // Power source is always powered
        powerState = PowerState.Powered;
        UpdateVisuals();
        PropagatePowerState(visited);
    }
}