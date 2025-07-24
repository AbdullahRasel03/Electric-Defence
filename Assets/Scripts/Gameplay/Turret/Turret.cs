using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] protected Transform turretBody;
    [SerializeField] protected float range = 18f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected bool isActive = false; // Track active state

    protected Enemy currentTarget;
    protected float refreshCooldown = 0.2f;
    protected float timer;

    [SerializeField] Renderer towerGFX;
    void Start()
    {
        timer = refreshCooldown;
    }

    void Update()
    {
        if (!isActive) return; // Only update if active

        AssignClosestEnemy();
        RotateYAxisToTarget();
        Fire();
    }

    // Public method to activate the turret
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

    // Public method to deactivate the turret
    // Public method to deactivate the turret
    public void Deactivate()
    {
        if (isActive == false) return; // Already deactivated

        isActive = false;
        currentTarget = null; // Clear target when deactivated

        // Handle emission reversal
        if (towerGFX != null && towerGFX.material != null)
        {
            Material mat = towerGFX.materials[0];
            Color currentEmission = mat.GetColor("_Emissive");
            Color targetEmission = currentEmission - Color.cyan * 10f; // Reverse the activation effect

            // Kill any existing tweens to prevent conflicts
            DOTween.Kill(mat);

            DOTween.To(() => currentEmission, x => {
                mat.SetColor("_Emissive", x); // Directly set the color
            }, targetEmission, 1f);
        }

        // Reset fire rate to base when deactivated
        fireRate = baseFireRate;
    }

    // Updated ToggleActive to handle visuals properly
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
                // if (!enemy.IsActive) continue;

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

    private bool wasPoweredThisFrame = false;

    [SerializeField] private float baseFireRate = 1f;

    public void ReceivePower(float totalMultiplier)
    {
        wasPoweredThisFrame = true;

        if (!isActive)
        {
            Activate();
        }

        fireRate = baseFireRate * Mathf.Max(1f, totalMultiplier); // prevents 0 or negative fire rate
    }

    void LateUpdate()
    {
        if (!wasPoweredThisFrame && isActive)
        {
            Deactivate(); // Turn off if not hit this frame
        }

        wasPoweredThisFrame = false;
    }

}