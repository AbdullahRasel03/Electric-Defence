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
}
