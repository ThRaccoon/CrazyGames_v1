using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------------------------------------
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _targetLayerMask;
    // ----------------------------------------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------------------------------------
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _health;
    #region Getters / Setters

    public float Health
    {
        get => _health;
        set => _health = value;
    }
    #endregion

    [SerializeField] private float _damage;
    #region Getters / Setters

    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }
    #endregion

    [SerializeField] private float _attackSpeed;
    #region Getters / Setters

    public float AttackSpeed
    {
        get => _attackSpeed;
        set => _attackSpeed = value;
    }
    #endregion

    [SerializeField] private float _attackRange;
    #region Getters / Setters

    public float AttackRange
    {
        get => _attackRange;
        set => _attackRange = value;
    }
    #endregion
    // ----------------------------------------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------------------------------------
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private float _syncedAttackAnimLength;
    // ----------------------------------------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------------------------------------
    [Space(15)]
    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileMoveSpeed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private float _projectileYOffset;

    private GameObject _projectileTarget;
    private Vector3 _projectileTargetInitPos;
    private Vector3 _projectileSpawnPoint;
    // ----------------------------------------------------------------------------------------------------------------------------------

    // --- Timers ---
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;
    private bool _canAttack = true;
    private bool _shouldSyncAttackAnim;

    // --- Public ---
    [HideInInspector] public static Player SPlayerScript;
    [HideInInspector] public List<GameObject> enemies;

    private void Awake()
    {
        // Projectile
        _projectileSpawnPoint = new Vector3(transform.position.x, transform.position.y + _projectileYOffset, transform.position.z);

        // Timers
        _attackTimer = new GlobalTimer(_attackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);

        // Public
        SPlayerScript = this;
        enemies = new List<GameObject>();
    }


    private void Update()
    {
        if (_health <= 0)
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
            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint, Quaternion.identity);
            projectile.GetComponent<Projectile>().Init(_projectileMoveSpeed, _projectileLifeTime, _projectileTargetInitPos, _projectileTarget, _damage);

            _shouldSyncAttackAnim = false;

            _syncAnimTimer.Reset();
        }
    }

    private GameObject FindClosest()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject obj in enemies)
        {
            if (obj == null) continue;

            float distance = Vector3.Distance(obj.transform.position, gameObject.transform.position);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + _projectileYOffset, transform.position.z), Vector3.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}