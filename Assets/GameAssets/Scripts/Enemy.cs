using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Components
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _targetLayersMask;
    [SerializeField] private LayerMask _ignoredLayersMask;
    #endregion

    #region Stats
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _expReward;

    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _projectileMoveSpeed;

    [Space(5)]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectionDistance;

    [Space(5)]
    [SerializeField] private bool _isRange;
    #endregion

    #region Projectile
    [Space(15)]
    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private float _projectileYOffset;

    private Vector3 _projectileSpawnPoint;
    #endregion

    #region SFX
    [Space(15)]
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private float _deathSoundVolume;
    #endregion

    #region Animations
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private float _syncedAttackAnimLength;

    private bool _shouldSyncAttackAnim;
    #endregion

    #region On Death
    [Space(15)]
    [Header("On Death")]
    [SerializeField] float _waitBeforeDestroy;

    [HideInInspector] public bool _isDead;
    #endregion

    #region Floating Text
    [Space(15)]
    [Header("Floating Text")]
    [SerializeField] private GameObject _floatingText;
    [SerializeField] private float _floatingTextYOffset;
    #endregion

    #region Runtime
    private bool _canAttack = true;
    private bool _isChasingTarget;
    private float _distanceToTarget;
    private Vector3 _targetPos;
    private Vector3 _targetDir;
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;
    #endregion

    private void Awake()
    {
        if (hasPlayer())
        {
            Player._SPlayerScript._enemies.Add(gameObject);

            _targetPos.Set(Player._SPlayerScript.transform.position.x, 0f, Player._SPlayerScript.transform.position.z);
        }

        _attackTimer = new GlobalTimer(_attackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
    }

    void Update()
    {
        if (_isDead) return;

        _distanceToTarget = Vector3.Distance(transform.position, _targetPos);

        if (!_isChasingTarget)
        {
            if (_distanceToTarget <= _detectionDistance)
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

    private bool hasPlayer()
    {
        return (Player._SPlayerScript != null && Player._SPlayerScript._enemies != null);
    }

    public void Init(float healthMultiplier, float damageMultiplier, float expRewardMultiplier)
    {
        _health *= healthMultiplier;
        _damage *= damageMultiplier;
        _expReward *= expRewardMultiplier;
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
                if (hasPlayer())
                {
                    Player._SPlayerScript.TakeDamage(_damage);
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
        if (_floatingText)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.25f, transform.position.x + 0.5f), UnityEngine.Random.Range(_floatingTextYOffset - 0.5f, _floatingTextYOffset + 0.5f), transform.position.z);
            var floatingTextObject = Instantiate(_floatingText, spawnPosition, Quaternion.identity, transform);

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


    // --- Interface ---
    public void TakeDamage(float damage)
    {
        _health -= damage;
        DisplayDamage(damage);

        if (_health <= 0)
        {
            _isDead = true;

            AudioManager.SAudioManager.PlaySoundFXClip(_deathSound, _deathSoundVolume, transform.position);

            if (_animator != null)
                _animator.Play("Death");

            if (hasPlayer())
                Player._SPlayerScript._enemies.Remove(gameObject);

            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider != null)
                Destroy(collider);

            Destroy(gameObject, _waitBeforeDestroy);
        }
    }
}