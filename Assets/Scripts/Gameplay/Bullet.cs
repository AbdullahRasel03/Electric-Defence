using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _lifeTimer;
    private const float _maxLifetime = 5f;
    [SerializeField] private GameObject trail;

    private TrailRenderer trailRenderer;

    private void Awake()
    {
        if (trail != null)
        {
            trailRenderer = trail.GetComponent<TrailRenderer>();
        }
    }

    private void OnEnable()
    {
        _lifeTimer = _maxLifetime;

        // Re-enable and clear trail after position is already set
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = true;
        }
    }

    private void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        if (ObjectPool.instance != null)
        {
            ObjectPool.instance.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
