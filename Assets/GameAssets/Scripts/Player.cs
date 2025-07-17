using System.Collections.Generic;
using UnityEngine;

public enum EStatsType { EHealth, EHealthRegen, EAttackDamage, EAttackSpeed, ESplashDamage }

public class Player : MonoBehaviour, IDamageable
{
    #region Components
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _targetLayersMask;
    [SerializeField] private LayerMask _ignoredLayersMask;
    #endregion

    #region VFX
    [Header("VFX")]
    [SerializeField] private GameObject _thunderPrefab;
    [SerializeField] private GameObject _thunderCritPrefab;

    [SerializeField] private float _thunderLifeTime;
    #endregion

    #region Stats
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _baseHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;

    [SerializeField] private float _baseHealthRegen;
    [SerializeField] private float _currentHealthRegen;

    [SerializeField] private float _baseHealthRegenRate;
    [SerializeField] private float _currentHealthRegenRate;

    [Space(10)]
    [SerializeField] private float _baseAttackDamage;
    [SerializeField] private float _currentAttackDamage;

    [SerializeField] private float _baseAttackSpeed;
    [SerializeField] private float _currentAttackSpeed;

    [Space(10)]
    [SerializeField] private float _baseSplashDamage;
    [SerializeField] private float _currentSplashDamage;

    [SerializeField] private float _baseSplashRadius;
    [SerializeField] private float _currentSplashRadius;

    [SerializeField] private float _baseCritChance;
    [SerializeField] private float _currentCritChance;

    [SerializeField] private float _baseCritMultiplier;
    [SerializeField] private float _currentCritMultiplier;
    #endregion

    #region Animations
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private float _syncedAttackAnimLength;
    [SerializeField] private float _animationAttackMultiplier;

    private bool _shouldSyncAttackAnim;
    #endregion

    #region Floating Text
    [Space(15)]
    [Header("Floating Text")]
    [SerializeField] private GameObject _floatingText;
    [SerializeField] private float _floatingTextYOffset;
    #endregion

    #region Runtime
    // --- Attack & Targeting ---
    private bool _canAttack = true;
    private GameObject _target;
    private BoxCollider _targetBoxCollider;
    private Collider[] _splashHitColliders;

    // --- Timers ---
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;

    private GlobalTimer _healthRegenTimer;

    // --- Runtime References ---
    [HideInInspector] public static Player _SPlayerScript;
    [HideInInspector] public List<GameObject> _enemies;
    #endregion

    [SerializeField] private AudioClip _attackSound;
    [SerializeField, Range(0f, 1f)] private float _attackSoundVolume;

    private void Awake()
    {
        _attackTimer = new GlobalTimer(_currentAttackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
        _healthRegenTimer = new GlobalTimer(_currentHealthRegenRate);

        if (_animator)
        {
            _animator.SetFloat("AttackSpeedMultiplier", _animationAttackMultiplier);
        }

        _SPlayerScript = this;
        _enemies = new List<GameObject>();
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
            if (_target != null && _targetBoxCollider != null)
            {
                AudioManager.SAudioManager.PlaySoundFXClip(_attackSound, _attackSoundVolume, _target.transform.position);

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
                    if (hit.gameObject == _target.gameObject)
                        continue;

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

    private void HealthRegen()
    {
        if (_currentHealth < _maxHealth)
        {
            _healthRegenTimer.Tick();

            if (_healthRegenTimer.Flag)
            {
                _currentHealth = Mathf.Min(_currentHealth + _currentHealthRegen, _maxHealth);

                _healthRegenTimer.Reset();
            }
        }
    }

    private void ResetPlayer()
    {
        //Health
        _currentHealth = _baseHealth;
        _maxHealth = _baseHealth;

        //Regeberation
        _currentHealthRegen = _baseHealthRegen;
        _currentAttackDamage = _baseAttackDamage;

        //Attack Speed
        _currentAttackSpeed = _baseAttackSpeed;

        //ToDo Pres 
    }


    // --- Apply Buffs ---
    public void ApplyBuff(EStatsType type, float value)
    {
        switch (type)
        {
            case EStatsType.EHealth:
                {
                    value = 1 + (value / 100);
                    _currentHealth *= value;
                }
                break;
            case EStatsType.EHealthRegen:
                {
                    value = 1 + (value / 100);
                    _currentHealthRegen *= value;
                }
                break;
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

                    if (_animator)
                    {
                        _animationAttackMultiplier *= 1 + (value / 100);
                        _animator.SetFloat("AttackSpeedMultiplier", _animationAttackMultiplier);
                    }
                }
                break;
            case EStatsType.ESplashDamage:
                {
                    value = 1 + (value / 100);
                    _currentSplashDamage *= value;
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
                    _baseHealth += value;
                }
                break;
            case EStatsType.EHealthRegen:
                {
                    _baseHealthRegen += value;
                }
                break;
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
            case EStatsType.ESplashDamage:
                {
                    _baseSplashDamage += value;
                }
                break;
        }
    }


    // --- Interface ---
    public void TakeDamage(float dmg)
    {
        _currentHealth -= dmg;
        DisplayDamage(dmg);

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