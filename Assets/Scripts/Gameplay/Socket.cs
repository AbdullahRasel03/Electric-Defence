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
    public Renderer[] gfxs;
    public GameObject multiText;

    [Header("Socket Configuration")]
    public LayerMask gridLayer;
    public LayerMask connectableLayers;
    public float ownMultiplier = 1f;
    public float actingMultiplier = 1f;
    public float pinMoveDuration = 0.3f;
    public Ease pinMoveEase = Ease.OutBack;

    #endregion

    #region Runtime State

    public List<SocketCube> socketCubes = new();
    public bool isMerging;
    public bool hasPower;
    public List<GridObject> assignedGrids = new();
    public SocketManager socketManager;
    public TMP_Text[] fireRateTexts;
    public Transform plugHolder; // Optional: remove if not used elsewhere
    public MLaser laser;
    public GameObject MulTxt => multiText;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeSocket();
        EnemySpawner.OnSpawnStarted += RotateMultiplier;
    }

    private void OnDestroy()
    {
        EnemySpawner.OnSpawnStarted -= RotateMultiplier;
    }

    private void RotateMultiplier()
    {
        if(multiText == null) return;
        multiText.transform.DOLocalRotateQuaternion(Quaternion.Euler(55f, 0f, 0f), 1.5f);
    }

    private void InitializeSocket()
    {
        AssignFireRateTexts();
        UpdateFireRateDisplay();
    }

    public void AssignFireRateTexts()
    {
        // fireRateTexts = cubesParent.GetComponentsInChildren<TMP_Text>();
    }

    #endregion

    #region Power Handling

    public void PowerUp()
    {
        if (gfxs == null || gfxs.Length == 0 || hasPower) return;

        hasPower = true;

        foreach (var renderer in gfxs)
        {
            if (renderer.material.HasProperty("_Emissive"))
            {
                Color currentEmission = renderer.material.GetColor("_Emissive");
                Color targetEmission = currentEmission + Color.cyan * 10f;

                DOTween.To(() => currentEmission, x =>
                {
                    renderer.material.SetColor("_Emissive", x);
                }, targetEmission, 1f).SetId(this);
            }
        }

        if (laser != null)
        {
            laser.gameObject.SetActive(true);
        }
    }

    public void PowerDown()
    {
        if (gfxs == null || gfxs.Length == 0 || !hasPower) return;

        hasPower = false;

        DOTween.Kill(this);

        foreach (var renderer in gfxs)
        {
            if (renderer.material.HasProperty("_Emissive"))
            {
                Color currentEmission = renderer.material.GetColor("_Emissive");
                Color targetEmission = currentEmission - Color.cyan * 10f;

                DOTween.To(() => currentEmission, x =>
                {
                    renderer.material.SetColor("_Emissive", x);
                }, targetEmission, 1f);
            }
        }

        if (laser != null)
        {
            laser.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Visuals

    public void UpdateFireRateDisplay()
    {
        if (fireRateTexts.Length <= 0) return;

        foreach (var item in fireRateTexts)
        {
            item.text = ownMultiplier.ToString("0") + "x";
        }
    }

    #endregion

    public void Upgrade()
    {
        // laser.UpgradeLaser();
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
