using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using MasterFX;

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
    public Renderer gfx;
    public GameObject multiText;
    #endregion

    #region Runtime State

    public List<SocketCube> socketCubes = new();

    public bool isMerging;
    public bool hasPower;
    public List<GridObject> assignedGrids = new();
    public SocketManager socketManager;

    public TMP_Text[] fireRateTexts;
    public Transform plugHolder;
    public Plug connectedPlug;
    #endregion


    public MLaser laser;

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

    public void AssignFireRateTexts()
    {
       // fireRateTexts = cubesParent.GetComponentsInChildren<TMP_Text>();
    }

    #endregion

    #region Power Handling
    public void PowerUp()
    {
        if (gfx == null || gfx.material == null || hasPower) return;
       // multiText.SetActive(false);
        hasPower = true;
        Material mat = gfx.materials[1];
        Color currentEmission = mat.GetColor("_Emissive");
        Color targetEmission = currentEmission + Color.cyan * 10f;
        DOTween.To(() => currentEmission, x => {
            currentEmission = x;
            mat.SetColor("_Emissive", currentEmission);
        }, targetEmission, 1f);
    }

    #endregion


    #region Visuals

    public void UpdateColorAndTextByLevel()
    {
        UpdateFireRateDisplay();

        if (levelColors == null || levelColors.Length == 0) return;

        Color colorToApply = levelColors[Mathf.Clamp(currentLevel, 0, levelColors.Length - 1)];
        gfx.materials[0].color = colorToApply;
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


    public void Upgrade()
    {
        laser.UpgradeLaser();
    }

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
