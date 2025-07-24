using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Turret : MonoBehaviour
{
    [SerializeField] protected Transform turretBody;
    [SerializeField] protected float range = 18f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float fireDelay = 1f;
    [SerializeField] private TMP_Text fireRateText;
    public GridObject[] gridsOnPath;
    protected Enemy currentTarget;
    protected float refreshCooldown = 0.2f;
    protected float timer;

    void Start()
    {
        timer = refreshCooldown;
        UpdateFireRateText();
    }

    private void UpdateFireRateText()
    {
        if (fireRateText != null)
        {
            fireRateText.text = (1f / fireDelay).ToString("F2") + "/s";
        }
    }


    void Update()
    {
        AssignClosestEnemy();
        RotateYAxisToTarget();
        Fire();
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

    public void CheckMultisOnPath()
    {
        // float fireRate = 1;
        float currentFireDelay = fireDelay;
        foreach (GridObject item in gridsOnPath)
        {
            if (item.socket)
            {
                currentFireDelay -= item.socket.ownMultiplier / 50f;
                item.socket.PowerUp();
            }
        }

        fireDelay = currentFireDelay;

        UpdateFireRateText();
        // shooter.SetFireRate(fireRate);
        // powerText.text = shooter.GetFireRate().ToString();
    }

    public void RotateFireRateText()
    {
        if (fireRateText != null)
        {
            fireRateText.transform.parent.DOLocalRotateQuaternion(Quaternion.Euler(-75f, 0f, 0f), 1.5f);
        }
    }

}
