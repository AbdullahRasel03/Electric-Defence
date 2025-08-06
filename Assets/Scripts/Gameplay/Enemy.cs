using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    #region Settings

    [Header("Health Settings")]
    [SerializeField, Min(1)] private float maxHealth;
    [SerializeField] private TMP_Text healthText;

    [Header("Movement Settings")]
    [SerializeField, Min(0)] private float movementSpeed = 5f;
    [SerializeField] private EnemyType enemyType;

    [Header("Death Settings")]
    [SerializeField, Min(0.1f)] private float sinkDuration = 2f;
    [SerializeField, Min(0.1f)] private float sinkDepth = 2f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject damageParticlePrefab;
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private List<Renderer> enemyMesh;
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Animator animator;

    [SerializeField] private List<ParticleSystem> warpTrail;
    [SerializeField] private List<TrailRenderer> trails;

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
    public EnemyType EnemyType => enemyType;
    public float Speed => movementSpeed;

    #endregion


    #region Events

    public event System.Action<Enemy> OnDeath;
    public static event System.Action<Enemy> OnEnemyDead;

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

    public void ActivateEnemy(Vector3 position, Quaternion rotation, float _maxHealth)
    {
        trails.ForEach(trail => { trail.emitting = false; trail.Clear(); });
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        foreach (Renderer renderer in enemyMesh)
        {
            renderer.material = enemyMaterial;
        }
        warpTrail.ForEach(trail => trail.Play());
        trails.ForEach(trail => trail.emitting = true);

        transform.rotation = rotation;
        AudioManager.CallPlaySFX(Sound.EnemyAppear);

        transform.DOMove(position, 0.5f).SetEase(Ease.OutQuint)
            .OnComplete(() => StartCoroutine(TurnOffWarpTrail()));

        _currentHealth = maxHealth;
        _currentState = EnemyState.Active;

        SetPhysicsEnabled(true);
        SetHealthTextEnabled(true);
        UpdateHealthUI();

        gameObject.SetActive(true);
        EnemyManager.Instance?.RegisterEnemy(this);
    }

    private IEnumerator TurnOffWarpTrail()
    {
        yield return new WaitForSeconds(0.15f);
        warpTrail.ForEach(trail => trail.Stop());
    }

    public void TakeDamage(float damage)
    {
        if (!IsActive) return;

        AudioManager.CallPlaySFX(Sound.EnemyHit);

        StopCoroutine(DoDamageFlash());
        StartCoroutine(DoDamageFlash());

        PlayHitAnimation();
        ApplyDamage(damage);
        SpawnDamageParticles();

        CheckForDeath();
    }

    private IEnumerator DoDamageFlash()
    {
        foreach (Renderer renderer in enemyMesh)
        {
            renderer.material = damageMaterial;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (Renderer renderer in enemyMesh)
        {
            renderer.material = enemyMaterial;
        }
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
        OnEnemyDead?.Invoke(this);
        AudioManager.CallPlaySFX(Sound.EnemyDeath);

        CamController.Instance.CameraShake();

        _currentState = EnemyState.Dying;
        SetHealthTextEnabled(false);

        PlayDeathAnimation();
        SpawnDeathParticles();
        NotifyDeath();
        StartSinking();
    }

    private void PlayDeathAnimation()
    {
        // Optional death animation
    }

    private void SpawnDeathParticles()
    {
        if (deathParticlePrefab == null) return;

        GameObject particle = ObjectPool.instance.GetObject(deathParticlePrefab,
            true, transform.position + Vector3.up, Quaternion.identity);

        particle.GetComponent<ParticleSystem>().Play();
    }

    private void NotifyDeath()
    {
        OnDeath?.Invoke(this);
    }

    private void StartSinking()
    {
        _currentState = EnemyState.Sinking;

        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;

        _rigidbody.AddForce(new Vector3(0, -5, 15), ForceMode.Impulse);

        DOVirtual.DelayedCall(1.5f, () =>
        {
            CompleteDeathSequence();
        });
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
