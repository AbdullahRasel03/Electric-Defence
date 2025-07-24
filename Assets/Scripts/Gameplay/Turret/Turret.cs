using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] protected Transform turretBody;
    [SerializeField] protected float range = 18f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected bool isActive = false;

    protected Enemy currentTarget;
    protected float refreshCooldown = 0.2f;
    protected float timer;

    [SerializeField] Renderer towerGFX;
    [SerializeField] TMP_Text fireRateText;

    private bool wasPoweredThisFrame = false;
    [SerializeField] private float baseFireRate = 1f;

    void Start()
    {
        timer = refreshCooldown;
        UpdateFireRateText("--");
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

        DOTween.To(() => currentEmission, x => {
            currentEmission = x;
            mat.SetColor("_Emissive", currentEmission);
        }, targetEmission, 1f);
    }

    public void Deactivate()
    {
        if (!isActive) return;

        isActive = false;
        currentTarget = null;

        if (towerGFX != null && towerGFX.material != null)
        {
            Material mat = towerGFX.materials[0];
            Color currentEmission = mat.GetColor("_Emissive");
            Color targetEmission = currentEmission - Color.cyan * 10f;

            DOTween.Kill(mat);
            DOTween.To(() => currentEmission, x => {
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
        if (!wasPoweredThisFrame && isActive)
        {
            Deactivate();
        }

        wasPoweredThisFrame = false;
    }

    // === New Methods ===

    private void SetFireRate(float rate)
    {
        fireRate = rate;
        UpdateFireRateText(rate.ToString("0.0"));
    }

    private void UpdateFireRateText(string text)
    {
        if (fireRateText != null)
        {
            fireRateText.text = text;
        }
    }
}
