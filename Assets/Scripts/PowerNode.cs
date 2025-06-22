using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(DragSystem))]
public class PowerNode : MonoBehaviour
{
    public enum NodeType { Plug, Socket, PowerSource }
    public enum PowerState { Disconnected, Connected, Powered }

    [Header("Node Settings")]
    public NodeType nodeType;
    public PowerState powerState;
    public float powerMultiplier = 1f;
    public LayerMask connectableLayers;
    public float connectionRadius = 0.5f;

    [Header("Visuals")]
    public Color disconnectedColor = Color.gray;
    public Color connectedColor = Color.yellow;
    public Color poweredColor = Color.green;
    public MeshRenderer indicatorRenderer;

    [Header("Connections")]
    public List<PowerNode> connectedNodes = new List<PowerNode>();
    public PowerNode parentNode; // Where power comes from

    protected Material indicatorMaterial;
    protected DragSystem dragSystem;


    public Transform plugParent;
    private void Awake()
    {
        dragSystem = GetComponent<DragSystem>();

        if (indicatorRenderer != null)
        {
            indicatorMaterial = indicatorRenderer.material;
            UpdateVisuals();
        }
    }

    public virtual bool CanConnectWith(PowerNode other)
    {
        // Basic connection rules
        if (other == this) return false;
        if (connectedNodes.Contains(other)) return false;

        // Type-specific rules will be defined in child classes
        return true;
    }

    public virtual void ConnectTo(PowerNode other)
    {
        if (!CanConnectWith(other)) return;
        connectedNodes.Add(other);
        other.connectedNodes.Add(this);
        UpdatePowerState();
        other.UpdatePowerState();
    }

    public virtual void DisconnectFrom(PowerNode other)
    {
        connectedNodes.Remove(other);
        other.connectedNodes.Remove(this);
        UpdatePowerState();
        other.UpdatePowerState();
    }

 
    public virtual void DisconnectAll()
    {
        foreach (var node in new List<PowerNode>(connectedNodes))
        {
            DisconnectFrom(node);
        }
    }

    public virtual void UpdatePowerState()
    {
        // Use a new HashSet for each propagation
        UpdatePowerState(new HashSet<PowerNode>());
    }

    protected virtual void UpdatePowerState(HashSet<PowerNode> visited)
    {
        // Skip if we've already visited this node
        if (visited.Contains(this)) return;
        visited.Add(this);

        // Store previous state to detect changes
        PowerState previousState = powerState;

        // Determine new state
        bool isPowered = CheckPowerConnection(visited);
        powerState = connectedNodes.Count > 0
            ? (isPowered ? PowerState.Powered : PowerState.Connected)
            : PowerState.Disconnected;

        // Only update visuals and propagate if state changed
        if (powerState != previousState)
        {
            UpdateVisuals();
            PropagatePowerState(visited);
        }
    }

    protected virtual bool CheckPowerConnection(HashSet<PowerNode> visited)
    {
        // Check if we're directly or indirectly connected to a power source
        HashSet<PowerNode> localVisited = new HashSet<PowerNode>(visited);
        return CheckPowerConnectionRecursive(this, localVisited);
    }

    private bool CheckPowerConnectionRecursive(PowerNode node, HashSet<PowerNode> visited)
    {
        if (visited.Contains(node)) return false;
        visited.Add(node);

        if (node.nodeType == NodeType.PowerSource) return true;

        foreach (var connectedNode in node.connectedNodes)
        {
            if (CheckPowerConnectionRecursive(connectedNode, visited))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void PropagatePowerState(HashSet<PowerNode> visited)
    {
        foreach (var node in connectedNodes)
        {
            node.UpdatePowerState(visited);
        }
    }
    protected virtual void UpdateVisuals()
    {
        if (indicatorMaterial == null) return;

        Color targetColor = disconnectedColor;
        if (powerState == PowerState.Connected) targetColor = connectedColor;
        if (powerState == PowerState.Powered) targetColor = poweredColor;

        indicatorMaterial.color = targetColor;
    }

    private void OnDrawGizmos()
    {
        // Draw connection lines in editor
        foreach (var node in connectedNodes)
        {
            if (node != null)
            {
                Gizmos.color = powerState == PowerState.Powered ? Color.green :
                              (powerState == PowerState.Connected ? Color.yellow : Color.gray);
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }

    public virtual float CalculatePowerOutput()
    {
        // Base case: power sources return their own multiplier
        if (nodeType == NodeType.PowerSource)
        {
            return powerMultiplier;
        }

        // For other nodes, find all paths to power sources and sum their effective outputs
        HashSet<PowerNode> visited = new HashSet<PowerNode>();
        return CalculatePowerOutputRecursive(this, visited);
    }

    private float CalculatePowerOutputRecursive(PowerNode node, HashSet<PowerNode> visited)
    {
        if (visited.Contains(node)) return 0f;
        visited.Add(node);

        float totalPower = 0f;

        foreach (var connectedNode in node.connectedNodes)
        {
            if (connectedNode.nodeType == NodeType.PowerSource)
            {
                totalPower += connectedNode.powerMultiplier * node.powerMultiplier;
            }
            else
            {
                totalPower += CalculatePowerOutputRecursive(connectedNode, visited) * node.powerMultiplier;
            }
        }

        return totalPower;
    }
}