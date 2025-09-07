using System;
using UnityEngine;

public enum EStatsType { None, EAttackDamage, ECritChance, EDefense, EHealth, EHealthRegenAmount }

public class Player : MonoBehaviour, IDamageable
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // General
    [Header("=== General ===")]
    [SerializeField] private Camera _mainCamera;
    
    [SerializeField] private LayerMask _targetLayersMask;
    [SerializeField] private LayerMask _ignoredLayersMask;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Animations
    [Space(15)]
    [Header("=== Animations ===")]
    [SerializeField] private Animator _animator;
    
    [SerializeField] private float _syncedAttackAnimLength;
    [SerializeField] private float _animationAttackMultiplier;
    
    private bool _shouldSyncAttackAnim;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // VFX
    [Space(15)]
    [Header("=== VFX ===")]
    [SerializeField] private GameObject _thunderPrefab;
    [SerializeField] private GameObject _thunderCritPrefab;
    
    [SerializeField] private float _thunderLifeTime;

    [Header("Damage Text")]
    [SerializeField] private GameObject _damageText;
    
    [SerializeField] private float _damageTextYOffset;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // SFX
    [Space(15)]
    [Header("=== SFX ===")]

    [Header("Attack")]
    [SerializeField] private AudioClip _attackSFXClip;
    [SerializeField, Range(0f, 1f)] private float _attackSFXVolume;

    [Header("Buff")]
    [SerializeField] private AudioClip _buffUpSFXClip;
    [SerializeField, Range(0f, 1f)] private float _buffUpSFXVolume;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Stats
    [Space(15)]
    [Header("=== Stats ===")]

    [Header("Attack")]
    [SerializeField] private float _baseAttackDamage;
    [SerializeField] private float _currentAttackDamage;
    
    [SerializeField] private float _baseAttackSpeed;

    [Header("Critical")]
    [SerializeField] private float _baseCritChance;
    [SerializeField] private float _currentCritChance;
    
    [SerializeField] private float _baseCritMultiplier;

    [Header("Splash")]
    [SerializeField] private float _baseSplashDamageMultiplier;
    [SerializeField] private float _baseSplashRadius;

    [Header("Defense")]
    [SerializeField] private float _baseDefense;
    [SerializeField] private float _currentDefense;

    [Header("Health")]
    [SerializeField] private float _baseHealth;
    [SerializeField] private float _currentHealth;
   
    [SerializeField] private float _baseHealthRegenAmount;
    [SerializeField] private float _currentHealthRegenAmount;
    
    [SerializeField] private float _baseHealthRegenSpeed;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Level Up
    [Space(15)]
    [Header("=== Level Up ===")]

    [Header("LVL")]
    [SerializeField] private int _currentLvl;

    [Header("XP")]
    [SerializeField] private int _baseXP;
    [SerializeField] private int _currentXP;
    [SerializeField] private int _requiredXP;

    [Header("Base Stats")]
    [SerializeField] private float _defensePercentageIncrease;
    [SerializeField] private float _healthPercentageIncrease;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // RUNTIME
    private bool _canAttack;
    private GameObject _target;
    private BoxCollider _targetBoxCollider;
    private Collider[] _splashHitColliders;
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;
    private GlobalTimer _healthRegenTimer;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    public event Action<float> OnHealthChanged;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    private void Start()
    {
        _currentAttackDamage = _baseAttackDamage;
        _currentCritChance = _baseCritChance;

        _currentDefense = _baseDefense;
        _currentHealth = _baseHealth;
        _currentHealthRegenAmount = _baseHealthRegenAmount;

        _requiredXP = _baseXP * (_currentLvl * _currentLvl);

        _canAttack = true;

        _attackTimer = new GlobalTimer(_baseAttackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
        _healthRegenTimer = new GlobalTimer(_baseHealthRegenSpeed);

        if (_animator)
        {
            _animator.SetFloat("AttackSpeedMultiplier", _animationAttackMultiplier);
        }
    }

    private void Update()
    {
        HealthRegen();

        if (_shouldSyncAttackAnim)
        {
            SyncAttackAnimation();
            return;
        }

        if (!_canAttack)
        {
            AttackCooldown();
            return;
        }

        if (_canAttack)
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

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _targetLayersMask))
        {
            _target = hit.collider.gameObject;
            _targetBoxCollider = _target.GetComponent<BoxCollider>();

            UpdateLookAt();
        }
    }

    private bool AttackTarget()
    {
        if (_target != null && _targetBoxCollider != null)
        {
            UpdateLookAt();

            if (_animator != null)
            {
                _animator.Play("Attack");
            }

            _shouldSyncAttackAnim = true;

            return true;
        }

        return false;
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
            AudioManager.Instance.PlaySFXClip(_attackSFXClip, 0.25f, _target.transform.position);

            if (_target != null && _targetBoxCollider != null)
            {
                GameObject thunderVFX;

                float finalAttackDamage = _currentAttackDamage * UnityEngine.Random.Range(0.9f, 1.1f);

                float finalSplashDamage = (_currentAttackDamage * _baseSplashDamageMultiplier) * UnityEngine.Random.Range(0.9f, 1.1f);

                if (UnityEngine.Random.value <= _currentCritChance)
                {
                    finalAttackDamage *= _baseCritMultiplier;
                    finalSplashDamage *= _baseCritMultiplier;

                    thunderVFX = Instantiate(_thunderCritPrefab, _target.transform.position, Quaternion.identity);
                }
                else
                {
                    thunderVFX = Instantiate(_thunderPrefab, _target.transform.position, Quaternion.identity);
                }

                Destroy(thunderVFX, _thunderLifeTime);

                IDamageable target = _target.GetComponent<IDamageable>();
                target.TakeDamage(finalAttackDamage);

                _splashHitColliders = Physics.OverlapSphere(_target.transform.position, _baseSplashRadius, ~_ignoredLayersMask);

                foreach (Collider hit in _splashHitColliders)
                {
                    if (hit.gameObject == _target.gameObject) continue;

                    IDamageable splashTarget = hit.GetComponent<IDamageable>();

                    splashTarget?.TakeDamage(finalSplashDamage);

                }
            }

            _shouldSyncAttackAnim = false;

            _syncAnimTimer.Reset();
        }
    }

    private void UpdateLookAt()
    {
        if (_target != null && _targetBoxCollider != null)
        {
            transform.LookAt(_target.transform);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }


    private void HealthRegen()
    {
        if (_currentHealth < _baseHealth)
        {
            _healthRegenTimer.Tick();

            if (_healthRegenTimer.Flag)
            {
                _currentHealth = Mathf.Min(_currentHealth + _currentHealthRegenAmount, _baseHealth);

                _healthRegenTimer.Reset();

                OnHealthChanged?.Invoke(_currentHealth / _baseHealth);
            }
        }
    }

    public void AddExp(int exp)
    {
        _currentXP += exp;

        while (_currentXP >= _requiredXP)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _currentXP -= _requiredXP;
        _currentLvl++;

        _baseDefense += _baseDefense * _defensePercentageIncrease;
        _baseHealth += _baseHealth * _healthPercentageIncrease;

        _currentHealth = _baseHealth;

        _requiredXP = _baseXP * (_currentLvl * _currentLvl);
    }


    // --- Interface ---
    public void TakeDamage(float damage)
    {
        float damageReduction = _currentDefense / (_currentDefense + 50f);
        float effectiveDamage = damage * (1f - damageReduction);

        _currentHealth -= effectiveDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _baseHealth);

        DisplayDamage(effectiveDamage);
        OnHealthChanged?.Invoke(_currentHealth / _baseHealth);

        if (_currentHealth <= 0f)
        {
            // ToDo: Game Over
        }
    }


    private void DisplayDamage(float damage)
    {
        if (_damageText)
        {
            float roundedDamage = Mathf.Round(damage * 100f) / 100f;

            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.25f, transform.position.x + 0.5f),
                                                UnityEngine.Random.Range(_damageTextYOffset - 0.5f, _damageTextYOffset + 0.5f), transform.position.z);

            var floatingTextObject = Instantiate(_damageText, spawnPosition, Quaternion.identity, transform);

            if (floatingTextObject)
            {
                var floatingTextMesh = floatingTextObject.GetComponent<TextMesh>();

                if (floatingTextMesh)
                {
                    floatingTextMesh.color = new Color(1f, UnityEngine.Random.Range(0, 100) / 255f, UnityEngine.Random.Range(0, 255) / 255f, 1f);
                    floatingTextMesh.text = roundedDamage.ToString();
                }
            }
        }
    }
}

/*private GameObject FindClosest()
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
   }*/