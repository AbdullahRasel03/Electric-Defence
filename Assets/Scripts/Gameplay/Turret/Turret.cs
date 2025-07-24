using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] protected Transform turretBody;
    [SerializeField] protected float range = 18f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float fireDelay = 1f;
    public GridObject[] gridsOnPath;
    protected Enemy currentTarget;
    protected float refreshCooldown = 0.2f;
    protected float timer;

    void Start()
    {
        timer = refreshCooldown;
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
        float fireRate = 1;
        foreach (GridObject item in gridsOnPath)
        {
            if (item.socket)
            {
                fireRate += item.socket.ownMultiplier;
                item.socket.PowerUp();
            }
        }
        // shooter.SetFireRate(fireRate);
        // powerText.text = shooter.GetFireRate().ToString();
    }

}
