using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EStatsType { EAttackDamage, EAttackSpeed, ECritMultiplier, ECritChance, ESplashDamage, ESplashRadius, EHealthRegenAmount, EHealthRegenSpeed, EHealth, EExperience }

public class Player : MonoBehaviour, IDamageable
{
    #region Components
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _targetLayersMask;
    [SerializeField] private LayerMask _ignoredLayersMask;
    [SerializeField] private Image _healthFillImage;
    #endregion

    #region VFX
    [Space(15)]
    [Header("VFX")]
    [SerializeField] private GameObject _thunderPrefab;
    [SerializeField] private GameObject _thunderCritPrefab;

    [SerializeField] private float _thunderLifeTime;

    [Space(10)]
    [SerializeField] private GameObject _buffUpVFX;
    #endregion

    #region SFX
    [Header("SFX")]
    [Space(15)]
    [SerializeField] private AudioClip _attackSFX;
    [SerializeField, Range(0f, 1f)] private float _attackSFXVolume;

    [SerializeField] private AudioClip _buffUpSFX;
    [SerializeField, Range(0f, 1f)] private float _buffUpSFXVolume;
    #endregion

    #region Stats
    [Space(15)]
    [Header("Stats")]

    [Space(10)]
    [SerializeField] private float _baseAttackDamage;
    [SerializeField] private float _currentAttackDamage;

    [SerializeField] private float _baseAttackSpeed;
    [SerializeField] private float _currentAttackSpeed;

    [SerializeField] private float _baseCritMultiplier;
    [SerializeField] private float _currentCritMultiplier;

    [SerializeField] private float _baseCritChance;
    [SerializeField] private float _currentCritChance;

    [Space(10)]
    [SerializeField] private float _baseSplashDamage;
    [SerializeField] private float _currentSplashDamage;

    [SerializeField] private float _baseSplashRadius;
    [SerializeField] private float _currentSplashRadius;

    [Space(10)]
    [SerializeField] private float _baseHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    private float _inverseMaxHealth;

    [SerializeField] private float _baseHealthRegenAmount;
    [SerializeField] private float _currentHealthRegenAmount;

    [SerializeField] private float _baseHealthRegenSpeed;
    [SerializeField] private float _currentHealthRegenSpeed;

    [Space(10)]
    [SerializeField] private float _currentXP;
    [SerializeField] private float _maxXP;
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

    // --- VFX ---
    private GameObject _buffUpInstance;

    // --- Timers ---
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;

    private GlobalTimer _healthRegenTimer;

    // --- Runtime References ---
    [HideInInspector] public static Player _SPlayerScript;
    [HideInInspector] public List<GameObject> _enemies;
    #endregion

    private void Awake()
    {
        _inverseMaxHealth = 1f / _maxHealth;

        _attackTimer = new GlobalTimer(_currentAttackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
        _healthRegenTimer = new GlobalTimer(_currentHealthRegenSpeed);

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
                AudioManager.SAudioManager.PlaySoundFXClip(_attackSFX, _attackSFXVolume, _target.transform.position);

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

    private void HealthRegen()
    {
        if (_currentHealth < _maxHealth)
        {
            _healthRegenTimer.Tick();

            if (_healthRegenTimer.Flag)
            {
                _currentHealth = Mathf.Min(_currentHealth + _currentHealthRegenAmount, _maxHealth);

                _healthRegenTimer.Reset();
                UpdateHealthBar();
            }
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = _currentHealth * _inverseMaxHealth;

        if (_healthFillImage != null)
        {
            _healthFillImage.fillAmount = healthPercent;
        }
    }

    private void DisplayDamage(float damage)
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
                    floatingTextMesh.text = damage.ToString();
                }
            }
        }
    }

    private void ResetPlayer()
    {
        //Health
        _currentHealth = _baseHealth;
        _maxHealth = _baseHealth;

        //Regeberation
        _currentHealthRegenAmount = _baseHealthRegenAmount;
        _currentAttackDamage = _baseAttackDamage;

        //Attack Speed
        _currentAttackSpeed = _baseAttackSpeed;

        //ToDo Pres 
    }


    // --- Apply Buffs ---
    public void ApplyBuff(EStatsType type, float value)
    {
        if (_buffUpInstance != null)
        {
            Destroy(_buffUpInstance);
        }

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

    public void InstantiateBuffSoundVisualEffect()
    {
        if (_buffUpVFX != null)
        {
            Vector3 buffUpVFXPosition = new Vector3(transform.position.x, transform.position.y + 0.35f, transform.position.z);
            _buffUpInstance = Instantiate(_buffUpVFX, buffUpVFXPosition, transform.rotation);

            var ps = _buffUpInstance.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.useUnscaledTime = true;
        }

        if (_buffUpSFX != null)
        {
            AudioManager.SAudioManager.PlaySoundFXClip(_buffUpSFX, _buffUpSFXVolume, transform.position);
        }

        GameManager._SGameManager.PauseGame();
    }


    // --- Interface ---
    public void TakeDamage(float dmg)
    {
        _currentHealth -= dmg;
        DisplayDamage(dmg);
        UpdateHealthBar();

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