using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : Turret
{

    [Space]
    [Header("Inferno Turret Properties")]
    [SerializeField] private GameObject laser;
    [SerializeField] private Transform shootPoint;

    private List<Enemy> targets = new List<Enemy>();
    private Dictionary<Collider, Laser> lasersAppointed = new Dictionary<Collider, Laser>();

    private float currentFireRate = 0;


    private void OnEnemyDeath(Enemy enemy)
    {
        if (targets.Contains(enemy))
        {
            targets.Remove(enemy);
        }

        if (lasersAppointed.ContainsKey(enemy.GetComponent<Collider>()))
        {
            ObjectPool.instance.ReturnToPool(lasersAppointed[enemy.GetComponent<Collider>()].gameObject);
            lasersAppointed.Remove(enemy.GetComponent<Collider>());
        }

    }



    protected override void Fire()
    {
        // if (!isActive) return;
        // if (targets.Count == 0) return;
        if (currentTarget == null)
        {
            foreach (KeyValuePair<Collider, Laser> laser in lasersAppointed)
            {
                ObjectPool.instance.ReturnToPool(laser.Value.gameObject);
            }
            
            lasersAppointed.Clear();
            return;
        }

        MoveLaser();
        DoDamageOverTime();
    }

    private void MoveLaser()
    {
        if (lasersAppointed.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<Collider, Laser> laser in lasersAppointed)
        {
            laser.Value.SetLinePosition(shootPoint.position, laser.Key.ClosestPoint(shootPoint.position));

            Vector3 hitPoint = laser.Key.ClosestPoint(shootPoint.position);

            laser.Value.SetLinePosition(shootPoint.position, hitPoint);

            laser.Value.transform.rotation = Quaternion.LookRotation(hitPoint - laser.Value.transform.position);
        }
    }

    private void DoDamageOverTime()
    {
        if (targets.Count == 0)
        {
            return;
        }

        // if (!isActive)
        // {
        //     return;
        // }

        currentFireRate += Time.deltaTime;

        if (currentFireRate >= fireDelay)
        {
            currentFireRate = 0;

            foreach (Enemy target in targets)
            {
                if (target == null) continue;

                target.TakeDamage(10);
            }
        }
    }

    protected override void AssignClosestEnemy()
    {
        base.AssignClosestEnemy();

        Shoot();
    }

    private void Shoot()
    {
        if (currentTarget == null)
        {
            return;
        }

        Collider targetCollider = currentTarget.GetComponent<Collider>();

        if (!lasersAppointed.ContainsKey(targetCollider))
        {
            GameObject laserObj = ObjectPool.instance.GetObject(laser.gameObject);
            Laser newLaser = laserObj.GetComponent<Laser>();

            laserObj.transform.position = shootPoint.position;
            laserObj.transform.parent = shootPoint;

            lasersAppointed.Add(targetCollider, newLaser);

            Vector3 hitPoint = targetCollider.ClosestPoint(shootPoint.position);

            newLaser.SetLinePosition(shootPoint.position, hitPoint);

            newLaser.transform.rotation = Quaternion.LookRotation(hitPoint - newLaser.transform.position);

            newLaser.gameObject.SetActive(true);
        }

    }
}


