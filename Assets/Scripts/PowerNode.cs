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

    // In PowerNode.cs
    public bool IsConnectedToPowerSource()
    {
        HashSet<PowerNode> visited = new HashSet<PowerNode>();
        return CheckPowerSourceConnection(visited);
    }

    protected bool CheckPowerSourceConnection(HashSet<PowerNode> visited)
    {
        if (visited.Contains(this)) return false;
        visited.Add(this);

        // If this is a power source, we found one!
        if (nodeType == NodeType.PowerSource) return true;

        // Check all connected nodes
        foreach (var node in connectedNodes)
        {
            if (node != null && node.CheckPowerSourceConnection(visited))
            {
                return true;
            }
        }

        return false;
    }

    public float CalculateEffectivePower()
    {
        if (!IsConnectedToPowerSource()) return 0f;

        HashSet<PowerNode> visited = new HashSet<PowerNode>();
        return CalculatePowerToSource(visited);
    }

    protected float CalculatePowerToSource(HashSet<PowerNode> visited)
    {
        if (visited.Contains(this)) return 0f;
        visited.Add(this);

        // Power sources terminate the recursion
        if (nodeType == NodeType.PowerSource)
        {
            return powerMultiplier;
        }

        float totalPower = 0f;

        // Sum power from all paths to sources
        foreach (var node in connectedNodes)
        {
            if (node != null)
            {
                totalPower += node.CalculatePowerToSource(visited) * powerMultiplier;
            }
        }

        return totalPower;
    }
    private void OnGUI()
    {
        if (Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            string status = $"Type: {nodeType}\n" +
                           $"State: {powerState}\n" +
                           $"Connected to source: {IsConnectedToPowerSource()}\n" +
                           $"Power: {CalculateEffectivePower()}";

            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200, 80), status);
        }
    }

    public virtual void DisconnectAllDownstream()
    {
        HashSet<PowerNode> visited = new HashSet<PowerNode>();
        DisconnectDownstreamRecursive(this, visited);
    }

    protected virtual void DisconnectDownstreamRecursive(PowerNode node, HashSet<PowerNode> visited)
    {
        if (visited.Contains(node)) return;
        visited.Add(node);

        // Create a copy of the list to avoid modification during iteration
        var nodesToDisconnect = new List<PowerNode>(node.connectedNodes);

        foreach (var connectedNode in nodesToDisconnect)
        {
            if (connectedNode != null)
            {
                // Only disconnect downstream nodes (not back toward power source)
                if (connectedNode.nodeType != NodeType.PowerSource)
                {
                    node.DisconnectFrom(connectedNode);
                    connectedNode.DisconnectDownstreamRecursive(connectedNode, visited);
                }
            }
        }
    }
}