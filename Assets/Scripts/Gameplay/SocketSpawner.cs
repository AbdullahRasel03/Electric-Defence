using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SocketSpawner : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private List<Socket> allSocketPrefabs = new();
    [SerializeField] private Transform[] newSocketSpawnTransforms;
    [SerializeField] public float slideInDuration = 1f;
    [SerializeField] private float delayBetweenSockets = 0.1f;
    [SerializeField] private int socketsPerBatch = 3; // How many sockets to spawn per refresh

    [Header("Visual Feedback")]
    [SerializeField] private Renderer conveyorBelt;
    [SerializeField] private float beltScrollSpeed = 0.5f;

    private SocketManager socketManager;
    private Material beltMat;
    private int currentSerialIndex = 0; // Tracks position in the prefab list

    private void Awake()
    {
        socketManager = GetComponent<SocketManager>();
        beltMat = conveyorBelt.material;
    }

    public void SpawnNewSockets()
    {
        if (newSocketSpawnTransforms.Length == 0 || allSocketPrefabs.Count == 0) return;

        float totalDuration = (socketsPerBatch - 1) * delayBetweenSockets + slideInDuration;
        ScrollBelt(totalDuration);
        SlideInSockets();
    }

    private void SlideInSockets()
    {
        int spawnCount = Mathf.Min(socketsPerBatch, newSocketSpawnTransforms.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPoint = newSocketSpawnTransforms[i];
            Socket prefab = GetNextSocketInSerial();
            if (prefab == null) continue;

            // Calculate start position and offset
            Vector3 offset = Vector3.left * 30f;
            float spacingOffset = i * delayBetweenSockets * (30f / slideInDuration);
            Vector3 spawnPos = spawnPoint.position + offset - Vector3.left * spacingOffset;

            // Spawn socket from pool
            GameObject socketObj = ObjectPool.instance.GetObject(
                prefab.gameObject,
                true,
                spawnPos,
                Quaternion.identity
            );

            Socket socket = socketObj.GetComponent<Socket>();
            socketManager.AddSpawnedSocket(socket);
            socket.socketManager = socketManager;

            socketObj.transform.parent = spawnPoint;
            socketObj.transform.localScale = Vector3.one * 1.4f;
            socketObj.transform.DOLocalMove(Vector3.zero, slideInDuration).SetEase(Ease.Linear);
        }
    }

    private Socket GetNextSocketInSerial()
    {
        if (allSocketPrefabs.Count == 0)
            return null;

        Socket socketToSpawn = allSocketPrefabs[currentSerialIndex];
        currentSerialIndex = (currentSerialIndex + 1) % allSocketPrefabs.Count; // Loop around
        return socketToSpawn;
    }

    public void ScrollBelt(float duration)
    {
        float scrollAmount = beltScrollSpeed * duration;
        Vector2 startOffset = beltMat.mainTextureOffset;
        Vector2 targetOffset = new Vector2(startOffset.x + scrollAmount, 1.3f);

        DOTween.To(() => beltMat.mainTextureOffset, x => beltMat.mainTextureOffset = x, targetOffset, duration)
            .SetEase(Ease.Linear);
    }

    public void ReturnSocketToPool(Socket socket)
    {
        if (socket != null && socket.gameObject != null)
        {
            ObjectPool.instance.ReturnToPool(socket.gameObject);
        }
    }
}
