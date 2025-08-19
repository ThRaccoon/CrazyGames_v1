using System;
using UnityEngine;

public enum EStatsType { None, EAttackDamage, EAttackSpeed, ECritMultiplier, ECritChance, ESplashDamage, ESplashRadius, EHealthRegenAmount, EHealthRegenSpeed, EHealth, EExperience }

public class Player : MonoBehaviour, IDamageable
{
    // ====================================================================================================
    // === General ===
    [Header("General")]
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private LayerMask _targetLayersMask;
    [SerializeField] private LayerMask _ignoredLayersMask;
    // ====================================================================================================

    // ====================================================================================================
    // === Animations ===
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private Animator _animator;

    [SerializeField] private float _syncedAttackAnimLength;
    [SerializeField] private float _animationAttackMultiplier;

    private bool _shouldSyncAttackAnim;
    // ====================================================================================================

    // ====================================================================================================
    // === VFX ===
    [Space(15)]
    [Header("VFX")]
    [SerializeField] private GameObject _thunderPrefab;
    [SerializeField] private GameObject _thunderCritPrefab;

    [SerializeField] private float _thunderLifeTime;

    [Space(5)]
    [Header("Damage Text")]
    [SerializeField] private GameObject _damageText;

    [SerializeField] private float _damageTextYOffset;
    // ====================================================================================================

    // ====================================================================================================
    // === SFX ===
    [Space(15)]
    [Header("SFX")]
    [SerializeField] private AudioClip _attackSFXClip;
    [SerializeField, Range(0f, 1f)] private float _attackSFXVolume;

    [Space(5)]
    [SerializeField] private AudioClip _buffUpSFXClip;
    [SerializeField, Range(0f, 1f)] private float _buffUpSFXVolume;
    // ====================================================================================================

    // ====================================================================================================
    // === Stats ===
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _baseAttackDamage;
    [SerializeField] private float _currentAttackDamage;

    [SerializeField] private float _baseAttackSpeed;
    [SerializeField] private float _currentAttackSpeed;

    [SerializeField] private float _baseCritMultiplier;
    [SerializeField] private float _currentCritMultiplier;

    [SerializeField] private float _baseCritChance;
    [SerializeField] private float _currentCritChance;

    [Space(5)]
    [SerializeField] private float _baseSplashDamage;
    [SerializeField] private float _currentSplashDamage;

    [SerializeField] private float _baseSplashRadius;
    [SerializeField] private float _currentSplashRadius;

    [Space(5)]
    [SerializeField] private float _baseHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;

    [SerializeField] private float _baseHealthRegenAmount;
    [SerializeField] private float _currentHealthRegenAmount;

    [SerializeField] private float _baseHealthRegenSpeed;
    [SerializeField] private float _currentHealthRegenSpeed;

    [Space(5)]
    [SerializeField] private float _currentXP;
    [SerializeField] private float _maxXP;
    // ====================================================================================================

    // ====================================================================================================
    // === RUNTIME ===
    private bool _canAttack = true;

    private GameObject _target;

    private BoxCollider _targetBoxCollider;
    private Collider[] _splashHitColliders;

    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;
    private GlobalTimer _healthRegenTimer;
    // ====================================================================================================

    // ====================================================================================================
    // === Events ===
    public event Action<float> OnHealthChanged;
    // ====================================================================================================

