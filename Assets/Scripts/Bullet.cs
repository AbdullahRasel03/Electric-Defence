using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.activeSelf)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy.IsActive)
            {
                enemy.TakeDamage(_damage);
            }
        }
        ReturnToPool();
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
