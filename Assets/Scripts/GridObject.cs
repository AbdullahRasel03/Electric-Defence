using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public GridManager gridManager;
    public bool isOccupied;
    public LayerMask gridLayer;
    public Transform plugSocketHolder;

    public Socket socket;
    public Plug plug;
    public Renderer gridRenderer;
    private Color defaultColor;
    public Color highlightColor = Color.yellow;

    private void Awake()
    {
        defaultColor = gridRenderer.material.color;
    }

    public void Highlight()
    {
        gridRenderer.material.color = highlightColor;
    }

    public void ResetHighlight()
    {
        gridRenderer.material.color = defaultColor;
    }

    public void ReleaseToGrid(Plug plug)
    {
        plug.transform.parent = plugSocketHolder;
        isOccupied = true;
        this.plug = plug;
        plug.PlaceOnGrid(this);
    }

    public static bool TryReleaseSocketToGrids(Socket socket, out Vector3 newSocketWorldPos)
    {
        newSocketWorldPos = Vector3.zero;
        if (!socket.IsReleasableByRaycast())
            return false;

        Dictionary<GameObject, GridObject> cubeGridMap = new();
        foreach (var cubeEntry in socket.socketCubes)
        {
            GameObject cube = cubeEntry.cube;
            Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, socket.gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid != null && !grid.isOccupied)
                {
                    cubeGridMap[cube] = grid;
                }
            }
        }

        if (cubeGridMap.Count != socket.socketCubes.Count)
            return false;

        foreach (var grid in socket.assignedGrids)
        {
            if (grid != null)
            {
                grid.isOccupied = false;
                grid.socket = null;
            }
        }

        socket.assignedGrids.Clear();

        GameObject firstCube = socket.socketCubes[0].cube;
        GridObject firstGrid = cubeGridMap[firstCube];

        Vector3 cubeWorldPos = firstCube.transform.position;
        Vector3 gridWorldPos = firstGrid.plugSocketHolder.position;
        Vector3 socketOffset = cubeWorldPos - socket.transform.position;

        newSocketWorldPos = gridWorldPos - socketOffset;

        foreach (var pair in cubeGridMap)
        {
            GridObject grid = pair.Value;
            grid.isOccupied = true;
            grid.socket = socket;
            socket.assignedGrids.Add(grid);
        }

       /* foreach (var grid in socket.assignedGrids)
        {
            if (grid.gridManager != null)
                grid.gridManager.CheckAllGridsPower();
        }*/
        newSocketWorldPos.y = 0.08f;
        return true;
    }
}
