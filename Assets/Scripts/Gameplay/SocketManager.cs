using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Collections;

public enum SocketShapeType
{
    a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z
}

/*[System.Serializable]
public class SocketCube
{
    public GameObject cube;
}*/
public class SocketManager : MonoBehaviour
{
    [Header("Socket Tracking")]
    [SerializeField] private List<Socket> spawnedNewSockets = new();
    [SerializeField] public List<Socket> activeGrids = new();

    [SerializeField] private ParticleSystem mergeParticlesPrefab;
    private SocketSpawner socketSpawner;

    private void Awake()
    {
        socketSpawner = GetComponent<SocketSpawner>();
    }

    private void Start()
    {
        socketSpawner.SpawnNewSockets();
    }

    public void AddSpawnedSocket(Socket socket)
    {
        if (!spawnedNewSockets.Contains(socket))
        {
            spawnedNewSockets.Add(socket);
        }
    }

    public void RemoveSocketFromSpwanedList(Socket socket)
    {
        if (spawnedNewSockets.Contains(socket))
        {
            spawnedNewSockets.Remove(socket);
        }
    }

    public void RefreshSockets()
    {
        StartCoroutine(RefreshSocketsRoutine());
    }

    private IEnumerator RefreshSocketsRoutine()
    {
        foreach (Socket socket in spawnedNewSockets.ToArray())
        {
            ReturnSocketToPoolImmediately(socket);
        }

        spawnedNewSockets.Clear();
        socketSpawner.SpawnNewSockets();
        yield return null;
    }

    public void ReturnSocketToPoolImmediately(Socket socket)
    {
        if (spawnedNewSockets.Contains(socket))
        {
            RemoveSocketFromSpwanedList(socket);
            socketSpawner.ReturnSocketToPool(socket);
        }
    }

    public void ReturnSocketToPool(Socket socket)
    {
        if (spawnedNewSockets.Contains(socket))
        {
            socket.transform.DOMoveX(socket.transform.position.x - 5f, 0.2f)
                .OnComplete(() => {
                    ReturnSocketToPoolImmediately(socket);
                });
        }
    }

    public void ResetAllSockets()
    {
        foreach (Socket socket in spawnedNewSockets.ToArray())
        {
            ReturnSocketToPoolImmediately(socket);
        }
    }

    public bool CanMergeSockets(Socket socketA, Socket socketB)
    {
        if (socketA == null || socketB == null)
            return false;
        if (socketA.shapeType != socketB.shapeType)        
            return false;
        
        if (socketA.currentLevel != socketB.currentLevel)
            return false;
        
        return true;
    }

    public void TryMergeSockets(Socket socketA, Socket socketB)
    {

        if (socketA == null || socketB == null) return;
        if (socketA.socketCubes.Count != socketB.socketCubes.Count) return;
        if (socketA.ownMultiplier != socketB.ownMultiplier) return;
        if (!socketA.shapeType.Equals(socketB.shapeType)) return;


        RemoveSocketFromSpwanedList(socketA);
        RemoveSocketFromSpwanedList(socketB);
        if (activeGrids.Contains(socketA)) { 
            activeGrids.Remove(socketA);
        }
        StartCoroutine(MergeSocketsRoutine(socketA, socketB));
    }

    private IEnumerator MergeSocketsRoutine(Socket incomingSocket, Socket gridSocket)
    {

        incomingSocket.isMerging = true;
        gridSocket.isMerging = true;

        // Particle effect (optional)
        if (mergeParticlesPrefab)
        {
            Instantiate(mergeParticlesPrefab, gridSocket.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        }

        // Merge logic
        gridSocket.ownMultiplier *= 2f;
        gridSocket.currentLevel += 1;
        gridSocket.UpdateColorAndTextByLevel();

        ReturnSocketToPoolImmediately(incomingSocket);

        // Maintain Y position
        float originalSocketY = gridSocket.transform.position.y;

        if (gridSocket.assignedGrids.Count > 0)
        {
            GridObject anchorGrid = gridSocket.assignedGrids[0];
            Vector3 gridWorldPos = anchorGrid.plugSocketHolder.position;
            GameObject firstCube = gridSocket.socketCubes[0].cube.gameObject;
            Vector3 cubeOffset = firstCube.transform.position - gridSocket.transform.position;

            Vector3 targetPosition = gridWorldPos - cubeOffset;
            targetPosition.y = originalSocketY;
            gridSocket.transform.position = targetPosition;
        }
        else
        {
            Vector3 pos = gridSocket.transform.position;
            pos.y = originalSocketY;
            gridSocket.transform.position = pos;
        }

        gridSocket.GetComponent<Collider>().enabled = true;
        incomingSocket.isMerging = false;
        gridSocket.isMerging = false;
        Destroy(incomingSocket.gameObject);
        yield return null;
    }

}