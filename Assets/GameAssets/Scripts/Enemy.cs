using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    // ====================================================================================================
    // === General ===
    [Header("General")]
    [SerializeField] private LayerMask _targetLayersMask;
    [SerializeField] private LayerMask _ignoredLayersMask;
    // ====================================================================================================

    // ====================================================================================================
    // === Animations ===
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private Animator _animator;

    [SerializeField] private float _syncedAttackAnimLength;

    private bool _shouldSyncAttackAnim;
    // ====================================================================================================

    // ====================================================================================================
    // === VFX ===
    [Space(15)]
    [Header("VFX")]
    [SerializeField] private GameObject _damageText;

    [SerializeField] private float _damageTextYOffset;
    // ====================================================================================================

    // ====================================================================================================
    // === SFX ===
    [Space(15)]
    [Header("SFX")]
    [SerializeField] private AudioClip _deathSound;

    [SerializeField] private float _deathSoundVolume;
    // ====================================================================================================

    // ====================================================================================================
    // === Stats ===
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _expReward;

    [Space(5)]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _attackRange;

    [Space(5)]
    [Header("Type")]
    [SerializeField] private bool _isRange;
    // ====================================================================================================

    // ====================================================================================================
    // === Projectile ===
    [Space(15)]
    [Header("Projectile (Only If Range)")]
    [SerializeField] private GameObject _projectilePrefab;

    [SerializeField] private float _projectileMoveSpeed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private float _projectileYOffset;

    private Vector3 _projectileSpawnPoint;
    // ====================================================================================================

    // ====================================================================================================
    // === On Death ===
    [Space(15)]
    [Header("On Death")]
    [SerializeField] private float _waitBeforeDestroy;

    [HideInInspector] public bool _isDead;
    // ====================================================================================================

    // ====================================================================================================
    // === RUNTIME ===
    private bool _canAttack = true;
    private bool _isChasingTarget;
    private float _distanceToTarget;
    private Vector3 _targetPos;
    private Vector3 _targetDir;
    private GameObject _target;
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;
    // ====================================================================================================

    void Update()
    {
        if (_isDead) return;

        _distanceToTarget = Vector3.Distance(transform.position, _targetPos);

        if (!_isChasingTarget)
        {
            if (_distanceToTarget <= _detectionRange)
            {
                _isChasingTarget = true;
            }
            else
            {
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (_distanceToTarget > _attackRange)
            {
                _targetDir = (_targetPos - transform.position).normalized;
                transform.position += _targetDir * _moveSpeed * Time.deltaTime;

                transform.LookAt(_targetPos);
            }
            else
            {
                if (_shouldSyncAttackAnim)
                {
                    SyncAttackAnimation();
                }

                if (!_canAttack && !_shouldSyncAttackAnim)
                {
                    AttackCooldown();
                }

                if (_canAttack && !_shouldSyncAttackAnim)
                {
                    AttackTarget();
                }
            }
        }
    }

    public void Init(float healthMultiplier, float damageMultiplier, float expRewardMultiplier, GameObject target)
    {
        _health *= healthMultiplier;
        _damage *= damageMultiplier;
        _expReward *= expRewardMultiplier;

        _target = target;
        _targetPos = _target.transform.position;

        _attackTimer = new GlobalTimer(_attackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
    }

    private void AttackTarget()
    {
        if (_animator != null)
        {
            _animator.Play("Attack");
        }

        _canAttack = false;
        _shouldSyncAttackAnim = true;
    }

    private void SyncAttackAnimation()
    {
        _syncAnimTimer.Tick();

        if (_syncAnimTimer.Flag)
        {
            _shouldSyncAttackAnim = false;

            if (_isRange)
            {
                _projectileSpawnPoint.Set(transform.position.x, _projectileYOffset, transform.position.z);
                GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint, Quaternion.identity);
                projectile.GetComponent<Projectile>().Init(_damage, _projectileMoveSpeed, _projectileLifeTime, _targetPos, _targetLayersMask, _ignoredLayersMask);
            }
            else
            {
                IDamageable target = _target.GetComponent<IDamageable>();

                if (target != null)
                {
                    target.TakeDamage(_damage);
                }
            }

            _syncAnimTimer.Reset();
        }
    }

    private void AttackCooldown()
    {
        _attackTimer.Tick();

        if (_attackTimer.Flag)
        {
            _canAttack = true;

            _attackTimer.Reset();
        }
    }

    private void DisplayDamage(float damage)
    {
        if (_damageText)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.25f, transform.position.x + 0.5f), UnityEngine.Random.Range(_damageTextYOffset - 0.5f, _damageTextYOffset + 0.5f), transform.position.z);
            var floatingTextObject = Instantiate(_damageText, spawnPosition, Quaternion.identity, transform);

            if (floatingTextObject)
            {
                var floatingTextMesh = floatingTextObject.GetComponent<TextMesh>();

                if (floatingTextMesh)
                {
                    floatingTextMesh.color = new Color(1f, UnityEngine.Random.Range(0, 100) / 255f, UnityEngine.Random.Range(0, 255) / 255f, 1f);
                    floatingTextMesh.text = damage.ToString();
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        DisplayDamage(damage);

        if (_health <= 0)
        {
            _isDead = true;

            if (_animator != null)
            {
                _animator.Play("Death");
            }

            AudioManager.Instance.PlaySFXClip(_deathSound, _deathSoundVolume, transform.position);

            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            Destroy(gameObject, _waitBeforeDestroy);
        }
    }
}