using System.Collections.Generic;
using UnityEngine;
using System;

public enum EStatsType { EHealth, EHealthRegen, EAttackDamage, EAttackSpeed, EProjectileSpeed }

public class Player : MonoBehaviour
{
    #region Components
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private LayerMask _ignoredLayerMask;
    #endregion

    #region Stats
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _healthBase;
    [SerializeField] private float _healthCurrentMax;
    [SerializeField] private float _healthCurrent;

    [SerializeField] private float _healthRegenerationBase;
    [SerializeField] private float _healthRegenerationCurrent;

    [SerializeField] private float _attackDamageBase;
    [SerializeField] private float _attackDamageCurrent;

    [SerializeField] private float _attackSpeedBase;
    [SerializeField] private float _attackSpeedCurrent;

    [SerializeField] private float _attackRange;

    [SerializeField] private float _projectileMoveSpeedBase;
    [SerializeField] private float _projectileMoveSpeedCurrent;
    #endregion

    #region Animations
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private float _syncedAttackAnimLength;
    [SerializeField] private float _animationAttackMultiplier;

    private bool _shouldSyncAttackAnim;
    #endregion

    #region Projectile
    [Space(15)]
    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileParent;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private float _projectileYOffset;

    private Vector3 _projectileTargetInitPos;
    [HideInInspector] public GameObject _projectileTarget;
    #endregion

    #region Floating Text
    [Space(15)]
    [Header("Floating Text")]
    [SerializeField] private GameObject _floatingText;
    [SerializeField] private float _floatingTextYOffset;
    #endregion

    #region Runtime
    private bool _canAttack = true;
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;

    [HideInInspector] public static Player _SPlayerScript;
    [HideInInspector] public List<GameObject> _enemies;
    #endregion

    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        _attackTimer = new GlobalTimer(_attackSpeedCurrent);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);

        if (_animator)
        {
            _animator.SetFloat("AttackSpeedMultiplier", _animationAttackMultiplier);
        }

        _SPlayerScript = this;
        _enemies = new List<GameObject>();
    }

    private void Update()
    {
        if (_healthCurrentMax <= 0)
        {
            Destroy(gameObject);
        }

        if (_projectileTarget == null)
        {
            _projectileTarget = FindClosest();
        }

        if (_shouldSyncAttackAnim)
        {
            SyncAttackAnimation();
        }

        if (!_canAttack && !_shouldSyncAttackAnim)
        {
            AttackCooldown();
        }

        if (_projectileTarget != null && _canAttack && !_shouldSyncAttackAnim)
        {
            if (AttackTarget())
            {
                _canAttack = false;
            }
        }
    }


    public void SelectTarget()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _targetLayerMask))
        {
            _projectileTarget = hit.collider.gameObject;

            UpdateLookAt();

            Debug.Log(_projectileTarget.name);
        }
    }

    private bool AttackTarget()
    {
        float targetDistance = Vector3.Distance(transform.position, _projectileTarget.transform.position);

        if (_projectileTarget != null && _attackRange >= targetDistance)
        {
            UpdateLookAt();

            _projectileTargetInitPos = _projectileTarget.transform.position;

            if (_animator != null)
            {
                _animator.Play("Attack");
            }

            _shouldSyncAttackAnim = true;

            return true;
        }

        return false;
    }

    private void UpdateLookAt()
    {
        if (_projectileTarget != null)
        {
            transform.LookAt(_projectileTarget.transform);
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

    private void SyncAttackAnimation()
    {
        _syncAnimTimer.Tick();

        if (_syncAnimTimer.Flag)
        {
            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Init((float)Math.Round(UnityEngine.Random.Range((_attackDamageCurrent - _attackDamageCurrent * 0.1f), (_attackDamageCurrent + _attackDamageCurrent * 0.1f)), 2),
                                                       _projectileMoveSpeedCurrent, _projectileLifeTime, _projectileTargetInitPos, _projectileTarget, _targetLayerMask, _ignoredLayerMask, true, _projectileParent);

            _shouldSyncAttackAnim = false;

            _syncAnimTimer.Reset();
        }
    }

    private GameObject FindClosest()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject obj in _enemies)
        {
            if (obj == null) continue;

            float distance = Vector3.Distance(obj.transform.position, gameObject.transform.position);

            if (distance < minDistance && distance <= _attackRange)
            {
                minDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }

    private void DisplayDamage(float dmg)
    {
        if (_floatingText)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.25f, transform.position.x + 0.5f),
                                                UnityEngine.Random.Range(_floatingTextYOffset - 0.5f, _floatingTextYOffset + 0.5f), transform.position.z);

            var floatingTextObject = Instantiate(_floatingText, spawnPosition, Quaternion.identity, transform);

            if (floatingTextObject)
            {
                var floatingTextMesh = floatingTextObject.GetComponent<TextMesh>();

                if (floatingTextMesh)
                {
                    floatingTextMesh.color = new Color(1f, UnityEngine.Random.Range(0, 100) / 255f, UnityEngine.Random.Range(0, 255) / 255f, 1f);
                    floatingTextMesh.text = dmg.ToString();
                }
            }
        }
    }

    private void ResetPlayer()
    {
        //Health
        _healthCurrent = _healthBase;
        _healthCurrentMax = _healthBase;

        //Regeberation
        _healthRegenerationCurrent = _healthRegenerationBase;
        _attackDamageCurrent = _attackDamageBase;

        //Attack Speed
        _attackSpeedCurrent = _attackSpeedBase;

        //ToDo Pres 
    }

    // --- Getters / Setters ---
    public void TakeDamage(float dmg)
    {
        _healthCurrent -= dmg;
        DisplayDamage(dmg);
    }

    public void ApplyBuff(EStatsType type, float value)
    {
        switch (type)
        {
            case EStatsType.EHealth:
                {
                    value = 1 + (value / 100);
                    _healthCurrent *= value;
                }
                break;
            case EStatsType.EHealthRegen:
                {
                    value = 1 + (value / 100);
                    _healthRegenerationCurrent *= value;
                }
                break;
            case EStatsType.EAttackDamage:
                {
                    value = 1 + (value / 100);
                    _attackDamageCurrent *= value;
                }
                break;
            case EStatsType.EAttackSpeed:
                {
                    var temp = 1 - (value / 100);

                    _attackSpeedCurrent *= temp;
                    _attackTimer.Duration = _attackSpeedCurrent;
                    _syncedAttackAnimLength *= temp;
                    _syncAnimTimer.Duration = _syncedAttackAnimLength;

                    if (_animator)
                    {
                        _animationAttackMultiplier *= 1 + (value / 100);
                        _animator.SetFloat("AttackSpeedMultiplier", _animationAttackMultiplier);
                    }
                }
                break;
            case EStatsType.EProjectileSpeed:
                {
                    value = 1 + (value / 100);
                    _projectileMoveSpeedCurrent *= value;
                }
                break;
        }
    }

    public void AddStats(EStatsType type, float value)
    {
        switch (type)
        {
            case EStatsType.EHealth:
                {
                    _healthBase += value;
                }
                break;
            case EStatsType.EHealthRegen:
                {
                    _healthRegenerationBase += value;
                }
                break;
            case EStatsType.EAttackDamage:
                {
                    _attackDamageBase += value;
                }
                break;
            case EStatsType.EAttackSpeed:
                {
                    _attackSpeedBase -= value;
                }
                break;
            case EStatsType.EProjectileSpeed:
                {
                    _projectileMoveSpeedBase += value;
                }
                break;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + _projectileYOffset, transform.position.z), Vector3.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}