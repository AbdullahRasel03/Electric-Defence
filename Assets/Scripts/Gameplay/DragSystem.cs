// DragSystem.cs
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
    public float dragSpeed = 10f;
    public bool lockYAxis = true;
    public float dragYOffset = 1f;
    public float fixedYPosition = 0f;

    public bool IsBeingDragged { get; private set; }
    public bool JustReleased;

    private Vector3 dragOffset;
    private Vector3 initialPosition;
    private float originalYPosition;

    public LayerMask gridLayer;

    private GridObject lastHoveredGrid;
    private List<GridObject> lastHoveredGrids = new List<GridObject>();

    private void OnMouseDown()
    {
        initialPosition = transform.position;
        originalYPosition = initialPosition.y;
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
                plug.assignedSocket.connectedPlug = null;
                plug.assignedSocket = null;
                // plug.assignedGrid.isOccupied = false;
                // plug.assignedGrid.plug = null;
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
                targetPosition.y = fixedYPosition + dragYOffset;
            else
                targetPosition.y += dragYOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
            UpdateGridHighlight();
        }
    }

    private void UpdateGridHighlight()
    {
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
                Ray ray = new Ray(cube.cube.position + Vector3.up * 2f + Vector3.back * 0.5f, Vector3.down);
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
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid != null && grid.isOccupied)
                {
                    grid.ReleaseToGrid(GetComponent<Plug>(), grid.socket);
                    return;
                }
            }
        }
        else
        {
            Socket socket = GetComponent<Socket>();
            if (!GridObject.TryReleaseSocketToGrids(socket, out Vector3 newPosition, out GridObject grid))
            {
                transform.position = initialPosition;
                return;
            }

            socket.socketManager.RemoveSocketFromSpwanedList(socket);
            socket.socketManager.activeGrids.Add(socket);
            newPosition.y = 0;
            socket.transform.DOMove(newPosition + Vector3.forward * 0.3f, 0.25f).OnComplete(() =>
            {
                socket.transform.DOMove(newPosition, 0.15f).OnComplete(() =>
                {
                    foreach (var grid in socket.assignedGrids)
                    {
                        if (grid.gridManager != null)
                        {
                            grid.gridManager.CheckAllGridsPower();
                            break;
                        }

                    }
                });
            });
            return;
        }

        transform.position = initialPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
