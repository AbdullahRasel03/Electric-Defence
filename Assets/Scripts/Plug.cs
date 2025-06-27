using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plug : MonoBehaviour
{

    public bool placedOnGrid;
    public GridObject assignedGrid;
    [SerializeField] LayerMask connectableLayers;
    public TowerController connectedTower;

    public void PlaceOnGrid(GridObject _assignedGrid)
    {
        assignedGrid = _assignedGrid;
        transform.DOLocalMove(Vector3.zero - Vector3.forward * 0.1f, 0.2f).OnComplete(()=> {

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
          
                ConnectPower(1); // no sockets involved
                return;
            }

            Socket socket = hit.collider.GetComponent<Socket>();
            if (socket != null)
            {
                if (socket.hasPower)
                {
                    ConnectPower(socket.actingMultiplier);
                }
                else
                {
                    connectedTower.DeactivateTower();
                }
            }
            else
            {
                connectedTower.DeactivateTower();
            }
        }
    }

    private void ConnectPower(float fireRate)
    {
      
        connectedTower.ActivateTower();
        connectedTower.shooter.SetFireRate(fireRate);
    }


}