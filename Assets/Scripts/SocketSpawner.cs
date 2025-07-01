using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketSpawner : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private List<Socket> allSocketPrefabs = new(); // Direct Socket reference
    [SerializeField] private Transform[] newSocketSpawnTransforms;
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private float delayBetweenSockets = 0.1f;
    [SerializeField] private float sameShapeSpawnChance = 0.7f;

    private SocketManager socketManager;

    private List<Socket> selectedSocketPool = new();

    private void Awake()
    {
        socketManager = GetComponent<SocketManager>();
        PickRandomSocketPool();
    }

    private void PickRandomSocketPool()
    {
        selectedSocketPool.Clear();

        List<Socket> cloneList = new(allSocketPrefabs);

        for (int i = 0; i < 5 && cloneList.Count > 0; i++)
        {
            int randIndex = Random.Range(0, cloneList.Count);
            selectedSocketPool.Add(cloneList[randIndex]);
            cloneList.RemoveAt(randIndex);
        }
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
        if (socketManager.activeGrids.Count > 0 && Random.value <= sameShapeSpawnChance)
        {
            Socket randomActiveSocket = socketManager.activeGrids[Random.Range(0, socketManager.activeGrids.Count)];
            SocketShapeType shapeToMatch = randomActiveSocket.shapeType;
            int levelToMatch = randomActiveSocket.currentLevel;

            List<Socket> matchingSockets = selectedSocketPool.FindAll(x =>
                x.shapeType == shapeToMatch &&
                x.currentLevel == levelToMatch);

            if (matchingSockets.Count > 0)
                return GetRandomSocket(matchingSockets);

            matchingSockets = selectedSocketPool.FindAll(x => x.shapeType == shapeToMatch);
            if (matchingSockets.Count > 0)
                return GetRandomSocket(matchingSockets);
        }

        return GetRandomSocket(selectedSocketPool);
    }

    private Socket GetRandomSocket(List<Socket> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public void ReturnSocketToPool(Socket socket)
    {
        if (socket != null && socket.gameObject != null)
        {
            ObjectPool.instance.ReturnToPool(socket.gameObject);
        }
    }
}
