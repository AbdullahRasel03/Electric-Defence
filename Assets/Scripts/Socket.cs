using UnityEngine;
using System.Collections.Generic;

public class Socket : MonoBehaviour
{
    public bool hasPower;  // ← Fixed declaration (was missing `;`)
    public List<GameObject> socketCubes;
    public LayerMask gridLayer;
    public List<GridObject> assignedGrids = new List<GridObject>();
    [SerializeField] LayerMask connectableLayers;
    public float ownMultiplier, actingMultiplier;

    public bool IsReleasableByRaycast()
    {
        foreach (var cube in socketCubes)
        {
            Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
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

        foreach (var cube in socketCubes)
        {
            Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
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

        foreach (var cube in socketCubes)
        {
            Ray ray = new Ray(cube.transform.position, -Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f, connectableLayers))
            {
                if (hit.collider.GetComponent<PowerSource>() != null)
                {
                    actingMultiplier += hit.collider.GetComponent<PowerSource>().sourcePowerMultiplier;
                    hasPower = true;
                    return;
                }

                Socket otherSocket = hit.collider.GetComponent<Socket>();
                if (otherSocket != null && otherSocket.hasPower)
                {
                    hasPower = true;
                    actingMultiplier += otherSocket.actingMultiplier;
                    return;
                }
            }
        }
    }
}
