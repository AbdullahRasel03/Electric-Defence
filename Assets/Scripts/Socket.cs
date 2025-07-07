using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class Socket : MonoBehaviour
{
    #region Configuration

    [Header("Base Configuration")]
    public int currentLevel = 1;
    public SocketShapeType shapeType;
    public Transform cubesParent;

    [Header("Visuals")]
    public Color[] levelColors;

    [Header("Socket Configuration")]
    public LayerMask gridLayer;
    public LayerMask connectableLayers;
    public float ownMultiplier = 1f;
    public float actingMultiplier = 1f;
    public float pinMoveDuration = 0.3f;
    public Ease pinMoveEase = Ease.OutBack;

    #endregion

    #region Runtime State

    public List<SocketCube> socketCubes = new();
    public bool isMerging;
    public bool hasPower;
    public List<GridObject> assignedGrids = new();
    public SocketManager socketManager;

    private TMP_Text[] fireRateTexts;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeSocket();
    }

    private void InitializeSocket()
    {
        AssignFireRateTexts();
        InitializePins();
        UpdateColorAndTextByLevel();
    }

    private void AssignFireRateTexts()
    {
        fireRateTexts = cubesParent.GetComponentsInChildren<TMP_Text>();
    }

    private void InitializePins()
    {
        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
            {
                socketCube.unpluggedZ = socketCube.pin.localPosition.z;
                socketCube.pluggedZ = socketCube.pin.localPosition.z - 0.5f;
            }
        }
    }

    #endregion

    #region Power Handling

    public void CheckPowerActivation()
    {
        actingMultiplier = ownMultiplier;
        hasPower = false;

        foreach (var socketCube in socketCubes)
        {
            socketCube.hasPowerSource = false;
        }

        foreach (var socketCube in socketCubes)
        {
            Ray ray = new Ray(socketCube.cube.transform.position, -Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f, connectableLayers))
            {
                var powerSource = hit.collider.GetComponent<PowerSource>();
                if (powerSource != null)
                {
                    Plugged();
                    actingMultiplier += powerSource.sourcePowerMultiplier;
                    hasPower = true;
                    socketCube.hasPowerSource = true;
                    continue;
                }

                Socket otherSocket = hit.collider.GetComponent<Socket>();
                if (otherSocket != null && otherSocket.hasPower)
                {
                    Plugged();
                    actingMultiplier += otherSocket.actingMultiplier;
                    hasPower = true;
                    socketCube.hasPowerSource = true;
                }
            }
        }
    }

    public void Plugged()
    {
        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
            {
                socketCube.pin.DOLocalMoveZ(socketCube.pluggedZ, pinMoveDuration).SetEase(pinMoveEase);
            }
        }
    }

    public void UnPlugged()
    {
        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
            {
                socketCube.pin.DOLocalMoveZ(socketCube.unpluggedZ, pinMoveDuration).SetEase(pinMoveEase);
            }
        }
    }

    #endregion

    #region Grid Detection

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
            else return false;
        }
        return true;
    }

    public List<GridObject> RequiredGridsByRaycast()
    {
        List<GridObject> grids = new();
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

    #endregion

    #region Visuals

    public void UpdateColorAndTextByLevel()
    {
        UpdateFireRateDisplay();

        if (levelColors == null || levelColors.Length == 0) return;

        Color colorToApply = levelColors[Mathf.Clamp(currentLevel, 0, levelColors.Length - 1)];

        foreach (var cubeEntry in socketCubes)
        {
            Renderer[] rends = cubeEntry.cubeRenderers;
            foreach (var rend in rends)
            {
                if (rend != null)
                {
                    rend.material.color = colorToApply;
                }
            }
            
        }
    }

    private void UpdateFireRateDisplay()
    {
        if (fireRateTexts == null) return;

        foreach (var item in fireRateTexts)
        {
            item.text = ownMultiplier.ToString("0") + "x";
        }
    }

    #endregion

    #region Auto-Assignment (Editor-Only)

    public void AutoAssignSocketCubes()
    {
        socketCubes.Clear();

        foreach (Transform child in cubesParent)
        {
            SocketCube cubeEntry = new SocketCube
            {
                cube = child.gameObject,
            };

            socketCubes.Add(cubeEntry);
        }
    }

    #endregion
}
