using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Socket : MonoBehaviour
{
    [System.Serializable]
    public class SocketCube
    {
        public GameObject cube;
        public Transform pin; // Reference to the pin on this cube
        public float pinRestPosition = 0f; // Local Z position when unpowered
        public float pinActivePosition = 0.5f; // Local Z position when powered
        [HideInInspector] public bool hasPowerSource;
    }

    [Header("Socket Configuration")]
    public List<SocketCube> socketCubes = new List<SocketCube>();
    public LayerMask gridLayer;
    public LayerMask connectableLayers;
    public float ownMultiplier = 1f;
    public float actingMultiplier = 1f;
    public float pinMoveDuration = 0.3f;
    public Ease pinMoveEase = Ease.OutBack;

    [Header("Runtime State")]
    public bool hasPower;
    public List<GridObject> assignedGrids = new List<GridObject>();

    private void Start()
    {
        InitializePins();
    }

    private void InitializePins()
    {
        // Set all pins to rest position at start
        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
            {
                Vector3 pos = socketCube.pin.localPosition;
                pos.z = socketCube.pinRestPosition;
                socketCube.pin.localPosition = pos;
            }
        }
    }

    public bool IsReleasableByRaycast()
    {
        foreach (var socketCube in socketCubes)
        {
            Ray ray = new Ray(socketCube.cube.transform.position + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid == null || grid.isOccupied)
                    return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public List<GridObject> RequiredGridsByRaycast()
    {
        List<GridObject> grids = new List<GridObject>();

        foreach (var socketCube in socketCubes)
        {
            Ray ray = new Ray(socketCube.cube.transform.position + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid != null && !grid.isOccupied && !grids.Contains(grid))
                {
                    grids.Add(grid);
                }
            }
        }
        assignedGrids = grids;
        return grids;
    }

    public void CheckPowerActivation()
    {
        actingMultiplier = ownMultiplier;
        hasPower = false;

        // Reset all power flags first
        foreach (var socketCube in socketCubes)
        {
            socketCube.hasPowerSource = false;
        }

        foreach (var socketCube in socketCubes)
        {
            Ray ray = new Ray(socketCube.cube.transform.position, -Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f, connectableLayers))
            {
                if (hit.collider.GetComponent<PowerSource>() != null)
                {
                    Plugged();
                    actingMultiplier += hit.collider.GetComponent<PowerSource>().sourcePowerMultiplier;
                    hasPower = true;
                    socketCube.hasPowerSource = true;
                    continue;
                }

                Socket otherSocket = hit.collider.GetComponent<Socket>();
                if (otherSocket != null && otherSocket.hasPower)
                {
                    Plugged();
                    hasPower = true;
                    actingMultiplier += otherSocket.actingMultiplier;
                    socketCube.hasPowerSource = true;
                }
            }
        }

       
    }

    public void UnPlugged()
    {
        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
            {
                // Only animate pins that were previously powered
             //   if (socketCube.hasPowerSource)
              //  {
                    socketCube.pin.DOLocalMoveZ(
                        socketCube.pinRestPosition,
                        pinMoveDuration
                    ).SetEase(pinMoveEase);
              //  }
            }
        }
    }

    public void Plugged()
    {
        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
            {
                // Only animate pins that have power sources
              //  if (socketCube.hasPowerSource)
              //  {
                    socketCube.pin.DOLocalMoveZ(
                        socketCube.pinActivePosition,
                        pinMoveDuration
                    ).SetEase(pinMoveEase);
              //  }
            }
        }
    }
}