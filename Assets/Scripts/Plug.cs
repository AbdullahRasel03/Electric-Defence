using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plug : MonoBehaviour
{

    public bool placedOnGrid;
    public GridObject assignedGrid;
    [SerializeField] LayerMask connectableLayers;
    public TurretBehaviour connectedTurret;

    public void PlaceOnGrid(GridObject _assignedGrid)
    {
        assignedGrid = _assignedGrid;
        transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(()=> {

            assignedGrid.gridManager.CheckAllGridsPower();
        });
    }

    public void CheckForSocketsUnderneath()
    {
        Ray ray = new Ray(transform.position, Vector3.back);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f, connectableLayers))
        {
            if (hit.collider.GetComponent<PowerSource>() != null)
            {
                Debug.Log("Directly connected to power source!");
                ConnectPower(new List<Socket>()); // no sockets involved
                return;
            }

            Socket socket = hit.collider.GetComponent<Socket>();
            if (socket != null)
            {
                if (socket.hasPower)
                {
                    ConnectPower(new List<Socket>());
                }
            }
        }
    }

    private void ConnectPower(List<Socket> socketChain)
    {
        Debug.Log("POWER CONNECTED! Chain length: " + socketChain.Count);
        connectedTurret.InititateTurret();
        float fireRate = 1;
        foreach (var item in socketChain)
        {
            fireRate += item.multiplier;
        }
        connectedTurret.UpdateFireRate(fireRate);
    }


}