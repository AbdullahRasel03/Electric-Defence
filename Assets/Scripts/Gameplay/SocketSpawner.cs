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
    [SerializeField] private float sameShapeSpawnChance = 0.7f;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer conveyorBelt;
    [SerializeField] private float beltScrollSpeed = 0.5f;

    private SocketManager socketManager;
    private List<Socket> selectedSocketPool = new();
    private Material beltMat;

    private void Awake()
    {
        socketManager = GetComponent<SocketManager>();
        beltMat = conveyorBelt.material;
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

        float totalDuration = (newSocketSpawnTransforms.Length - 1) * delayBetweenSockets + slideInDuration;
        ScrollBelt(totalDuration);
        SlideInSockets();
    }

    private void SlideInSockets()
    {
        for (int i = 0; i < newSocketSpawnTransforms.Length; i++)
        {
            Transform spawnPoint = newSocketSpawnTransforms[i];
            Socket prefab = GetPrioritizedSocketPrefab();

            Vector3 offset = Vector3.left * 30f;
            float spacingOffset = i * delayBetweenSockets * (30f / slideInDuration); // calculates equal spacing based on speed
            Vector3 spawnPos = spawnPoint.position + offset - Vector3.left * spacingOffset;

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

    public void ScrollBelt(float duration)
    {
        float scrollAmount = beltScrollSpeed * duration;
        Vector2 startOffset = beltMat.mainTextureOffset;
        Vector2 targetOffset = new Vector2(startOffset.x + scrollAmount, 1.3f);

        DOTween.To(() => beltMat.mainTextureOffset, x => beltMat.mainTextureOffset = x, targetOffset, duration).SetEase(Ease.Linear);
    }

    private Socket GetPrioritizedSocketPrefab()
    {
        if (socketManager.activeGrids.Count > 0 && Random.value <= sameShapeSpawnChance)
        {
            Socket randomActiveSocket = socketManager.activeGrids[Random.Range(0, socketManager.activeGrids.Count)];
            SocketShapeType shapeToMatch = randomActiveSocket.shapeType;
            int levelToMatch = randomActiveSocket.currentLevel;

            List<Socket> matchingSockets = selectedSocketPool.FindAll(x =>
                x.shapeType == shapeToMatch && x.currentLevel == levelToMatch);

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
