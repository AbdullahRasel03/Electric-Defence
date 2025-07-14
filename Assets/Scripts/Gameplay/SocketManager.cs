using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Collections;

public enum SocketShapeType
{
    a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z
}

[System.Serializable]
public class SocketCube
{
    public GameObject cube;
    public Transform pin;
    [ReadOnly] public float unpluggedZ, pluggedZ;
    [ReadOnly] public bool hasPowerSource;
}
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

        if (socketA.socketCubes.Count != socketB.socketCubes.Count)
            return false;

        for (int i = 0; i < socketA.socketCubes.Count; i++)
        {
            if (socketA.socketCubes[i]?.cube == null || socketB.socketCubes[i]?.cube == null)
                return false;
        }

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
        foreach (SocketCube cube in incomingSocket.socketCubes)
        {
          if(cube.pin) cube.pin.gameObject.SetActive(false);
        }
        foreach (SocketCube cube in gridSocket.socketCubes)
        {
            if (cube.pin) cube.pin.gameObject.SetActive(false);
        }
        incomingSocket.isMerging = true;
        gridSocket.isMerging = true;

        float liftY = 0.4f;
        float alignSpacing = 1.1f;
        float zOffset = 1.2f;
        float liftDuration = 0.3f;
        float shakeDuration = 0.25f;
        float bounceDuration = 0.25f;
        float mergeDuration = 0.35f;

        // Store original socket Y position
        float originalSocketY = gridSocket.transform.position.y;

        List<Transform> cubesA = new();
        List<Transform> cubesB = new();
        List<Vector3> finalMergePoints = new();

        Vector3 center = gridSocket.transform.position;
        int count = incomingSocket.socketCubes.Count;

        // Store original values before any modifications
        List<Vector3> originalLocalPositionsB = new();
        List<Vector3> originalLocalScalesB = new();

        for (int i = 0; i < count; i++)
        {
            cubesA.Add(incomingSocket.socketCubes[i].cube.transform);
            cubesB.Add(gridSocket.socketCubes[i].cube.transform);

            // Store original values
            originalLocalPositionsB.Add(cubesB[i].localPosition);
            originalLocalScalesB.Add(cubesB[i].localScale);
        }

        // Step 1: Lift and align
        for (int i = 0; i < count; i++)
        {
            float xOffset = (i - (count - 1) / 2f) * alignSpacing;
            Vector3 targetA = center + new Vector3(xOffset, liftY, 0);
            Vector3 targetB = center + new Vector3(xOffset, liftY, -zOffset);

            cubesA[i].DOMove(targetA, liftDuration).SetEase(Ease.OutBack);
            cubesB[i].DOMove(targetB, liftDuration).SetEase(Ease.OutBack);
            finalMergePoints.Add(center + new Vector3(xOffset, liftY, -zOffset * 0.5f));
        }

        yield return new WaitForSeconds(liftDuration + 0.05f);

        // Step 2: Bounce away briefly
        for (int i = 0; i < count; i++)
        {
            Vector3 awayA = cubesA[i].position + Vector3.forward * 0.5f;
            Vector3 awayB = cubesB[i].position;

            cubesA[i].DOMove(awayA, bounceDuration / 2f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
            cubesB[i].DOMove(awayB, bounceDuration / 2f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);

            cubesA[i].DOScale(cubesA[i].localScale * 1.15f, bounceDuration / 2f).SetLoops(2, LoopType.Yoyo);
            cubesB[i].DOScale(cubesB[i].localScale * 1.15f, bounceDuration / 2f).SetLoops(2, LoopType.Yoyo);
        }

        yield return new WaitForSeconds(bounceDuration + 0.1f);

        // Step 3: Merge with twist and squash
        for (int i = 0; i < count; i++)
        {
            cubesA[i].DOMove(finalMergePoints[i], mergeDuration).SetEase(Ease.InBack);
            cubesB[i].DOMove(finalMergePoints[i], mergeDuration).SetEase(Ease.InBack);

            cubesA[i].DORotate(Vector3.up * 360, mergeDuration, RotateMode.FastBeyond360);
            cubesB[i].DORotate(Vector3.up * -360, mergeDuration, RotateMode.FastBeyond360);

            cubesA[i].DOScale(cubesA[i].localScale * 0.8f, mergeDuration / 2f).SetLoops(2, LoopType.Yoyo);
            cubesB[i].DOScale(cubesB[i].localScale * 0.8f, mergeDuration / 2f).SetLoops(2, LoopType.Yoyo);
        }

        yield return new WaitForSeconds(mergeDuration + 0.1f);

        // Step 4: Particle burst and cleanup
        for (int i = 0; i < count; i++)
        {
            if (mergeParticlesPrefab)
            {
                Instantiate(mergeParticlesPrefab, finalMergePoints[i] + Vector3.up * 0.1f, Quaternion.identity);
            }

          //  Destroy(cubesA[i].gameObject);
        }

        // Step 5: Power up grid socket
        gridSocket.ownMultiplier *= 2f;
        gridSocket.currentLevel += 1;
        gridSocket.UpdateColorAndTextByLevel();

        // Return incoming socket
        ReturnSocketToPoolImmediately(incomingSocket);

        // Restore grid socket cube properties
        for (int i = 0; i < cubesB.Count; i++)
        {
            // Restore local position and scale
            cubesB[i].localPosition = originalLocalPositionsB[i];
            cubesB[i].localScale = originalLocalScalesB[i];
            cubesB[i].localRotation = Quaternion.identity;
        }

        // Re-align grid socket position while maintaining original Y position
        if (gridSocket.assignedGrids.Count > 0)
        {
            GridObject anchorGrid = gridSocket.assignedGrids[0];
            Vector3 gridWorldPos = anchorGrid.plugSocketHolder.position;
            GameObject firstCube = gridSocket.socketCubes[0].cube;
            Vector3 cubeOffset = firstCube.transform.position - gridSocket.transform.position;

            // Calculate new position with original Y
            Vector3 targetPosition = gridWorldPos - cubeOffset;
            targetPosition.y = originalSocketY;

            gridSocket.transform.DOMove(targetPosition, 0.25f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    // Final verification
                    Vector3 finalPos = gridSocket.transform.position;
                    finalPos.y = originalSocketY;
                    gridSocket.transform.position = finalPos;
                });
        }
        else
        {
            // Just maintain Y position if not assigned to grid
            Vector3 pos = gridSocket.transform.position;
            pos.y = originalSocketY;
            gridSocket.transform.position = pos;
        }

        gridSocket.GetComponent<Collider>().enabled = true;
        incomingSocket.isMerging = false;
        gridSocket.isMerging = false;
        Destroy(incomingSocket.gameObject);
        foreach (SocketCube cube in gridSocket.socketCubes)
        {
            if (cube.pin) cube.pin.gameObject.SetActive(true);
        }
    }
}