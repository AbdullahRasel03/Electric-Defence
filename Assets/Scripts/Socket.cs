using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
public class Socket : MonoBehaviour
{
  
    public int currentLevel = 1;
    public SocketShapeType shapeType;
    public bool isMerging;
    public GameObject pins;
    [Header("Visuals")]
    public Color[] levelColors; // Assign via Inspector, index = level (0, 1, 2, ...)
    public TMP_Text[] fireRateTexts;
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

    public SocketManager socketManager;
    private void Start()
    {
        InitializePins();
        foreach (var item in fireRateTexts)
        {
            item.text = ownMultiplier.ToString() + "x";
        }
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
    public void UpdateColorAndTextByLevel()
    {
        if (levelColors == null || levelColors.Length == 0) return;

        foreach (var item in fireRateTexts)
        {
            item.text = ownMultiplier.ToString("0.0") + "x";
        }
        if (levelColors.Length > 0)
        {

            Color colorToApply = levelColors[Mathf.Clamp(currentLevel, 0, levelColors.Length - 1)];
            foreach (var cubeEntry in socketCubes)
            {
                Renderer rend = cubeEntry.cube.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = colorToApply;
                }
            }
        }
    }

}