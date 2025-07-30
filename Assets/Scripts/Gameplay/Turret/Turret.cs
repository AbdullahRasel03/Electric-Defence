using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [SerializeField] bool reflectorActive;
    [SerializeField] protected Transform turretBody;
    [SerializeField] protected float range = 18f;
    [SerializeField] protected float rotationSpeed = 5f;
    // [SerializeField] protected float fireDelay = 1f;
    [SerializeField] private TMP_Text fireRateText;
    public GridObject[] gridsOnPath;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected bool isActive = false;

    protected Enemy currentTarget;
    protected float refreshCooldown = 0.2f;
    protected float timer;

    [SerializeField] Renderer towerGFX;

    private bool wasPoweredThisFrame = false;
    [SerializeField] private float baseFireRate = 1f;

    [Space(15)]
    [SerializeField] private Canvas sliderCanvas;
    [SerializeField] private Slider fireRateSlider;

    protected Sequence fireSequence;

    protected float fireTime = 0f;

    void Start()
    {
        timer = refreshCooldown;

        if (!reflectorActive)
        {
            UpdateFireRateText();
        }
    }

    private void UpdateFireRateText()
    {
        if (fireRateText != null)
        {
            fireRateText.text = (1f / fireRate).ToString("F2") + "/s";
        }
        // UpdateFireRateText("--");
    }

    void Update()
    {
        if (!isActive) return;

        AssignClosestEnemy();
        RotateYAxisToTarget();
        Fire();
    }

    public void Activate()
    {
        isActive = true;

        if (towerGFX == null || towerGFX.material == null) return;

        Material mat = towerGFX.materials[0];
        Color currentEmission = mat.GetColor("_Emissive");
        Color targetEmission = currentEmission + Color.cyan * 10f;

        DOTween.To(() => currentEmission, x =>
        {
            currentEmission = x;
            mat.SetColor("_Emissive", currentEmission);
        }, targetEmission, 1f);
    }

    public void Deactivate()
    {
        if (!isActive) return;
        if (!reflectorActive) return;

        isActive = false;
        currentTarget = null;

        if (towerGFX != null && towerGFX.materials.Length > 0)
        {
            Material mat = towerGFX.materials[0];
            Color currentEmission = mat.GetColor("_Emissive");
            Color targetEmission = currentEmission - Color.cyan * 10f;

            DOTween.Kill(mat);
            DOTween.To(() => currentEmission, x =>
            {
                mat.SetColor("_Emissive", x);
            }, targetEmission, 1f);
        }

        SetFireRate(baseFireRate);
        UpdateFireRateText("--");
    }

    public void ToggleActive()
    {
        if (isActive)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    protected virtual void AssignClosestEnemy()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            float closestDist = range;
            Enemy best = null;

            foreach (Enemy enemy in EnemyManager.Instance.ActiveEnemies)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= range && dist < closestDist)
                {
                    closestDist = dist;
                    best = enemy;
                }
            }
            currentTarget = best;
            timer = refreshCooldown;
        }
    }

    protected void RotateYAxisToTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.transform.position - turretBody.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            turretBody.rotation = Quaternion.Slerp(turretBody.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    protected virtual void Fire()
    {
        // Implement firing logic in derived classes
        if (currentTarget == null) return;

    }

    public void CheckMultisOnPath()
    {
        if (reflectorActive)
        {
            return;
        }

        // Debug.LogError("Call");
        // float fireRate = 1;
        float currentFireDelay = fireRate;
        foreach (GridObject item in gridsOnPath)
        {
            if (item.socket)
            {
                currentFireDelay -= item.socket.ownMultiplier / 50f;
                item.socket.PowerUp();
            }
        }

        fireRate = currentFireDelay;

        UpdateFireRateText();

        // shooter.SetFireRate(fireRate);
        // powerText.text = shooter.GetFireRate().ToString();
    }

    public void HideFireRateText()
    {
        fireRateText.transform.parent.gameObject.SetActive(false);

        // if (fireRateText != null)
        // {
        //     fireRateText.transform.parent.DOLocalRotateQuaternion(Quaternion.Euler(-75f, 0f, 0f), 1.5f);
        // }
    }

    public void SetFireRateSlider()
    {
        sliderCanvas.gameObject.SetActive(true);
        sliderCanvas.transform.DORotateQuaternion(Quaternion.Euler(20f, 0f, 90f), 1.5f);
        fireRateSlider.maxValue = fireRate;
        fireRateSlider.value = 0;
        fireTime = 0f;
    }

    public void ReceivePower(float totalMultiplier)
    {
        wasPoweredThisFrame = true;

        if (!isActive)
        {
            Activate();
        }

        float newRate = baseFireRate * Mathf.Max(1f, totalMultiplier);
        SetFireRate(newRate);
    }

    void LateUpdate()
    {
        if (!wasPoweredThisFrame && isActive && reflectorActive)
        {
            Deactivate();
        }

        wasPoweredThisFrame = false;
    }

    // === New Methods ===

    private void SetFireRate(float rate)
    {
        fireRate = rate;
        UpdateFireRateText();
    }

    private void UpdateFireRateText(string text)
    {
        if (fireRateText != null)
        {
            fireRateText.text = text;
        }
    }

    protected void SetFireRateSlider(float value)
    {
        if (fireRateSlider != null)
        {
            DOTween.To(() => fireRateSlider.value, x => fireRateSlider.value = x, value, 0.1f)
                .SetEase(Ease.OutCubic);
        }
    }
}
