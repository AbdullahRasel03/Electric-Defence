using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketSpawner : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private List<SocketPrefabWeight> weightedSocketPrefabs = new();
    [SerializeField] private Transform[] newSocketSpawnTransforms;
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private float delayBetweenSockets = 0.1f;
    [SerializeField] private float sameShapeSpawnChance = 0.7f; // 70% chance to spawn same shape

    private SocketManager socketManager;

    private void Awake()
    {
        socketManager = GetComponent<SocketManager>();
    }

    public void SpawnNewSockets()
    {
        if (newSocketSpawnTransforms.Length == 0) return;
        StartCoroutine(SlideInSockets());
    }

    private IEnumerator SlideInSockets()
    {
        foreach (Transform spawnPoint in newSocketSpawnTransforms)
        {
            Socket prefab = GetPrioritizedSocketPrefab();
            GameObject socketObj = ObjectPool.instance.GetObject(
                prefab.gameObject,
                true,
                spawnPoint.position + Vector3.left * 5f,
                Quaternion.identity
            );

            Socket socket = socketObj.GetComponent<Socket>();
            socketManager.AddSpawnedSocket(socket);
            socket.socketManager = socketManager;

            socketObj.transform.DOMove(spawnPoint.position, slideInDuration);
            yield return new WaitForSeconds(delayBetweenSockets);
        }
    }

    private Socket GetPrioritizedSocketPrefab()
    {
        // If there are active sockets, prioritize their shapes
        if (socketManager.activeGrids.Count > 0 && Random.value <= sameShapeSpawnChance)
        {
            // Get a random active socket to match its shape
            Socket randomActiveSocket = socketManager.activeGrids[Random.Range(0, socketManager.activeGrids.Count)];
            SocketShapeType shapeToMatch = randomActiveSocket.shapeType;
            int levelToMatch = randomActiveSocket.currentLevel;

            // Try to find matching shape and level
            List<SocketPrefabWeight> matchingSockets = weightedSocketPrefabs.FindAll(x =>
                x.prefab.shapeType == shapeToMatch &&
                x.prefab.currentLevel == levelToMatch);

            if (matchingSockets.Count > 0)
            {
                return GetWeightedRandomFromList(matchingSockets);
            }

            // If no exact level match, try just matching shape
            matchingSockets = weightedSocketPrefabs.FindAll(x => x.prefab.shapeType == shapeToMatch);
            if (matchingSockets.Count > 0)
            {
                return GetWeightedRandomFromList(matchingSockets);
            }
        }

        // Fallback to completely random if no matches or chance fails
        return GetWeightedRandomSocketPrefab();
    }

    private Socket GetWeightedRandomFromList(List<SocketPrefabWeight> socketList)
    {
        int totalWeight = 0;
        foreach (var entry in socketList)
        {
            totalWeight += entry.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var entry in socketList)
        {
            currentWeight += entry.weight;
            if (randomWeight < currentWeight)
            {
                return entry.prefab;
            }
        }

        return socketList[0].prefab;
    }

    private Socket GetWeightedRandomSocketPrefab()
    {
        return GetWeightedRandomFromList(weightedSocketPrefabs);
    }

    public void ReturnSocketToPool(Socket socket)
    {
        if (socket != null && socket.gameObject != null)
        {
            ObjectPool.instance.ReturnToPool(socket.gameObject);
        }
    }
}