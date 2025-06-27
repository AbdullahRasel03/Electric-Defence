using UnityEngine;
using TMPro;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    #region Settings

    [Header("Health Settings")]
    [SerializeField, Min(1)] private float maxHealth;
    [SerializeField] private TMP_Text healthText;

    [Header("Movement Settings")]
    [SerializeField, Min(0)] private float movementSpeed = 5f;
    [SerializeField] private string enemyType = "BasicEnemy";

    [Header("Death Settings")]
    [SerializeField, Min(0.1f)] private float sinkDuration = 2f;
    [SerializeField, Min(0.1f)] private float sinkDepth = 2f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject damageParticlePrefab;
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private Renderer enemyMesh;
    [SerializeField] private Animator animator;

    #endregion

    #region Components

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Tween _sinkTween;

    #endregion

    #region State

    private enum EnemyState
    {
        Inactive,
        Active,
        Dying,
        Sinking
    }

    private EnemyState _currentState = EnemyState.Inactive;
    private float _currentHealth;

    public bool IsActive => _currentState == EnemyState.Active;
    public string EnemyType => enemyType;

    #endregion

    #region Events

    public event System.Action<Enemy> OnDeath;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        CacheComponents();
        ValidateReferences();
    }

    private void FixedUpdate()
    {
        if (!IsActive) return;
        MoveForward();
    }

    private void OnDisable()
    {
        if (IsActive)
        {
            HandleForcedDeactivation();
        }
        CleanupTweens();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Activates the enemy at the specified position and rotation
    /// </summary>
    /// <param name="position">World position to spawn</param>
    /// <param name="rotation">World rotation to apply</param>
    public void ActivateEnemy(Vector3 position, Quaternion rotation, float _maxHealth, float _speed)
    {
        movementSpeed = _speed;
        maxHealth = _maxHealth;
        transform.SetPositionAndRotation(position, rotation);
        _currentHealth = maxHealth;
        _currentState = EnemyState.Active;

        SetPhysicsEnabled(true);
        SetHealthTextEnabled(true);
        UpdateHealthUI();

        gameObject.SetActive(true);
        EnemyManager.Instance?.RegisterEnemy(this);
    }

    public void TakeDamage(float damage)
    {
        if (!IsActive) return;

        PlayHitAnimation();
        ApplyDamage(damage);
        SpawnDamageParticles();

        CheckForDeath();
    }

    #endregion

    #region Private Methods

    private void CacheComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void ValidateReferences()
    {
        Debug.Assert(animator != null, "Animator reference is missing!", this);
        Debug.Assert(enemyMesh != null, "Enemy mesh reference is missing!", this);
    }

    private void MoveForward()
    {
        _rigidbody.MovePosition(_rigidbody.position + Vector3.back * (movementSpeed * Time.fixedDeltaTime));
    }

    private void PlayHitAnimation()
    {
        animator.Play("Head Hit");
    }

    private void ApplyDamage(float damage)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText == null) return;

        healthText.text = $"{Mathf.Round(_currentHealth)}";
        healthText.transform.forward = Camera.main.transform.forward;
    }

    private void SpawnDamageParticles()
    {
        if (damageParticlePrefab == null) return;

        var particle = ObjectPool.instance.GetObject(damageParticlePrefab,
            true, transform.position, Quaternion.identity);
        ObjectPool.instance.ReturnToPool(particle, 1.5f);
    }

    private void CheckForDeath()
    {
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _currentState = EnemyState.Dying;
        SetHealthTextEnabled(false);

        PlayDeathAnimation();
        SpawnDeathParticles();
        NotifyDeath();
        StartSinking();
    }

    private void PlayDeathAnimation()
    {
        animator.Play("Death");
    }

    private void SpawnDeathParticles()
    {
        if (deathParticlePrefab == null) return;

        var particle = ObjectPool.instance.GetObject(deathParticlePrefab,
            true, transform.position, Quaternion.identity);
        ObjectPool.instance.ReturnToPool(particle, 2f);
    }

    private void NotifyDeath()
    {
        OnDeath?.Invoke(this);
    }

    private void StartSinking()
    {
        _currentState = EnemyState.Sinking;
        SetPhysicsEnabled(false);

        _sinkTween = transform.DOMoveY(transform.position.y - sinkDepth, sinkDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(CompleteDeathSequence);
    }

    private void CompleteDeathSequence()
    {
        CleanupTweens();
        ResetEnemy();
        ReturnToPool();
    }

    private void HandleForcedDeactivation()
    {
        Die();
    }

    private void SetPhysicsEnabled(bool enabled)
    {
        _rigidbody.isKinematic = !enabled;
        _collider.enabled = enabled;
    }

    private void SetHealthTextEnabled(bool enabled)
    {
        if (healthText != null)
        {
            healthText.enabled = enabled;
        }
    }

    private void CleanupTweens()
    {
        if (_sinkTween == null || !_sinkTween.IsActive()) return;
        _sinkTween.Kill();
    }

    private void ResetEnemy()
    {
        _currentState = EnemyState.Inactive;
        SetPhysicsEnabled(true);
    }

    private void ReturnToPool()
    {
        EnemyManager.Instance?.UnregisterEnemy(this);
        ObjectPool.instance.ReturnToPool(gameObject);
    }

    #endregion
}