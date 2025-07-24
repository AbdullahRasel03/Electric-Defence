using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterFX
{
    public class MLaser : MonoBehaviour
    {
        public float LaserDistance = 10f;
        public Transform LaserHead;
        public List<ParticleSystem> LaserBodies;

        public ParticleSystem Laser;
        public ParticleSystem LaserStop; // Pre-assigned in inspector
        //Apply an offset before hit;
        public float HitOffset;

        Vector3 EndPos;

        void OnValidate()
        {
            UpdateLaser();
        }

        void UpdateLaser()
        {
            if (LaserHead != null)
            {
                LaserHead.localPosition = new Vector3(0, 0, LaserDistance);
            }

            if (LaserBodies != null)
            {
                foreach (var laserBody in LaserBodies)
                {
                    if (laserBody == null) continue;

                    var mainModule = laserBody.main;
                    mainModule.startSize3D = true;
                    mainModule.startSizeX = laserBody.main.startSizeX.constant;
                    mainModule.startSizeY = LaserDistance;
                    mainModule.startSizeZ = 1;
                }
            }
        }

        public void UpgradeLaser()
        {

            if (LaserBodies != null)
            {
                foreach (var laserBody in LaserBodies)
                {
                    if (laserBody == null) continue;

                    var mainModule = laserBody.main;
                    mainModule.startSize3D = true;
                    mainModule.startSizeX = laserBody.main.startSizeX.constant + 1;
                    mainModule.startSizeY = LaserDistance;
                    mainModule.startSizeZ = 1;
                }
            }
        }
        void Start()
        {
          /*  // Ensure stop particle is disabled at start
            if (LaserStop != null)
            {
                LaserStop.gameObject.SetActive(false);
            }*/
        }

        public void SetLaser(Vector3 start, Vector3 end)
        {
            transform.position = start;
            EndPos = end;
            LaserDistance = Vector3.Distance(start, end) - HitOffset;
            transform.LookAt(end);
            UpdateLaser();
            LaserStop.transform.position = end;
            // Enable laser if it was disabled
            if (Laser != null && !Laser.isPlaying)
            {
                Laser.gameObject.SetActive(true);
                Laser.Play();
            }
        }

        public void StopLaser()
        {
            // Stop main laser
            if (Laser != null)
            {
                Laser.Stop();
            }

            // Show stop effect
            if (LaserStop != null)
            {
                LaserStop.gameObject.SetActive(true);
                LaserStop.transform.localPosition = Vector3.zero;
                LaserStop.transform.LookAt(EndPos);

                var mainModule = LaserStop.main;
                mainModule.startSize3D = true;
                mainModule.startSizeX = LaserStop.main.startSizeX.constant;
                mainModule.startSizeY = LaserDistance;
                mainModule.startSizeZ = 1;

                LaserStop.Play();
            }
        }

        public void EnableLaser(bool enable)
        {
            if (Laser != null)
            {
                if (enable)
                {
                    Laser.gameObject.SetActive(true);
                    Laser.Play();
                }
                else
                {
                    Laser.Stop();
                    Laser.gameObject.SetActive(false);
                }
            }
        }
    }
}