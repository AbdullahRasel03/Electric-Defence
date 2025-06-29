using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SocketManager : MonoBehaviour
{
    [System.Serializable]
    public struct SocketPrefabWeight
    {
        public Socket prefab;
        public int weight;
    }

    [Header("Socket Settings")]
    [SerializeField] private List<SocketPrefabWeight> weightedSocketPrefabs = new();
    [SerializeField] private Transform[] newSocketSpawnTransforms;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private float delayBetweenSockets = 0.1f;

    [Header("Socket Tracking")]
    [SerializeField] private List<Socket> availableSockets = new();
    [SerializeField] private List<Socket> activeSockets = new();

    [SerializeField] private ParticleSystem mergeParticlesPrefab;
    private void Start()
    {
        InitializeSocketPool();
        SpawnInitialSockets();
    }

    public void RefreshSockets()
    {
        StartCoroutine(RefreshSocketsRoutine());
    }

    private IEnumerator RefreshSocketsRoutine()
    {
        foreach (Socket socket in activeSockets.ToArray())
        {
            ReturnSocketToPoolImmediately(socket);
        }

        StartCoroutine(SlideInSockets());
        yield return null;
    }

    private void InitializeSocketPool()
    {
        for (int i = 0; i < initialPoolSize * weightedSocketPrefabs.Count; i++)
        {
            Socket prefab = GetWeightedRandomSocketPrefab();
            CreateNewSocket(prefab);
        }
    }

    private void SpawnInitialSockets()
    {
        if (newSocketSpawnTransforms.Length == 0) return;
        StartCoroutine(SlideInSockets());
    }

    private IEnumerator SlideInSockets()
    {
        foreach (Transform spawnPoint in newSocketSpawnTransforms)
        {
            Socket prefab = GetWeightedRandomSocketPrefab();
            Socket socket = GetAvailableSocket(prefab);

            socket.transform.position = spawnPoint.position + Vector3.left * 5f;
            socket.gameObject.SetActive(true);
            activeSockets.Add(socket);
            socket.socketManager = this;
            socket.transform.DOMove(spawnPoint.position, slideInDuration);
            yield return new WaitForSeconds(delayBetweenSockets);
        }
    }

    private Socket GetAvailableSocket(Socket preferredPrefab = null)
    {
        if (preferredPrefab != null)
        {
            for (int i = 0; i < availableSockets.Count; i++)
            {
                if (availableSockets[i].name.StartsWith(preferredPrefab.name))
                {
                    Socket socket = availableSockets[i];
                    availableSockets.RemoveAt(i);
                    return socket;
                }
            }
        }

        if (availableSockets.Count > 0)
        {
            Socket socket = availableSockets[0];
            availableSockets.RemoveAt(0);
            return socket;
        }

        Socket fallbackPrefab = preferredPrefab != null ? preferredPrefab : GetWeightedRandomSocketPrefab();
        return CreateNewSocket(fallbackPrefab);
    }

    private Socket CreateNewSocket(Socket prefab)
    {
        Socket socket = Instantiate(prefab);
        socket.name = prefab.name;
        socket.gameObject.SetActive(false);
        availableSockets.Add(socket);
        return socket;
    }

    private Socket GetWeightedRandomSocketPrefab()
    {
        int totalWeight = 0;
        foreach (var entry in weightedSocketPrefabs)
        {
            totalWeight += entry.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var entry in weightedSocketPrefabs)
        {
            currentWeight += entry.weight;
            if (randomWeight < currentWeight)
            {
                return entry.prefab;
            }
        }

        return weightedSocketPrefabs[0].prefab;
    }

    public void ReturnSocketToPoolImmediately(Socket socket)
    {
        if (activeSockets.Contains(socket))
        {
            activeSockets.Remove(socket);
            socket.gameObject.SetActive(false);
            availableSockets.Add(socket);
        }
    }

    public void ReturnSocketToPool(Socket socket)
    {
        if (activeSockets.Contains(socket))
        {
            socket.transform.DOMoveX(socket.transform.position.x - 5f, slideInDuration)
                .OnComplete(() => {
                    ReturnSocketToPoolImmediately(socket);
                });
        }
    }

    public void ResetAllSockets()
    {
        foreach (Socket socket in activeSockets.ToArray())
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
        if (Mathf.Abs(socketA.ownMultiplier - socketB.ownMultiplier) > 0.01f) return;

        for (int i = 0; i < socketA.socketCubes.Count; i++)
        {
            Vector3 localA = socketA.socketCubes[i].cube.transform.localPosition;
            Vector3 localB = socketB.socketCubes[i].cube.transform.localPosition;

            if (Vector3.Distance(localA, localB) > 0.01f)
                return;
        }

        StartCoroutine(MergeSocketsRoutine(socketA, socketB));
    }
    private IEnumerator MergeSocketsRoutine(Socket incomingSocket, Socket gridSocket)
    {
        incomingSocket.pins.SetActive(false);
        incomingSocket.isMerging = true;
        gridSocket.isMerging = true;

        float liftY = 0.4f;
        float alignSpacing = 1.1f;
        float zOffset = 1.2f;
        float liftDuration = 0.3f;
        float shakeDuration = 0.25f;
        float bounceDuration = 0.25f;
        float mergeDuration = 0.35f;

        List<Transform> cubesA = new();
        List<Transform> cubesB = new();

        List<Vector3> finalMergePoints = new();

        Vector3 center = gridSocket.transform.position;
        int count = incomingSocket.socketCubes.Count;

        for (int i = 0; i < count; i++)
        {
            cubesA.Add(incomingSocket.socketCubes[i].cube.transform);
            cubesB.Add(gridSocket.socketCubes[i].cube.transform);
        }
        List<Vector3> originalLocalPositionsB = new();
        for (int i = 0; i < cubesB.Count; i++)
        {
            originalLocalPositionsB.Add(cubesB[i].localPosition);
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

        // Step 2: Bounce away briefly (like ping pong)
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

            Destroy(cubesA[i].gameObject);
        }

        // Step 5: Power up grid socket
        gridSocket.ownMultiplier *= 2f;
        gridSocket.currentLevel += 1;
        gridSocket.UpdateColorAndTextByLevel();

        // Return incoming socket
        ReturnSocketToPoolImmediately(incomingSocket);

        // Restore grid socket cube local positions
        for (int i = 0; i < cubesB.Count; i++)
        {
            cubesB[i].localPosition = originalLocalPositionsB[i];
            cubesB[i].localRotation = Quaternion.identity;
            cubesB[i].localScale = Vector3.one;
        }

        // Re-align grid socket position
        if (gridSocket.assignedGrids.Count > 0)
        {
            GridObject anchorGrid = gridSocket.assignedGrids[0];
            Vector3 gridWorldPos = anchorGrid.plugSocketHolder.position;
            GameObject firstCube = gridSocket.socketCubes[0].cube;
            Vector3 cubeOffset = firstCube.transform.position - gridSocket.transform.position;

            gridSocket.transform.DOMove(gridWorldPos - cubeOffset, 0.25f).SetEase(Ease.OutBack);
        }
        for (int i = 0; i < cubesB.Count; i++)
        {
            cubesB[i].localPosition = originalLocalPositionsB[i];
            cubesB[i].localRotation = Quaternion.identity;
            cubesB[i].localScale = Vector3.one;
        }
        gridSocket.GetComponent<Collider>().enabled = true;
        incomingSocket.isMerging = false;
        gridSocket.isMerging = false;


    }


}
