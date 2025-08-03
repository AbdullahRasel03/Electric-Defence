using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plug : MonoBehaviour
{

    public bool placedOnGrid;
    public GridObject assignedGrid;
    [SerializeField] LayerMask connectableLayers;
    public TowerController connectedTower;
    public Socket assignedSocket;
    public Renderer plugGFX;
    public void PlaceOnGrid(GridObject _assignedGrid, Socket _assignedSocket)
    {
        assignedGrid = _assignedGrid;
        assignedSocket= _assignedSocket;
        transform.DOLocalMove(Vector3.zero + Vector3.forward * 0.1f, 0.3f).OnComplete(() =>
        {
            transform.DOLocalMove(Vector3.zero - Vector3.forward * 0.02f, 0.2f).OnComplete(() =>
            {
                assignedGrid.gridManager.CheckAllGridsPower();
            });
        });
    }

    public void CheckForSocketsUnderneath()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f, connectableLayers))
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
        plugGFX.materials[0].color = new Color(0.5f, 1f, 0.5f); // Light green

        connectedTower.ActivateTower();
        connectedTower.shooter.SetFireRate(fireRate);
    }


}