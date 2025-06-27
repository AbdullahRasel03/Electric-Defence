using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    public float movementSpeed = 5f;
    public string enemyType = "BasicEnemy";

    [Header("References")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject damageParticlePrefab;
    [SerializeField] private GameObject deathParticlePrefab;

    private Rigidbody _rb;
   
    private bool _isActive;

    public event System.Action<Enemy> OnDeath;
    public bool IsActive => _isActive;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        _currentHealth = maxHealth;
        _isActive = true;
        gameObject.SetActive(true);
        UpdateHealthUI();

        EnemyManager.Instance?.RegisterEnemy(this);
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        _rb.MovePosition(_rb.position + Vector3.back * (movementSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(float damage)
    {
        if (!_isActive) return;

        _currentHealth -= damage;
        UpdateHealthUI();

        if (damageParticlePrefab != null)
        {
            var particle = ObjectPool.instance.GetObject(damageParticlePrefab, true, transform.position, Quaternion.identity);
            ObjectPool.instance.ReturnToPool(particle, 1.5f); // Adjust time to match particle duration
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Round(_currentHealth)}";
            healthText.transform.forward = Camera.main.transform.forward;
        }
    }

    public void Die()
    {
        if (!_isActive) return;

        _isActive = false;

        if (deathParticlePrefab != null)
        {
            var particle = ObjectPool.instance.GetObject(deathParticlePrefab, true, transform.position, Quaternion.identity);
            ObjectPool.instance.ReturnToPool(particle, 2f); // Adjust time to match particle duration
        }

        OnDeath?.Invoke(this);
        Cleanup();
    }

    private void Cleanup()
    {
        EnemyManager.Instance?.UnregisterEnemy(this);
        ObjectPool.instance.ReturnToPool(gameObject);
    }

    private void OnDisable()
    {
        if (_isActive)
        {
            Die();
        }
    }
}