    private void Start()
    {
        _attackTimer = new GlobalTimer(_currentAttackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
        _healthRegenTimer = new GlobalTimer(_currentHealthRegenSpeed);

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
                float finalAttackDamage = _currentAttackDamage;
                float finalSplashDamage = _currentSplashDamage;

                if (UnityEngine.Random.value <= _currentCritChance)
                {
                    finalAttackDamage *= _currentCritMultiplier;
                    finalSplashDamage *= _currentCritMultiplier;

                    thunderVFX = Instantiate(_thunderCritPrefab, _target.transform.position, Quaternion.identity);
                }
                else
                {
                    thunderVFX = Instantiate(_thunderPrefab, _target.transform.position, Quaternion.identity);
                }

                Destroy(thunderVFX, _thunderLifeTime);

                IDamageable target = _target.GetComponent<IDamageable>();
                target.TakeDamage(finalAttackDamage);

                _splashHitColliders = Physics.OverlapSphere(_target.transform.position, _currentSplashRadius, ~_ignoredLayersMask);

                foreach (Collider hit in _splashHitColliders)
                {
                    if (hit.gameObject == _target.gameObject) continue;

                    IDamageable splashTarget = hit.GetComponent<IDamageable>();
                    if (splashTarget != null)
                    {
                        splashTarget.TakeDamage(finalSplashDamage);
                    }
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
        if (_currentHealth < _maxHealth)
        {
            _healthRegenTimer.Tick();

            if (_healthRegenTimer.Flag)
            {
                _currentHealth = Mathf.Min(_currentHealth + _currentHealthRegenAmount, _maxHealth);

                _healthRegenTimer.Reset();

                OnHealthChanged?.Invoke(_currentHealth / _maxHealth);
            }
        }
    }

    private void DisplayDamage(float damage)
    {
        if (_damageText)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.25f, transform.position.x + 0.5f),
                                                UnityEngine.Random.Range(_damageTextYOffset - 0.5f, _damageTextYOffset + 0.5f), transform.position.z);

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


    // --- Apply Buffs ---
    public void ApplyBuff(EStatsType type, float value)
    {
        switch (type)
        {
            case EStatsType.EAttackDamage:
                {
                    value = 1 + (value / 100);
                    _currentAttackDamage *= value;
                }
                break;
            case EStatsType.EAttackSpeed:
                {
                    float temp = 1 - (value / 100);

                    _currentAttackSpeed *= temp;
                    _attackTimer.Duration = _currentAttackSpeed;
                    _syncedAttackAnimLength *= temp;
                    _syncAnimTimer.Duration = _syncedAttackAnimLength;

                    if (_animator != null)
                    {
                        _animationAttackMultiplier *= 1 + (value / 100);
                        _animator.SetFloat("AttackSpeedMultiplier", _animationAttackMultiplier);
                    }
                }
                break;
            case EStatsType.ECritMultiplier:
                {
                    value = 1 + (value / 100);
                    _currentCritMultiplier *= value;
                }
                break;
            case EStatsType.ECritChance:
                {
                    value = 1 + (value / 100);
                    _currentCritChance *= value;
                }
                break;
            case EStatsType.ESplashDamage:
                {
                    value = 1 + (value / 100);
                    _currentSplashDamage *= value;
                }
                break;
            case EStatsType.ESplashRadius:
                {
                    value = 1 + (value / 100);
                    _currentSplashRadius *= value;
                }
                break;
            case EStatsType.EHealthRegenAmount:
                {
                    value = 1 + (value / 100);
                    _currentHealthRegenAmount *= value;
                }
                break;
            case EStatsType.EHealthRegenSpeed:
                {
                    value = 1 - (value / 100);
                    _currentHealthRegenSpeed *= value;
                }
                break;
            case EStatsType.EHealth:
                {
                    value = 1 + (value / 100);
                    _currentHealth += value;

                    if (_currentHealth > _maxHealth)
                    {
                        _currentHealth = _maxHealth;
                    }
                }
                break;
        }
    }

    public void AddStats(EStatsType type, float value)
    {
        switch (type)
        {
            case EStatsType.EAttackDamage:
                {
                    _baseAttackDamage += value;
                }
                break;
            case EStatsType.EAttackSpeed:
                {
                    _baseAttackSpeed -= value;
                }
                break;
            case EStatsType.ECritMultiplier:
                {
                    _baseCritMultiplier += value;
                }
                break;
            case EStatsType.ECritChance:
                {
                    _baseCritChance += value;
                }
                break;
            case EStatsType.ESplashDamage:
                {
                    _baseSplashDamage += value;
                }
                break;
            case EStatsType.ESplashRadius:
                {
                    _baseSplashRadius += value;
                }
                break;
            case EStatsType.EHealthRegenAmount:
                {
                    _baseHealthRegenAmount += value;
                }
                break;
            case EStatsType.EHealthRegenSpeed:
                {
                    _baseHealthRegenSpeed -= value;
                }
                break;
            case EStatsType.EHealth:
                {
                    _baseHealth += value;
                }
                break;
        }
    }

    // --- Interface ---
    public void TakeDamage(float dmg)
    {
        _currentHealth -= dmg;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        DisplayDamage(dmg);
        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);

        // ToDo Game Over
        // if (_healthCurrent <= 0){}
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