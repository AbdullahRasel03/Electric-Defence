using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Collections;
using UnityEngine;

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

    [SerializeField] private GameObject mergeParticlesPrefab;
    private SocketSpawner socketSpawner;
    [SerializeField] GridManager gridManager;
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
        print("11111");
        StartCoroutine(RefreshSocketsRoutine());
    }

    private IEnumerator RefreshSocketsRoutine()
    {
        float spacing = 1f; // or match spacing from SlideInSockets
        float moveDuration = 0.3f;
        float totalScrollDuration = socketSpawner.slideInDuration;

        socketSpawner.ScrollBelt(totalScrollDuration);

        for (int i = 0; i < spawnedNewSockets.Count; i++)
        {
            Socket socket = spawnedNewSockets[i];
            float offsetX = i * spacing;

            socket.transform
                .DOMoveX(socket.transform.position.x + 30f + offsetX, socketSpawner.slideInDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ReturnSocketToPoolImmediately(socket);
                });
        }

        spawnedNewSockets.Clear();

        yield return new WaitForSeconds(moveDuration + 0.05f);
        socketSpawner.SpawnNewSockets();
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
                .OnComplete(() =>
                {
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
        if (activeGrids.Contains(socketA))
        {
            activeGrids.Remove(socketA);
        }
        StartCoroutine(MergeSocketsRoutine(socketA, socketB));
    }

    private IEnumerator MergeSocketsRoutine(Socket incomingSocket, Socket gridSocket)
    {

        incomingSocket.isMerging = true;
        gridSocket.isMerging = true;


        // gridSocket.transform.localScale = Vector3.one;
        // Particle effect (optional)
        if (mergeParticlesPrefab)
        {
            GameObject mergeParticle = Instantiate(mergeParticlesPrefab, gridSocket.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(mergeParticle, 2);
        }

        // Merge logic
        gridSocket.ownMultiplier *= 2f;
        gridSocket.currentLevel += 1;
        gridSocket.UpdateColorAndTextByLevel();
        gridSocket.Upgrade();
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
        gridManager.CheckAllGridsPower();
        gridSocket.UpdateColorAndTextByLevel();
        yield return null;
    }

}