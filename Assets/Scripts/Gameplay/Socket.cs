using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class Socket : MonoBehaviour
{
    #region Configuration

    [Header("Base Configuration")]
    public int currentLevel = 1;
    public SocketShapeType shapeType;
    public Transform cubesParent;

    [Header("Visuals")]
    public Color[] levelColors;

    [Header("Socket Configuration")]
    public LayerMask gridLayer;
    public LayerMask connectableLayers;
    public float ownMultiplier = 1f;
    public float actingMultiplier = 1f;
    public float pinMoveDuration = 0.3f;
    public Ease pinMoveEase = Ease.OutBack;
    public Renderer socketGFX;
    #endregion

    #region Runtime State

    public List<SocketCube> socketCubes = new();

    public bool isMerging;
    public bool hasPower;
    public List<GridObject> assignedGrids = new();
    public SocketManager socketManager;

    private TMP_Text[] fireRateTexts;
    public Transform plugHolder;
    public Plug connectedPlug;
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeSocket();
    }

    private void InitializeSocket()
    {
        AssignFireRateTexts();
        UpdateColorAndTextByLevel();
    }

    private void AssignFireRateTexts()
    {
        fireRateTexts = cubesParent.GetComponentsInChildren<TMP_Text>();
    }

    #endregion

    #region Power Handling

    public void CheckPowerActivation()
    {
        actingMultiplier = ownMultiplier;
        hasPower = false;

        foreach (var socketCube in socketCubes)
        {
            if (socketCube.pin != null)
                socketCube.pin.gameObject.SetActive(false); // Disable all first

            Ray ray = new Ray(socketCube.cube.transform.position, -Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, connectableLayers))
            {
                var powerSource = hit.collider.GetComponent<PowerSource>();
                if (powerSource != null)
                {
                    actingMultiplier += powerSource.sourcePowerMultiplier;
                    hasPower = true;

                    if (socketCube.pin != null)
                        socketCube.pin.gameObject.SetActive(true);

                    continue;
                }

                Socket otherSocket = hit.collider.GetComponent<Socket>();
                if (otherSocket != null && otherSocket.hasPower)
                {
                    actingMultiplier += otherSocket.actingMultiplier;
                    hasPower = true;

                    if (socketCube.pin != null)
                        socketCube.pin.gameObject.SetActive(true);
                }
            }
        }
    }



    #endregion


    #region Visuals

    public void UpdateColorAndTextByLevel()
    {
        UpdateFireRateDisplay();

        if (levelColors == null || levelColors.Length == 0) return;

        Color colorToApply = levelColors[Mathf.Clamp(currentLevel, 0, levelColors.Length - 1)];
        socketGFX.material.color = colorToApply;
    }

    private void UpdateFireRateDisplay()
    {
        if (fireRateTexts == null) return;

        foreach (var item in fireRateTexts)
        {
            item.text = ownMultiplier.ToString("0") + "x";
        }
    }

    #endregion

    #region Auto-Assignment (Editor-Only)

    public void AutoAssignSocketCubes()
    {
        socketCubes.Clear();
    }

    #endregion
}
[System.Serializable]
public class SocketCube
{
    public Transform cube;
    public Transform pin;
}
