using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public bool isOccupied;
    public bool isLaserPath;
    public bool isBlocked;
    public GameObject blocker;

    public GridManager gridManager;
    public LayerMask gridLayer;
    public Transform plugSocketHolder;

    public Socket socket;
    //public Plug plug;
    public Renderer gridRenderer;
    private Color defaultColor;
    public Color highlightColor = Color.yellow;



    private void Awake()
    {
        defaultColor = gridRenderer.material.color;
    }
    private void Start()
    {
        if (isBlocked)
        {
            gridRenderer.enabled = false;
            blocker.SetActive(true);
        }
        else if (isLaserPath)
        {
           // gridRenderer.enabled = false;
        }
    }
    public void Highlight()
    {
        gridRenderer.material.color = highlightColor;
    }

    public void ResetHighlight()
    {
        gridRenderer.material.color = defaultColor;
    }

    public void ReleaseToGrid(Plug plug, Socket assignedSocket)
    {
        plug.transform.parent = socket.plugHolder;
      //  isOccupied = true;
       // this.plug = plug;
        plug.PlaceOnGrid(this, assignedSocket);
    }

    public static bool TryReleaseSocketToGrids(Socket socket, out Vector3 newSocketWorldPos, out GridObject anchorGrid)
    {
        newSocketWorldPos = Vector3.zero;
        anchorGrid = null;

        Dictionary<GameObject, GridObject> cubeGridMap = new();
        foreach (var cubeEntry in socket.socketCubes)
        {
            GameObject cube = cubeEntry.cube.gameObject;
            Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, socket.gridLayer))
            {
                GridObject grid = hit.collider.GetComponent<GridObject>();
                if (grid != null)
                {
                    if (!grid.isOccupied)
                    {
                        cubeGridMap[cube] = grid;
                    }
                    else
                    {
                        Socket targetSocket = grid.socket;
                        if (targetSocket != null && targetSocket.socketManager.CanMergeSockets(socket, targetSocket))
                        {
                            foreach (var grd in socket.assignedGrids)
                            {
                                print("here");
                                if (grd.gridManager != null)
                                    grd.gridManager.CheckAllGridsPower();
                            }
                            targetSocket.socketManager.TryMergeSockets(socket, targetSocket);
                            return false;
                        }
                    }
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

        GameObject firstCube = socket.socketCubes[0].cube.gameObject;
        GridObject firstGrid = cubeGridMap[firstCube];

        anchorGrid = firstGrid;

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

        //newSocketWorldPos.y = 0.08f;
        return true;
    }


}
