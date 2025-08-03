using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //public int damageOverTime = 30;

    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private LineRenderer lineRenderer;
    public float mainTextureLength = 1f;
    public float noiseTextureLength = 1f;
    private Vector4 length = new Vector4(1, 1, 1, 1);

    private ParticleSystem[] effects;

    void Awake()
    {
        effects = GetComponentsInChildren<ParticleSystem>();
    }

    public void SetLinePosition(Vector3 start, Vector3 end)
    {
        lineRenderer.material.SetTextureScale("_MainTex", new Vector2(length[0], length[1]));
        lineRenderer.material.SetTextureScale("_Noise", new Vector2(length[2], length[3]));

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        hitEffect.transform.position = end;

        foreach (var AllPs in effects)
        {
            if (!AllPs.isPlaying) AllPs.Play();
        }
        //Texture tiling
        length[0] = mainTextureLength * Vector3.Distance(transform.position, end);
        length[2] = noiseTextureLength * Vector3.Distance(transform.position, end);
    }
}
