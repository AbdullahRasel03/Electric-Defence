using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f;
    public float movementSpeed = 5f;
    public string enemyType = "BasicEnemy";

    [Header("References")]
    [SerializeField] private TMP_Text healthText;

    private Rigidbody _rb;
    [SerializeField] private float _currentHealth;
    private bool _isActive;

    // Event for death notification
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

        // Notify systems that this enemy has spawned
        EnemyManager.Instance?.RegisterEnemy(this);
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        // Move along -Z axis
        _rb.MovePosition(_rb.position + Vector3.back * (movementSpeed * Time.fixedDeltaTime));
    }

    public void TakeDamage(float damage)
    {
        if (!_isActive) return;

        _currentHealth -= damage;
        UpdateHealthUI();

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
        OnDeath?.Invoke(this);
        Cleanup();
    }

    private void Cleanup()
    {
        // Notify systems that this enemy is dying
        EnemyManager.Instance?.UnregisterEnemy(this);
        ObjectPool.instance.ReturnToPool(gameObject);
    }

    private void OnDisable()
    {
        // Handle case where object is disabled without Die() being called
        if (_isActive)
        {
            Die();
        }
    }
}