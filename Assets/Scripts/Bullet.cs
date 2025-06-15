using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    private float _lifeTimer;
    private const float _maxLifetime = 5f;

    private void OnEnable()
    {
        // Reset lifetime whenever bullet is activated
        _lifeTimer = _maxLifetime;
    }

    private void Update()
    {
        // Auto-return to pool if bullet doesn't hit anything
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
        if (ObjectPool.instance != null)
        {
            ObjectPool.instance.ReturnToPool(gameObject);
        }
        else
        {
            // Fallback in case pool system isn't available
            gameObject.SetActive(false);
        }
    }
}