using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum DragObjectType
{
    Plug,
    Socket
}

[RequireComponent(typeof(Collider))]
public class DragSystem : MonoBehaviour
{
    public DragObjectType objectType;

    [Header("Drag Settings")]
    public float dragSpeed = 15f;
    public float snapSpeed = 20f;
    public bool lockYAxis = true;
    public float fixedYPosition = 0f;

    public bool IsBeingDragged { get; private set; }
    public bool JustReleased;

    private Vector3 dragOffset;
    private Vector3 initialPosition;

    public LayerMask gridLayer;


    private GridObject lastHoveredGrid;
    private List<GridObject> lastHoveredGrids = new List<GridObject>();

    private void OnMouseDown()
    {
        initialPosition = transform.position;
        StartDrag();
    }

    private void OnMouseUp()
    {
        EndDrag();
    }

    public void StartDrag()
    {
        IsBeingDragged = true;
        JustReleased = false;

        Vector3 mousePosition = GetMouseWorldPosition();
        dragOffset = mousePosition - transform.position;

        if (objectType == DragObjectType.Plug)
        {
            Plug plug = GetComponent<Plug>();
            if (plug != null && plug.assignedGrid != null)
            {
                plug.assignedGrid.isOccupied = false;
                plug.assignedGrid.plug = null;
                plug.assignedGrid = null;
            }
            plug.connectedTower.DeactivateTower();
        }
        else if (objectType == DragObjectType.Socket)
        {
            Socket socket = GetComponent<Socket>();
            if (socket != null && socket.assignedGrids != null)
            {
                foreach (var grid in socket.assignedGrids)
                {
                    if (grid != null)
                    {
                        grid.isOccupied = false;
                        grid.socket = null;
                    }
                }
                socket.assignedGrids.Clear();
            }
        }
    }


    public void EndDrag()
    {
        IsBeingDragged = false;
        JustReleased = true;
        CheckForGridUnderneath();
    }

    private void Update()
    {
        if (IsBeingDragged)
        {
            Vector3 targetPosition = GetMouseWorldPosition() - dragOffset;
            if (lockYAxis)
                targetPosition.y = fixedYPosition;

            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);

            UpdateGridHighlight();
        }
    }
    private void UpdateGridHighlight()
    {
        // Reset previous highlights
        if (objectType == DragObjectType.Plug && lastHoveredGrid != null)
        {
            lastHoveredGrid.ResetHighlight();
            lastHoveredGrid = null;
        }
        else if (objectType == DragObjectType.Socket && lastHoveredGrids.Count > 0)
        {
            foreach (var g in lastHoveredGrids)
                g.ResetHighlight();

            lastHoveredGrids.Clear();
        }

        if (objectType == DragObjectType.Plug)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid != null && !grid.isOccupied)
                {
                    grid.Highlight();
                    lastHoveredGrid = grid;
                }
            }
        }
        else
        {
            Socket socket = GetComponent<Socket>();
            foreach (var cube in socket.socketCubes)
            {
                Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, 10f, gridLayer))
                {
                    GridObject grid = hit.collider.GetComponent<GridObject>();
                    if (grid != null && !grid.isOccupied && !lastHoveredGrids.Contains(grid))
                    {
                        grid.Highlight();
                        lastHoveredGrids.Add(grid);
                    }
                }
            }
        }
    }

    private void CheckForGridUnderneath()
    {
        if (objectType == DragObjectType.Plug)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid != null && !grid.isOccupied)
                {
                    grid.ReleaseToGrid(GetComponent<Plug>());
                    return;
                }
            }
        }
        else
        {
            Socket socket = GetComponent<Socket>();
            if (socket.IsReleasableByRaycast())
            {
                List<GridObject> requiredGrids = socket.RequiredGridsByRaycast();

                // Clear previous grid assignments
                foreach (var grid in socket.assignedGrids)
                {
                    grid.isOccupied = false;
                    grid.socket = null;
                }
                socket.assignedGrids.Clear();

                // Map each cube to the grid below it
                Dictionary<GameObject, GridObject> cubeGridMap = new Dictionary<GameObject, GridObject>();

                for (int i = 0; i < socket.socketCubes.Count; i++)
                {
                    GameObject cube = socket.socketCubes[i];
                    Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, 10f, gridLayer))
                    {
                        GridObject grid = hit.collider.GetComponent<GridObject>();
                        if (grid != null && !grid.isOccupied)
                        {
                            cubeGridMap.Add(cube, grid);
                        }
                    }
                }

                if (cubeGridMap.Count != socket.socketCubes.Count)
                {
                    transform.position = initialPosition;
                    return; // Not all cubes over valid grids
                }

                // Calculate required movement
                GameObject firstCube = socket.socketCubes[0];
                GridObject firstGrid = cubeGridMap[firstCube];

                Vector3 cubeWorldPos = firstCube.transform.position;
                Vector3 gridWorldPos = firstGrid.plugSocketHolder.position;

                Vector3 socketOffset = cubeWorldPos - socket.transform.position;
                Vector3 newSocketPosition = gridWorldPos - socketOffset;

                // Move the whole socket to align first cube
                socket.transform.DOMove(newSocketPosition, 0.2f).OnComplete(()=> {
                    foreach (var pair in cubeGridMap)
                    {
                        GridObject grid = pair.Value;
                        grid.isOccupied = true;
                        grid.socket = socket;
                        socket.assignedGrids.Add(grid);

                    }
                    foreach (var pair in cubeGridMap)
                    {
                        pair.Value.gridManager.CheckAllGridsPower();
                    }

                });

                // Mark all involved grids
               
                return;
            }

        }

        transform.position = initialPosition; // No valid connection, reset position
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
