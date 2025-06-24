using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plug : MonoBehaviour
{
    public GridObject assignedGrid;
    [SerializeField] LayerMask connectableLayers;
    public TurretBehaviour connectedTurret;
    public void PlaceOnGrid(GridObject _assignedGrid)
    {
        assignedGrid = _assignedGrid;
        transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(()=> {

        CheckForSocketsUnderneath();
        });
    }

    private void CheckForSocketsUnderneath()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -Vector3.forward, 10f, connectableLayers);
        List<Socket> connectedSockets = new List<Socket>();
        print(hits.Length);
        foreach (RaycastHit hit in hits)
        {
            Socket socket = hit.collider.GetComponent<Socket>();
            if (socket == null)
                continue;
            connectedSockets.Add(socket);
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
        connectedTurret.InititateTurret();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.back, out hit, 10f, connectableLayers))
        {
           
        }
    }

}