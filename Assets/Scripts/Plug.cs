using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plug : MonoBehaviour
{
    public GridObject assignedGrid;
    [SerializeField] LayerMask socketLayer;

    public void PlaceOnGrid(GridObject _assignedGrid)
    {
        assignedGrid = _assignedGrid;
        transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(()=> {

        CheckForSocketsUnderneath();
        });
    }

    private void CheckForSocketsUnderneath()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.back, 20f, socketLayer);
        List<Socket> connectedSockets = new List<Socket>();

        foreach (RaycastHit hit in hits)
        {
            Socket socket = hit.collider.GetComponent<Socket>();
            if (socket == null)
                continue;

            bool isSocketPlaced = true;
            foreach (var cube in socket.socketCubes)
            {
                Ray ray = new Ray(cube.transform.position + Vector3.up * 2f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit cubeHit, 10f, socket.gridLayer))
                {
                    GridObject grid = cubeHit.collider.GetComponent<GridObject>();
                    if (grid == null || !grid.isOccupied)
                    {
                        isSocketPlaced = false;
                        break;
                    }
                }
                else
                {
                    isSocketPlaced = false;
                    break;
                }
            }

            if (isSocketPlaced)
            {
                connectedSockets.Add(socket);
                print(connectedSockets.Count);
            }
            else
            {
                return; // One socket in the chain isn't placed properly
            }
        }

        if (hits.Length > 0)
        {
            RaycastHit lastHit = hits[hits.Length - 1];
            Transform lastObj = lastHit.collider.transform;

            if (lastObj.GetComponent<PowerSource>() != null)
            {
                ConnectPower(connectedSockets);
            }
        }
    }
    private void ConnectPower(List<Socket> socketChain)
    {
        Debug.Log("POWER CONNECTED! Chain length: " + socketChain.Count);
        // Do visual or gameplay effect here

        // Example: call a method on the PowerPoint
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.back, out hit, 10f, socketLayer))
        {
           
        }
    }

}