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
            Socket prefab = GetWeightedRandomSocketPrefab();
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

    public void ReturnSocketToPool(Socket socket)
    {
        if (socket != null && socket.gameObject != null)
        {
            ObjectPool.instance.ReturnToPool(socket.gameObject);
        }
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
}