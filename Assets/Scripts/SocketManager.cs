using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SocketManager : MonoBehaviour
{
    [Header("Socket Settings")]
    public SocketBehaviour[] socketPrefabs;
    public Transform[] newSocketSpawnTransforms;
    public int initialPoolSize = 5;
    public float slideInDuration = 0.5f;
    public float delayBetweenSockets = 0.1f;

    [Header("Socket Tracking")]
    [SerializeField] private List<SocketBehaviour> availableSockets = new List<SocketBehaviour>();
    [SerializeField] private List<SocketBehaviour> activeSockets = new List<SocketBehaviour>();

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
        // Immediately return all sockets to pool without animation
        foreach (SocketBehaviour socket in activeSockets.ToArray())
        {
            ReturnSocketToPoolImmediately(socket);
        }

        // No need to wait since there's no animation
        StartCoroutine(SlideInSockets());
        yield return null;
    }

    private void InitializeSocketPool()
    {
        for (int i = 0; i < initialPoolSize * socketPrefabs.Length; i++)
        {
            SocketBehaviour randomPrefab = socketPrefabs[Random.Range(0, socketPrefabs.Length)];
            CreateNewSocket(randomPrefab);
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
            SocketBehaviour randomPrefab = socketPrefabs[Random.Range(0, socketPrefabs.Length)];
            SocketBehaviour socket = GetAvailableSocket(randomPrefab);

            socket.transform.position = spawnPoint.position + (Vector3.left * 5f);
            socket.gameObject.SetActive(true);
            activeSockets.Add(socket);

            socket.transform.DOMove(spawnPoint.position, slideInDuration);
            yield return new WaitForSeconds(delayBetweenSockets);
        }
    }

    private SocketBehaviour GetAvailableSocket(SocketBehaviour preferredPrefab = null)
    {
        if (preferredPrefab != null)
        {
            for (int i = 0; i < availableSockets.Count; i++)
            {
                if (availableSockets[i].name.StartsWith(preferredPrefab.name))
                {
                    SocketBehaviour socket = availableSockets[i];
                    availableSockets.RemoveAt(i);
                    return socket;
                }
            }
        }

        if (availableSockets.Count > 0)
        {
            SocketBehaviour socket = availableSockets[0];
            availableSockets.RemoveAt(0);
            return socket;
        }

        SocketBehaviour prefabToUse = preferredPrefab != null ?
            preferredPrefab :
            socketPrefabs[Random.Range(0, socketPrefabs.Length)];

        return CreateNewSocket(prefabToUse);
    }

    private SocketBehaviour CreateNewSocket(SocketBehaviour prefab)
    {
        SocketBehaviour socket = Instantiate(prefab);
        socket.name = prefab.name;
        socket.gameObject.SetActive(false);
        socket.SetSocketManager(this);
        availableSockets.Add(socket);
        return socket;
    }

    // New method for immediate return to pool
    public void ReturnSocketToPoolImmediately(SocketBehaviour socket)
    {
        if (activeSockets.Contains(socket))
        {
            activeSockets.Remove(socket);
            socket.gameObject.SetActive(false);
            availableSockets.Add(socket);
        }
    }

    // Original method kept in case you want slide-out in other situations
    public void ReturnSocketToPool(SocketBehaviour socket)
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
        foreach (SocketBehaviour socket in activeSockets.ToArray())
        {
            ReturnSocketToPoolImmediately(socket);
        }
    }
}